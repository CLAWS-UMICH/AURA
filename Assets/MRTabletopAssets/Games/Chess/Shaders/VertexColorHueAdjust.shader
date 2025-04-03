Shader "Transmutable/Vertex Color + Hue Adjustment"
{
    Properties
    {
        _HueShift("Hue Shift", Range(-360,360)) = 0
        _Saturation("Saturation", Range(-20,20)) = 1
        _Brightness("Brightness", Range(0,5)) = 1
    }

    SubShader
    {
        Tags {"RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float _HueShift;
                float _Saturation;
                float _Brightness;
            CBUFFER_END

            float3 AdjustColor(float3 RGB, float3 shift)
            {
                float3 RESULT = RGB;
                float VSU = shift.z * shift.y * cos(shift.x * 3.14159265 / 180);
                float VSW = shift.z * shift.y * sin(shift.x * 3.14159265 / 180);

                RESULT.x = (0.299 * shift.z + 0.701 * VSU + 0.168 * VSW) * RGB.x
                         + (0.587 * shift.z - 0.587 * VSU + 0.330 * VSW) * RGB.y
                         + (0.114 * shift.z - 0.114 * VSU - 0.497 * VSW) * RGB.z;

                RESULT.y = (0.299 * shift.z - 0.299 * VSU - 0.328 * VSW) * RGB.x
                         + (0.587 * shift.z + 0.413 * VSU + 0.035 * VSW) * RGB.y
                         + (0.114 * shift.z - 0.114 * VSU + 0.292 * VSW) * RGB.z;

                RESULT.z = (0.299 * shift.z - 0.3 * VSU + 1.25 * VSW) * RGB.x
                         + (0.587 * shift.z - 0.588 * VSU - 1.05 * VSW) * RGB.y
                         + (0.114 * shift.z + 0.886 * VSU - 0.203 * VSW) * RGB.z;

                return (RESULT);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.color = input.color;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Assuming input color is already in linear space (typical for modern rendering)
                half3 linearColor = input.color.rgb;

                // Convert to gamma space for color adjustments
                half3 gammaColor = pow(linearColor, 1.0 / 2.2);

                float3 shift = float3(_HueShift, _Saturation, _Brightness);
                half3 adjustedColor = AdjustColor(gammaColor, shift);

                // Convert back to linear space for output
                half3 adjustedLinearColor = pow(adjustedColor, 2.2);

                return half4(adjustedLinearColor, 1);
            }

            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
