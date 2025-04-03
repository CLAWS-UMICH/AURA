Shader "URP/PassthroughFade_NoBranch"
{
    Properties
    {
        _PassthroughOpacity("Passthrough Opacity", Range(0,1)) = 1
        _Mode("Render Mode", Int) = 0 // 0=VR, 1=AR, 2=MR
        _FadeCenter ("Fade Center", Vector) = (0,0,0,0)
        _FadeScale ("Fade Scale", Vector) = (1,1,1,1)
        _FadeStart ("Fade Start", Float) = 0
        _FadeSize ("Fade Size", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Overlay" "RenderPipeline"="UniversalPipeline"
        }
        LOD 100

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float _PassthroughOpacity;
            int _Mode; // 0=VR, 1=AR, 2=MR
            float4x4 _WorldToVolume;
            float3 _FadeCenter;
            float3 _FadeScale;
            float _FadeStart;
            float _FadeSize;
        CBUFFER_END

        float IsInsideVolume(float3 positionWS)
        {
            float3 volumePos = mul(_WorldToVolume, float4(positionWS, 1.0)).xyz;

            // abs(volumePos.x)<0.4 => inside if: 1 - step(0.4, abs(volumePos.x))
            // This yields 1 if inside, 0 if outside for each axis
            float insideX = 1.0 - step(0.4, abs(volumePos.x));
            float insideY = 1.0 - step(0.4, abs(volumePos.y));
            float insideZ = 1.0 - step(0.4, abs(volumePos.z));
            return insideX * insideY * insideZ;
        }

        /// <summary>
        /// Returns fadeFactor:
        ///   1.0 inside region
        ///   0.0 outside region
        ///   between 0 and 1 in border region
        /// </summary>
        float ComputeFadeFactor(float3 positionOS, float3 fadeCenter, float3 fadeScale, float fadeStart, float fadeSize)
        {
            float3 scaledPos = positionOS / fadeScale;
            float distance = length(scaledPos - fadeCenter);

            float boundary = fadeStart + fadeSize;
            float borderSize = 0.01;
            float distToBoundary = distance - boundary;

            // Branched conditions:
            // if(distToBoundary>0) fade=0
            // else if(distToBoundary>-borderSize) fade = saturate((-distToBoundary)/borderSize)
            // else fade=1
            float outside = step(0.0, distToBoundary);
            float inside = 1.0 - step(-borderSize, distToBoundary);
            float border = 1.0 - inside - outside;

            float fadeBorder = saturate((-distToBoundary) / borderSize);
            float fade = inside * 1.0 + border * fadeBorder + outside * 0.0;
            return fade;
        }

        float SampleDitherPattern(float2 pixelCoord)
        {
            float ditherPattern[16] =
            {
                0.0/16.0,  8.0/16.0,  2.0/16.0, 10.0/16.0,
                12.0/16.0, 4.0/16.0, 14.0/16.0,  6.0/16.0,
                3.0/16.0, 11.0/16.0,  1.0/16.0,  9.0/16.0,
                15.0/16.0,7.0/16.0, 13.0/16.0,  5.0/16.0
            };

            float px = floor(pixelCoord.x);
            float py = floor(pixelCoord.y);
            float pxMod = fmod(px, 4.0);
            float pyMod = fmod(py, 4.0);
            float patternIndexF = pyMod * 4.0 + pxMod;
            int patternIndex = (int)patternIndexF;
            return ditherPattern[patternIndex];
        }

        struct Attributes
        {
            float4 positionOS : POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS : TEXCOORD0;
            float3 positionOS : TEXCOORD1;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };
        ENDHLSL

        Pass
        {
            Name "Passthrough Fade"
            Blend Zero One, One Zero
            ZWrite On
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vpi = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vpi.positionCS;
                output.positionWS = vpi.positionWS;
                output.positionOS = input.positionOS.xyz;
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // isVR=1 if _Mode=0, else 0
                // step(0.5, (float)_Mode) = 0 if _Mode<0.5 => VR mode, else 1
                // isVR = 1 - step(0.5, (float)_Mode)
                float isVR = 1.0 - step(0.5, (float)_Mode);

                // Compute fade factor
                float fadeFactor = ComputeFadeFactor(input.positionOS, _FadeCenter, _FadeScale, _FadeStart, _FadeSize);

                // Only in VR mode we do dithering
                // In VR mode:
                // if (fadeFactor==0) discard
                // else if (fadeFactor<1) { threshold=pattern; if(fadeFactor<threshold) discard }
                // else fadeFactor=1 => no discard
                //
                // Non VR mode: no discard
                float2 uv = input.positionCS.xy / input.positionCS.w;
                float2 pixelCoord = 0.5 * (uv + 1.0) * float2(_ScreenParams.x, _ScreenParams.y);
                float threshold = SampleDitherPattern(pixelCoord);

                // Detect fadeFactor=0
                // fadeFactorIsZero=1 if fadeFactor=0 else 0
                // Use a small epsilon to handle floating precision: if fadeFactor<0.0000001 assume zero.
                float fadeFactorIsZero = 1.0 - step(0.0000001, fadeFactor);

                // border region discard:
                // happens if fadeFactor<1 and fadeFactor<threshold
                float lessThanOne = 1.0 - step(1.0, fadeFactor);
                float lessThanThreshold = 1.0 - step(threshold, fadeFactor);
                float borderDiscard = lessThanOne * lessThanThreshold;

                // Combine conditions:
                // discard if VR and (fadeFactorIsZero=1 or borderDiscard=1)
                float discardCondition = isVR * max(fadeFactorIsZero, borderDiscard);

                // If discardCondition=1 => clip
                // clip(x) discards if x<0
                // We want to discard if discardCondition=1. Use clip(discardCondition-0.5) => if discardCondition=1 => 1-0.5=0.5>0 no discard?
                // We need negative for discard. We can do clip((discardCondition*2.0)-1.0):
                // If discardCondition=1 => (2*1)-1=1 => not negative. We need negative. Let's simply use:
                // clip(-discardCondition) => if discardCondition=1 => -1 discard
                // if discardCondition=0 => 0 discard? clip(0) doesn't discard. Perfect.
                clip(-discardCondition);

                // Final color
                // VR mode => return float4(0,0,0,0)
                // non VR mode => return float4(0,0,0,(1 - _PassthroughOpacity))
                float4 vrColor = float4(0,0,0,0);
                float4 otherColor = float4(0,0,0,(1.0 - _PassthroughOpacity));
                float4 finalColor = vrColor * isVR + otherColor * (1.0 - isVR);
                return finalColor;
            }
            ENDHLSL
        }
    }
}
