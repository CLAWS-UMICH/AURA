Shader "Transmutable/Vertex Color Background Fragment + Alpha + Stencil Read"
{
    Properties
    {
        _Opacity ("Opacity", Range(0,1)) = 1.0
        _StencilID("Stencil Read ID", Range(0, 255)) = 255
    }

    SubShader
    {
        Pass{
                Tags { "Queue" = "Geometry-2" "RenderType"="Transparent" }
                LOD 200

                Cull Off
                ZWrite On
                Blend SrcAlpha OneMinusSrcAlpha

                Stencil
                {
                    Ref [_StencilID]
                    Comp Equal
                }

                CGPROGRAM
                #include "UnityCG.cginc"

                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_instancing
                #pragma target 3.0

                struct appdata
                {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float4 color : COLOR;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float3 vertColor : TEXCOORD2;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                UNITY_INSTANCING_BUFFER_START(Props)
                    UNITY_DEFINE_INSTANCED_PROP(float, _Opacity)
                UNITY_INSTANCING_BUFFER_END(Props)

                v2f vert(appdata v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                    float opacity = UNITY_ACCESS_INSTANCED_PROP(Props, _Opacity);
                    o.vertColor = v.color.rgb;
                    o.vertex = UnityObjectToClipPos(v.vertex + ((1 - opacity) * -v.normal * 1));
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                    float opacity = UNITY_ACCESS_INSTANCED_PROP(Props, _Opacity);
                    half3 gammaAdjustedColor = GammaToLinearSpace(i.vertColor.rgb);
                    return half4(gammaAdjustedColor.r, gammaAdjustedColor.g, gammaAdjustedColor.b, opacity);
                }
                ENDCG
            }
    }

    FallBack "Diffuse"
}
