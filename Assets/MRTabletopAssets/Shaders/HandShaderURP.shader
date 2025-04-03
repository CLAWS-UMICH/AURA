Shader "Custom/HandNoiseShader"
{
    Properties
    {
        [Header(Main Properties)]
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        _EdgeHighlightPower ("Edge Highlight Power", Float) = 1.0

        [Header(Finger Highlights)]
        _CombinedFingerIndexHighlightMask ("Finger Index Highlight Mask", 2D) = "white" {}
        _ThumbColor ("Thumb Color", Color) = (1,1,1,1)
        [HDR]_FingerColor1 ("Finger Color 1", Color) = (1,1,1,1)
        [HDR]_FingerColor2 ("Finger Color 2", Color) = (1,1,1,1)
        [HDR]_FingerColor3 ("Finger Color 3", Color) = (1,1,1,1)
        [HDR]_FingerColor4 ("Finger Color 4", Color) = (1,1,1,1)

        [Header(Fade Properties)]
        _FadeCenter ("Fade Center", Vector) = (0,0,0,0)
        _FadeScale ("Fade Scale", Vector) = (1,1,1,0)
        _FadeSize ("Fade Size", Float) = 1.0
        _FadeStart ("Fade Start", Float) = 0.0

        [Header(Noise Properties)]
        _NoiseScale ("Noise Scale", Float) = 1.0
        _NoiseStrength ("Noise Strength", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        // Depth prepass
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex DepthVert
            #pragma fragment DepthFrag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings DepthVert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            float4 DepthFrag(Varyings IN) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }

        Pass
        {
            Name "MainPass"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
            };

            TEXTURE2D(_CombinedFingerIndexHighlightMask);
            SAMPLER(sampler_CombinedFingerIndexHighlightMask);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainColor;
                float4 _EdgeColor;
                float _EdgeHighlightPower;
                float4 _ThumbColor;
                float4 _FingerColor1;
                float4 _FingerColor2;
                float4 _FingerColor3;
                float4 _FingerColor4;
                float3 _FadeCenter;
                float3 _FadeScale;
                float _FadeSize;
                float _FadeStart;
                float _NoiseScale;
                float _NoiseStrength;
            CBUFFER_END

            float2 hash2D2D(float2 s)
            {
                return frac(sin(mul(float2x2(127.1, 311.7, 269.5, 183.3), s)) * 43758.5453);
            }

            float voronoiNoise(float2 x)
            {
                float2 n = floor(x);
                float2 f = frac(x);

                float3 m = float3(8, 0, 0);
                for (int j = -1; j <= 1; j++)
                    for (int i = -1; i <= 1; i++)
                    {
                        float2 g = float2(i, j);
                        float2 o = hash2D2D(n + g);
                        o = 0.5 + 0.5 * sin(_Time.y + 6.2831 * o);
                        float2 r = g + o - f;
                        float d = dot(r, r);
                        if (d < m.x)
                            m = float3(d, o);
                    }
                return sqrt(m.x);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = IN.uv;
                OUT.screenPos = ComputeScreenPos(OUT.positionHCS);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // Base rim light calculation (for all fingers)
                float3 viewDirectionWS = normalize(GetCameraPositionWS() - IN.positionWS);
                float rimDot = 1.0 - saturate(dot(IN.normalWS, viewDirectionWS));
                float rimIntensity = pow(rimDot, _EdgeHighlightPower);
                float smoothRim = smoothstep(0.0, 1.0, rimIntensity);
                float4 rimLight = _EdgeColor * smoothRim;

                // Sample and process finger highlights with better blending
                float4 fingerMask = SAMPLE_TEXTURE2D(_CombinedFingerIndexHighlightMask,
                             sampler_CombinedFingerIndexHighlightMask, IN.uv);

                // Blend finger highlights more smoothly
                float4 fingerHighlights = _ThumbColor * smoothstep(0.0, 1.0, fingerMask.r) +
                    _FingerColor1 * smoothstep(0.0, 1.0, fingerMask.g) +
                    _FingerColor2 * smoothstep(0.0, 1.0, fingerMask.b) +
                    _FingerColor3 * smoothstep(0.0, 1.0, fingerMask.a) +
                    _FingerColor4 * smoothstep(0.0, 1.0, fingerMask.a);

                // Improved noise for fade
                float2 noiseUV = IN.uv * _NoiseScale;
                float noise = voronoiNoise(noiseUV);
                noise = lerp(0.7, noise, _NoiseStrength);

                // Fade calculation
                float3 fadePos = (IN.positionWS - _FadeCenter) * _FadeScale;
                float fadeDistance = length(fadePos);
                float baseFade = saturate((fadeDistance - _FadeStart) / _FadeSize);
                float smoothFade = smoothstep(0.0, 1.0, baseFade);

                // Combine fade and noise
                float fadeWithNoise = smoothFade;
                fadeWithNoise = lerp(fadeWithNoise, fadeWithNoise * noise, smoothFade);
                fadeWithNoise = smoothstep(0.0, 1.0, fadeWithNoise);

                // Final color combination with improved blending
                float4 finalColor = _MainColor;
                finalColor += fingerHighlights * (1.0 - fadeWithNoise); // Finger highlights affected by fade
                finalColor += rimLight; // Add rim light on top
                finalColor.a *= (1.0 - fadeWithNoise);

                return finalColor;
            }
            ENDHLSL
        }
    }
}
