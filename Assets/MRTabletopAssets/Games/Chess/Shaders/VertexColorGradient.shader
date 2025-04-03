Shader "Slices/Vertex Color Gradient"
{
    // Handles drawing two-color gradients across UV coordinates
    Properties
    {
        _Color1 ("Color 1", Color) = (1, 1, 1, 1)
        _Color2 ("Color 2", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags { "Queue" = "Geometry-1" "RenderType"="Opaque" }
        LOD 200

        Cull Back
        ZWrite On

        CGPROGRAM
        #pragma surface surf NoLighting vertex:vert
        #pragma target 3.0

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            fixed4 c;
            c.rgb = s.Albedo;
            c.a = s.Alpha;
            return c;
        }

        struct Input
        {
            float4 vertColor;
        };

        half4 _Color1;
        half4 _Color2;

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertColor = lerp(_Color1, _Color2, v.texcoord1[0]);
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
           o.Emission = IN.vertColor.rgb;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
