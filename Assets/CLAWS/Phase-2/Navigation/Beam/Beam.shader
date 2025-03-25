Shader "Unlit/beam" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _Alpha ("Transparency", Range(0,1)) = 1  // New alpha control
    }
    SubShader {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "IgnoreProjector"="True" 
        }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _EmissionColor;
            float _Alpha;  // New alpha variable

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target {
                half4 col = tex2D(_MainTex, i.uv);
                half3 emission = col.rgb * _EmissionColor.rgb;
                return half4(emission, col.a * _Alpha);  // Modified to use _Alpha
            }
            ENDCG
        }
    }
}