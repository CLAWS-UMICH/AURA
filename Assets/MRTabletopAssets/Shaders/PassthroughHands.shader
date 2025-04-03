Shader "URP/Passthrough Hands"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        _EdgeHighlightPower ("Edge Highlight Power", Float) = 1
        _FadeCenter ("Fade Center", Vector) = (0,0,0,0)
        _FadeScale ("Fade Scale", Vector) = (1,1,1,1)
        _FadeStart ("Fade Start", Float) = 0
        _FadeSize ("Fade Size", Float) = 1
        _NoiseStrength ("Noise Strength", Float) = 0.1
        _CombinedFingerIndexMask ("Finger Index Mask", 2D) = "white" {}
        _ThumbColor ("Thumb Color", Color) = (1,0,0,1)
        _FingerColor_1 ("Finger Color 1", Color) = (0,1,0,1)
        _FingerColor_2 ("Finger Color 2", Color) = (0,0,1,1)
        _FingerColor_3 ("Finger Color 3", Color) = (1,1,0,1)
        _FingerColor_4 ("Finger Color 4", Color) = (1,0,1,1)
        _AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        _Mode("Render Mode", Int) = 0 // 0=VR, 1=AR, 2=MR
        _ARPassthroughAlpha("_ARPassthroughAlpha", Float) = 1.0
        _PassthroughAlpha("_PassthroughAlpha", Float) = 1.0
        _PassthroughBoundaryCrossAlpha("_PassthroughBoundaryCrossAlpha", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Blend One OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : NORMAL;
                float3 viewDirWS : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 positionOS : TEXCOORD2;
                float2 uv : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_CombinedFingerIndexMask);
            SAMPLER(sampler_CombinedFingerIndexMask);

            float4x4 _WorldToVolume;
            float4x4 _WorldToVolumePlayRegion;

            CBUFFER_START(UnityPerMaterial)
                float4 _MainColor;
                float4 _EdgeColor;
                float _EdgeHighlightPower;
                float3 _FadeCenter;
                float3 _FadeScale;
                float _FadeStart;
                float _FadeSize;
                float _NoiseStrength;
                float4 _CombinedFingerIndexMask_ST;
                float4 _ThumbColor;
                float4 _FingerColor1;
                float4 _FingerColor2;
                float4 _FingerColor3;
                float4 _FingerColor4;
                float _AlphaCutoff;
                int _Mode; // 0=VR, 1=AR, 2=MR
                float _ARPassthroughAlpha;
                float _PassthroughAlpha;
                float _PassthroughBoundaryCrossAlpha;
            CBUFFER_END

            //---------------------------------------
            // Helper Functions (no branching)
            //---------------------------------------

            /// <summary>
            /// Compute the fading factor based on the object's position.
            /// </summary>
            float ComputeFadeFactor(float3 positionOS, float3 fadeCenter, float3 fadeScale, float fadeStart, float fadeSize)
            {
                float3 scaledPos = positionOS / fadeScale;
                float distance = length(scaledPos - fadeCenter);
                float normalizedDist = (distance - fadeStart) / fadeSize;
                float fade = smoothstep(0.0, 1.0, normalizedDist);
                return 1.0 - fade;
            }

            /// <summary>
            /// Returns a finger color without branching.
            /// fingerIndex expected to be between 1 and 5.
            /// </summary>
            float4 GetFingerColor(float fingerIndex, float4 thumbColor, float4 f1, float4 f2, float4 f3, float4 f4)
            {
                float4 c0 = float4(0,0,0,0);
                float4 c1 = thumbColor;
                float4 c2 = f1;
                float4 c3 = f2;
                float4 c4 = f3;
                float4 c5 = f4;

                // Use stepped ranges to select correct color:
                float4 col_1 = step(0.5, fingerIndex)*step(fingerIndex,1.5)*c1;
                float4 col_2 = step(1.5, fingerIndex)*step(fingerIndex,2.5)*c2;
                float4 col_3 = step(2.5, fingerIndex)*step(fingerIndex,3.5)*c3;
                float4 col_4 = step(3.5, fingerIndex)*step(fingerIndex,4.5)*c4;
                float4 col_5 = step(4.5, fingerIndex)*step(fingerIndex,5.5)*c5;

                float4 combined = col_1 + col_2 + col_3 + col_4 + col_5;
                // If none matched, combined=0,0,0,0
                return combined;
            }

            /// <summary>
            /// Returns 1.0 if inside volume, 0.0 otherwise (no branching).
            /// </summary>
            float IsInsideVolume(float3 positionWS)
            {
                float3 volumePos = mul(_WorldToVolume, float4(positionWS, 1.0)).xyz;
                float insideX = 1.0 - step(0.5, abs(volumePos.x));
                float insideY = 1.0 - step(0.5, abs(volumePos.y));
                float insideZ = 1.0 - step(0.5, abs(volumePos.z));
                return insideX * insideY * insideZ;
            }

            /// <summary>
            /// Sample the 4x4 dithering pattern.
            /// </summary>
            float SampleDitherPattern(float2 pixelCoord)
            {
                float ditherPattern[16] =
                {
                    0.0/16.0, 8.0/16.0, 2.0/16.0,10.0/16.0,
                    12.0/16.0,4.0/16.0,14.0/16.0,6.0/16.0,
                    3.0/16.0,11.0/16.0,1.0/16.0,9.0/16.0,
                    15.0/16.0,7.0/16.0,13.0/16.0,5.0/16.0
                };

                int px = (uint)floor(pixelCoord.x) % 4;
                int py = (uint)floor(pixelCoord.y) % 4;
                int patternIndex = py * 4 + px;
                return ditherPattern[patternIndex];
            }

            /// <summary>
            /// Compute clip condition without branching:
            /// Returns a value 0 or 1, where 1 means clip.
            /// </summary>
            float ComputeClipCondition(float4 colorMix, float fadeFactor, float2 pixelCoord, float alphaCutoff)
            {
                float margin = 0.1;
                float lowerBound = alphaCutoff - margin;
                float upperBound = alphaCutoff + margin;

                // alpha < lowerBound => clip
                float clipConditionForAlpha = 1.0 - step(lowerBound, colorMix.a);

                // Within fade zone and alpha range => dither
                float fadeFactorInRange = 1.0 - step(1.0, fadeFactor);
                float alphaInRange = step(lowerBound, colorMix.a)*step(colorMix.a, upperBound);
                float threshold = SampleDitherPattern(pixelCoord);
                float t = saturate((colorMix.a - lowerBound) / (upperBound - lowerBound));
                float clipIfDither = fadeFactorInRange * alphaInRange * (1.0 - step(threshold, t));

                return max(clipConditionForAlpha, clipIfDither);
            }

            /// <summary>
            /// Compute final color before mode logic.
            /// </summary>
            float4 ComputeBaseColor(float3 normalWS, float3 viewDirWS)
            {
                float fresnelTerm = pow((1.0 - saturate(dot(normalWS, viewDirWS))), _EdgeHighlightPower);
                float4 mainColorContribution = _MainColor * (1.0 - fresnelTerm);
                float4 edgeColorContribution = _EdgeColor * fresnelTerm;
                return mainColorContribution + edgeColorContribution;
            }

            /// <summary>
            /// Combine wrist fade and finger colors without branching.
            /// </summary>
            float4 CombineColors(float4 finalColor, float fadeFactor, float4 fingerColor)
            {
                float wristColorAlpha = finalColor.a * fadeFactor;
                float maxAlpha = max(wristColorAlpha, fingerColor.a);
                float blendFactor = saturate(fingerColor.a);
                float4 colorMix = lerp(finalColor, fingerColor, blendFactor);
                colorMix.a = maxAlpha;

                // Determine VR or non-VR mode influence:
                // mode=0 (VR) => use (1 - _PassthroughAlpha)
                // mode!=0 => use _PassthroughAlpha
                float modeIsVR = 1.0 - step(0.5, (float)_Mode);
                float modeNotVR = 1.0 - modeIsVR;
                float modeAlphaBlend = (modeIsVR*(1.0 - _PassthroughAlpha) + modeNotVR*_PassthroughAlpha);

                colorMix.a *= _PassthroughBoundaryCrossAlpha * modeAlphaBlend;
                return colorMix;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionHCS = vertexInput.positionCS;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionWS = vertexInput.positionWS;
                output.viewDirWS = GetWorldSpaceViewDir(output.positionWS);
                output.positionOS = input.positionOS.xyz;
                output.uv = TRANSFORM_TEX(input.uv, _CombinedFingerIndexMask);
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = normalize(input.viewDirWS);

                float4 baseColor = ComputeBaseColor(normalWS, viewDirWS);
                float fadeFactor = ComputeFadeFactor(input.positionOS, _FadeCenter, _FadeScale, _FadeStart, _FadeSize);

                float4 fingerMask = SAMPLE_TEXTURE2D(_CombinedFingerIndexMask, sampler_CombinedFingerIndexMask, input.uv);
                float fingerIndex = floor(fingerMask.r * 5.999);
                float fingerStrength = fingerMask.a;
                float4 rawFingerColor = GetFingerColor(fingerIndex, _ThumbColor, _FingerColor1, _FingerColor2, _FingerColor3, _FingerColor4) * fingerStrength;

                float4 colorMix = CombineColors(baseColor, fadeFactor, rawFingerColor);

                float3 volumePos = mul(_WorldToVolume, float4(input.positionWS,1.0)).xyz;
                float insideVolume = IsInsideVolume(input.positionWS);

                // Determine mode:
                float isVR = 1.0 - step(0.5, (float)_Mode);                       // 1 if mode=0, else 0
                float isAR = step(0.5, (float)_Mode)* (1.0 - step(1.5,(float)_Mode)); // 1 if mode=1, else 0
                float isMR = step(1.5, (float)_Mode);                            // 1 if mode=2, else 0

                // Compute pixel coordinates for dithering:
                float2 uv = input.positionHCS.xy / input.positionHCS.w;
                float2 pixelCoord = 0.5 * (uv + 1.0) * float2(_ScreenParams.x, _ScreenParams.y);

                // Clip condition (no if):
                // For VR (mode=0), always apply clip logic.
                // For AR (mode=1), no hand, so no clip needed (will be masked out).
                // For MR (mode=2), only apply clip if insideVolume=1.
                float clipConditionRaw = ComputeClipCondition(colorMix, fadeFactor, pixelCoord, _AlphaCutoff);
                // Mask out AR:
                clipConditionRaw *= (1.0 - isAR);
                // For MR, only if insideVolume=1 or VR mode:
                clipConditionRaw *= (isVR + insideVolume*isMR);

                // Perform clip without branching:
                clip(-clipConditionRaw);

                // Final color without branching:
                // VR: finalColor = colorMix
                // AR: finalColor = 0
                // MR: finalColor = insideVolume ? colorMix : 0
                float4 vrColor = colorMix * isVR;
                float4 arColor = float4(0,0,0,0) * isAR;
                float4 mrColor = insideVolume * colorMix * isMR;

                float4 finalColor = vrColor + arColor + mrColor;

                return finalColor;
            }

            ENDHLSL
        }
    }
}
