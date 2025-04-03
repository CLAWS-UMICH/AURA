Shader "TextMeshPro/Distance Field Sprite" {

Properties {
    _FaceTex            ("Face Texture", 2D) = "white" {}
    _FaceUVSpeedX       ("Face UV Speed X", Range(-5, 5)) = 0.0
    _FaceUVSpeedY       ("Face UV Speed Y", Range(-5, 5)) = 0.0
    _FaceColor          ("Face Color", Color) = (1,1,1,1)
    _FaceDilate         ("Face Dilate", Range(-1,1)) = 0

    [HDR]_OutlineColor  ("Outline Color", Color) = (0,0,0,1)
    _OutlineTex         ("Outline Texture", 2D) = "white" {}
    _OutlineUVSpeedX    ("Outline UV Speed X", Range(-5, 5)) = 0.0
    _OutlineUVSpeedY    ("Outline UV Speed Y", Range(-5, 5)) = 0.0
    _OutlineWidth       ("Outline Thickness", Range(0, 1)) = 0
    _OutlineSoftness    ("Outline Softness", Range(0,1)) = 0

    _WeightNormal       ("Weight Normal", float) = 0
    _WeightBold         ("Weight Bold", float) = 0.5

    _ScaleRatioA        ("Scale RatioA", float) = 1

    _MainTex            ("Font Atlas", 2D) = "white" {}
    _TextureWidth       ("Texture Width", float) = 512
    _TextureHeight      ("Texture Height", float) = 512
    _GradientScale      ("Gradient Scale", float) = 5.0
    _ScaleX             ("Scale X", float) = 1.0
    _ScaleY             ("Scale Y", float) = 1.0
    _PerspectiveFilter  ("Perspective Correction", Range(0, 1)) = 0.875
    _Sharpness          ("Sharpness", Range(-1,1)) = 0

    _VertexOffsetX      ("Vertex OffsetX", float) = 0
    _VertexOffsetY      ("Vertex OffsetY", float) = 0

    _CullMode           ("Cull Mode", Float) = 0
    _ColorMask          ("Color Mask", Float) = 15
}

SubShader {

    Tags
    {
        "Queue"="Transparent"
        "IgnoreProjector"="True"
        "RenderType"="Transparent"
    }

    Stencil
    {
        Ref [_Stencil]
        Comp [_StencilComp]
        Pass [_StencilOp]
        ReadMask [_StencilReadMask]
        WriteMask [_StencilWriteMask]
    }

    Cull [_CullMode]
    ZWrite Off
    Lighting Off
    Fog { Mode Off }
    ZTest [unity_GUIZTestMode]
    Blend One OneMinusSrcAlpha
    ColorMask [_ColorMask]

    Pass {
        CGPROGRAM
        #pragma target 3.0
        #pragma vertex VertShader
        #pragma fragment PixShader

        #include "UnityCG.cginc"
        #pragma multi_compile_instancing

        struct vertex_t {
            UNITY_VERTEX_INPUT_INSTANCE_ID
            float4  position        : POSITION;
            float3  normal          : NORMAL;
            fixed4  color           : COLOR;
            float2  texcoord0       : TEXCOORD0;
            float2  texcoord1       : TEXCOORD1;
        };


        struct pixel_t {
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
            float4  position        : SV_POSITION;
            fixed4  color           : COLOR;
            float2  atlas           : TEXCOORD0;        // Atlas
            float4  param           : TEXCOORD1;        // alphaClip, scale, bias, weight
            float4  mask            : TEXCOORD2;        // Position in object space(xy), pixel Size(zw)
            float3  viewDir         : TEXCOORD3;

        #if (UNDERLAY_ON || UNDERLAY_INNER)
            float4  texcoord2       : TEXCOORD4;        // u,v, scale, bias
            fixed4  underlayColor   : COLOR1;
        #endif
            float4 textures         : TEXCOORD5;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
           UNITY_DEFINE_INSTANCED_PROP(float4, _InstancedUVOffsets)
           UNITY_DEFINE_INSTANCED_PROP(float4, _InstancedFaceColor)
        UNITY_INSTANCING_BUFFER_END(Props)

        // Used by Unity internally to handle Texture Tiling and Offset.
        float4 _FaceTex_ST;
        float4 _OutlineTex_ST;

        // API Editable properties
        uniform float       _WeightNormal;
        uniform float       _WeightBold;

        uniform float       _ScaleRatioA;

        uniform float       _VertexOffsetX;
        uniform float       _VertexOffsetY;

        uniform float       _ScaleX;
        uniform float       _ScaleY;

        // Font Atlas properties
        uniform sampler2D   _MainTex;
        uniform float       _GradientScale;
        uniform float       _PerspectiveFilter;
        uniform float       _Sharpness;

        uniform sampler2D   _FaceTex;                   // Alpha : Signed Distance
        uniform float       _FaceUVSpeedX;
        uniform float       _FaceUVSpeedY;
        uniform fixed4      _FaceColor;                 // RGBA : Color + Opacity
        uniform float       _FaceDilate;                // v[ 0, 1]
        uniform float       _OutlineSoftness;           // v[ 0, 1]

        uniform sampler2D   _OutlineTex;                // RGBA : Color + Opacity
        uniform float       _OutlineUVSpeedX;
        uniform float       _OutlineUVSpeedY;
        uniform fixed4      _OutlineColor;              // RGBA : Color + Opacity
        uniform float       _OutlineWidth;              // v[ 0, 1]

        float2 UnpackUV(float uv)
        {
            float2 output;
            output.x = floor(uv / 4096);
            output.y = uv - 4096 * output.x;

            return output * 0.001953125;
        }

        fixed4 GetColor(half d, fixed4 faceColor, fixed4 outlineColor, half outline, half softness)
        {
            half faceAlpha = 1-saturate((d - outline * 0.5 + softness * 0.5) / (1.0 + softness));
            half outlineAlpha = saturate((d + outline * 0.5)) * sqrt(min(1.0, outline));

            faceColor.rgb *= faceColor.a;
            outlineColor.rgb *= outlineColor.a;

            faceColor = lerp(faceColor, outlineColor, outlineAlpha);

            faceColor *= faceAlpha;

            return faceColor;
        }

        float2 ApplyInstancedUVOffsets(float2 uv, float4 offsets)
        {
            uv.x *= offsets.z;
            uv.y *= offsets.w;
            uv.x += offsets.x;
            uv.y += offsets.y;
            return uv;
        }

        pixel_t VertShader(vertex_t input)
        {
            pixel_t output;

            UNITY_INITIALIZE_OUTPUT(pixel_t, output);
            UNITY_SETUP_INSTANCE_ID(input);
            UNITY_TRANSFER_INSTANCE_ID(input,output);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

            float bold = step(input.texcoord1.y, 0);

            float4 vert = input.position;
            vert.x += _VertexOffsetX;
            vert.y += _VertexOffsetY;

            float4 vPosition = UnityObjectToClipPos(vert);

            float2 pixelSize = vPosition.w;
            pixelSize /= float2(_ScaleX, _ScaleY) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
            float scale = rsqrt(dot(pixelSize, pixelSize));
            scale *= abs(input.texcoord1.y) * _GradientScale * (_Sharpness + 1);
            if (UNITY_MATRIX_P[3][3] == 0) scale = lerp(abs(scale) * (1 - _PerspectiveFilter), scale, abs(dot(UnityObjectToWorldNormal(input.normal.xyz), normalize(WorldSpaceViewDir(vert)))));

            float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
            weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;

            float bias =(.5 - weight) + (.5 / scale);

            float alphaClip = (1.0 - _OutlineWidth * _ScaleRatioA - _OutlineSoftness * _ScaleRatioA);

            // Support for texture tiling and offset
            float2 textureUV = UnpackUV(input.texcoord1.x);
            float2 faceUV = TRANSFORM_TEX(textureUV, _FaceTex);
            float2 outlineUV = TRANSFORM_TEX(textureUV, _OutlineTex);

            output.position = vPosition;
            output.color = input.color;
            output.atlas = ApplyInstancedUVOffsets(input.texcoord0, UNITY_ACCESS_INSTANCED_PROP(Props, _InstancedUVOffsets));
            output.param =  float4(alphaClip, scale, bias, weight);
            output.textures = float4(faceUV, outlineUV);

            return output;
        }

        fixed4 PixShader(pixel_t input) : SV_Target
        {
            UNITY_SETUP_INSTANCE_ID(input);

            float c = tex2D(_MainTex, input.atlas).a;

            float   scale   = input.param.y;
            float   bias    = input.param.z;
            float   sd = (bias - c) * scale;

            float outline = (_OutlineWidth * _ScaleRatioA) * scale;
            float softness = (_OutlineSoftness * _ScaleRatioA) * scale;

            half4 faceColor = UNITY_ACCESS_INSTANCED_PROP(Props, _InstancedFaceColor);
            half4 outlineColor = _OutlineColor;

            faceColor.rgb *= input.color.rgb;

            faceColor *= tex2D(_FaceTex, input.textures.xy + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y);
            outlineColor *= tex2D(_OutlineTex, input.textures.zw + float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.y);

            faceColor = GetColor(sd, faceColor, outlineColor, outline, softness);

            return faceColor * input.color.a;
        }

        ENDCG
    }
}

//CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
