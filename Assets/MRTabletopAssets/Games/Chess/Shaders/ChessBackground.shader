Shader "MR Tabletop/Chess Background"
{
    Properties
    {
        _Opacity ("Opacity", Range(0,1)) = 1.0
        _StencilID("Stencil Read ID", Range(0, 255)) = 255
    }

    SubShader
    {
        Tags { "Queue" = "Geometry-2" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
        LOD 200

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        ENDHLSL

        Pass
        {
            Name "VertexColorPass"

            Cull Off
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            Stencil
            {
                Ref [_StencilID]
                Comp Equal
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma target 3.0

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 vertColor : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                half _Opacity;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.vertColor = input.color.rgb;
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz + ((1 - _Opacity) * -input.normalOS * 1));
                output.positionCS = TransformWorldToHClip(positionWS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Perform gamma to linear conversion
                half3 linearColor = pow(abs(input.vertColor), 2.2);
                return half4(linearColor, _Opacity);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
