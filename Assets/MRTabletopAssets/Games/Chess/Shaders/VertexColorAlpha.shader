Shader "Transmutable/Vertex Color - Linear + Vertex Alpha"
{
    // Handles drawing standard Transmutable vertex-color elements, with vertex alpha support
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert alpha vertex:vert
        #pragma target 3.0

        struct Input
        {
            float4 vertColor;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertColor = v.color;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Emission = GammaToLinearSpace(IN.vertColor.rgb);
            o.Alpha = IN.vertColor.a;
        }

        ENDCG
    }

    FallBack "Diffuse"
}
