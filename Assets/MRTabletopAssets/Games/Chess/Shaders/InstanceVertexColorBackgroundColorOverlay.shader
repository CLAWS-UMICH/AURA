Shader "Transmutable/Instance Vertex Color Background + Multiply Color"
{
    Properties
    {
        [PerRendererData] _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _HueShift("Hue Shift", Range(-360,360)) = 0
        _Saturation("Saturation", Range(-20,20)) = 1
        _Brightness("Brightness", Range(0,5)) = 1
    }

    SubShader
    {
        Tags { "Queue" = "Geometry" "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color: COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;

                UNITY_VERTEX_OUTPUT_STEREO
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float3 AdjustColor(float3 RGB, float3 shift)
            {
                float3 RESULT = float3(RGB);
                float VSU = shift.z*shift.y*cos(shift.x*3.14159265/180);
                float VSW = shift.z*shift.y*sin(shift.x*3.14159265/180);

                RESULT.x = (0.299*shift.z + 0.701*VSU + 0.168*VSW)*RGB.x
                        + (0.587*shift.z - 0.587*VSU + 0.330*VSW)*RGB.y
                        + (0.114*shift.z - 0.114*VSU - 0.497*VSW)*RGB.z;

                RESULT.y = (0.299*shift.z - 0.299*VSU - 0.328*VSW)*RGB.x
                        + (0.587*shift.z + 0.413*VSU + 0.035*VSW)*RGB.y
                        + (0.114*shift.z - 0.114*VSU + 0.292*VSW)*RGB.z;

                RESULT.z = (0.299*shift.z - 0.3*VSU + 1.25*VSW)*RGB.x
                        + (0.587*shift.z - 0.588*VSU - 1.05*VSW)*RGB.y
                        + (0.114*shift.z + 0.886*VSU - 0.203*VSW)*RGB.z;

                return (RESULT);
            }

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

            float _HueShift;
            float _Saturation;
            float _Brightness;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                half3 gammaAdjustedColor = i.color.rgb;
                float3 shift = half3(_HueShift, _Saturation, _Brightness);
                float3 shiftedColor = GammaToLinearSpace(half3(AdjustColor(gammaAdjustedColor, shift)));
                const float4 instanceColor = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                const float4 finalColor = float4(shiftedColor, 1) * instanceColor;
                return finalColor;
            }
            ENDCG
        }
    }
}
