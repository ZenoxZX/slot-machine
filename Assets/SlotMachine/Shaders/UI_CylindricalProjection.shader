Shader "SlotMachine/UI/CylindricalProjection"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)

        _PerspectiveAmount ("Perspective Amount", Range(0, 0.5)) = 0.15
        _MaskCenterY ("Mask Center Y", Float) = 0
        _MaskHalfHeight ("Mask Half Height", Float) = 225

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                half4 mask : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                half4 _TextureSampleAdd;
                float4 _ClipRect;
                half _PerspectiveAmount;
                float _MaskCenterY;
                float _MaskHalfHeight;
                half _UIMaskSoftnessX;
                half _UIMaskSoftnessY;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float4 vPos = v.vertex;

                float normalizedY = (_MaskCenterY - vPos.y) / max(_MaskHalfHeight, 0.001);
                normalizedY = clamp(normalizedY, -1.0, 1.0);

                float reelSide = v.uv1.y;
                float distFromCenter = abs(normalizedY);
                float squeeze = 1.0 - _PerspectiveAmount * distFromCenter * distFromCenter;

                float vertexSide = sign(vPos.x);
                float asymmetry = vertexSide * reelSide;
                float asymSqueeze = squeeze - _PerspectiveAmount * 0.3 * asymmetry * distFromCenter;

                vPos.x *= asymSqueeze;

                o.worldPosition = vPos;
                o.vertex = TransformObjectToHClip(vPos.xyz);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;

                float2 pixelSize = o.vertex.w;
                pixelSize /= abs(float2(_ScreenParams.x * UNITY_MATRIX_P[0][0],
                                        _ScreenParams.y * UNITY_MATRIX_P[1][1]));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);

                o.mask = half4(
                    vPos.xy * 2 - clampedRect.xy - clampedRect.zw,
                    0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy))
                );

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 color = (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) + _TextureSampleAdd) * i.color;

                #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(i.mask.xy)) * i.mask.zw);
                color.a *= m.x * m.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                color.rgb *= color.a;

                return color;
            }
            ENDHLSL
        }
    }
}
