Shader "UI/QuantumForge/MonolithFittedSupport"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _FootprintTex ("Monolith Footprint", 2D) = "black" {}
        _SupportTex ("Approved Support Atlas", 2D) = "black" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Layer ("Layer", Range(0,1)) = 0
        _FootprintStage ("Footprint Stage", Range(0,3)) = 0
        [HideInInspector] _ClipRect ("Clip Rect", Vector) = (-32767,-32767,32767,32767)
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
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
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
                float2 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _FootprintTex;
            sampler2D _SupportTex;
            fixed4 _Color;
            float _Layer;
            float _FootprintStage;
            float4 _ClipRect;

            v2f vert(appdata_t input)
            {
                v2f output;
                output.worldPosition = input.vertex.xy;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.texcoord = input.texcoord;
                output.color = input.color * _Color;
                return output;
            }

            float Between(float value, float minimum, float maximum, float softness)
            {
                return smoothstep(minimum - softness, minimum + softness, value) *
                    (1.0 - smoothstep(maximum - softness, maximum + softness, value));
            }

            float OverviewMatter(float2 localUv)
            {
                if (localUv.x < 0.0 || localUv.x > 1.0 ||
                    localUv.y < 0.0 || localUv.y > 1.0)
                    return 0.0;

                float stage = floor(_FootprintStage + .5);
                float column = fmod(stage, 2.0);
                float topRow = 1.0 - step(1.5, stage);
                float2 sheetUv = float2((column + localUv.x) * .5,
                    (topRow + localUv.y) * .5);
                fixed4 source = tex2D(_FootprintTex, sheetUv);
                float brightness = max(source.r, max(source.g, source.b));
                float keyedMatter = step(.025, brightness);
                float edge = .006;

                float topLeftT = saturate((localUv.y - .555) / (.925 - .555));
                float topLeftL = lerp(.255, .335, topLeftT);
                float topLeftR = lerp(.475, .455, topLeftT);
                float topLeft = smoothstep(topLeftL - edge, topLeftL + edge, localUv.x) *
                    (1.0 - smoothstep(topLeftR - edge, topLeftR + edge, localUv.x)) *
                    Between(localUv.y, .555, .925, edge);

                float topRightT = saturate((localUv.y - .545) / (.910 - .545));
                float topRightL = lerp(.535, .625, topRightT);
                float topRightR = lerp(.770, .745, topRightT);
                float topRight = smoothstep(topRightL - edge, topRightL + edge, localUv.x) *
                    (1.0 - smoothstep(topRightR - edge, topRightR + edge, localUv.x)) *
                    Between(localUv.y, .545, .910, edge);

                float bottomLeftT = saturate((localUv.y - .125) / (.570 - .125));
                float bottomLeftL = lerp(.145, .235, bottomLeftT);
                float bottomLeftR = lerp(.485, .475, bottomLeftT);
                float bottomLeft = smoothstep(bottomLeftL - edge, bottomLeftL + edge, localUv.x) *
                    (1.0 - smoothstep(bottomLeftR - edge, bottomLeftR + edge, localUv.x)) *
                    Between(localUv.y, .125, .570, edge);

                float bottomRightT = saturate((localUv.y - .125) / (.560 - .125));
                float bottomRightL = lerp(.515, .535, bottomRightT);
                float bottomRightR = lerp(.855, .775, bottomRightT);
                float bottomRight = smoothstep(bottomRightL - edge, bottomRightL + edge, localUv.x) *
                    (1.0 - smoothstep(bottomRightR - edge, bottomRightR + edge, localUv.x)) *
                    Between(localUv.y, .125, .560, edge);

                float core = Between(localUv.x, .395, .615, edge) *
                    Between(localUv.y, .105, .900, edge);
                float spineT = saturate((localUv.y - .115) / (.900 - .115));
                float spineHalfWidth = lerp(.090, .055, spineT);
                float spine = 1.0 - smoothstep(spineHalfWidth - edge,
                    spineHalfWidth + edge, abs(localUv.x - .505));
                spine *= Between(localUv.y, .115, .900, edge);
                float base = Between(localUv.x, .100, .900, edge) *
                    Between(localUv.y, .040, .190, edge);

                float solidPlates = max(max(topLeft, topRight),
                    max(bottomLeft, bottomRight));
                float articulatedMatter = keyedMatter * max(core, base);
                return saturate(max(max(solidPlates, spine), articulatedMatter));
            }

            fixed4 frag(v2f input) : SV_Target
            {
                float2 uv = input.texcoord;
                float stage = floor(_FootprintStage + .5);
                float column = fmod(stage, 2.0);
                float topRow = 1.0 - step(1.5, stage);
                float2 sheetUv = float2((column + uv.x) * .5,
                    (topRow + uv.y) * .5);
                fixed4 color = tex2D(_SupportTex, sheetUv) * input.color;
                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(input.worldPosition, _ClipRect);
                #endif
                return color;
            }
            ENDCG
        }
    }
}
