Shader "UI/QuantumForge/MonolithOverviewCutout"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
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
            #include "UnityCG.cginc"

            struct appdata_t { float4 vertex : POSITION; float2 texcoord : TEXCOORD0; fixed4 color : COLOR; };
            struct v2f { float4 vertex : SV_POSITION; float2 texcoord : TEXCOORD0; fixed4 color : COLOR; };
            sampler2D _MainTex;
            fixed4 _Color;

            v2f vert(appdata_t input)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.texcoord = input.texcoord;
                output.color = input.color * _Color;
                return output;
            }

            fixed4 frag(v2f input) : SV_Target
            {
                fixed4 source = tex2D(_MainTex, input.texcoord) * input.color;
                float2 localUv = frac(input.texcoord * 2.0);
                float brightness = max(source.r, max(source.g, source.b));
                float keyedMatter = step(.025, brightness);
                float edge = 0.006;

                float topLeftT = saturate((localUv.y - .555) / (.925 - .555));
                float topLeftL = lerp(.255, .335, topLeftT);
                float topLeftR = lerp(.475, .455, topLeftT);
                float topLeft = smoothstep(topLeftL - edge, topLeftL + edge, localUv.x) *
                    (1.0 - smoothstep(topLeftR - edge, topLeftR + edge, localUv.x)) *
                    smoothstep(.555 - edge, .555 + edge, localUv.y) *
                    (1.0 - smoothstep(.925 - edge, .925 + edge, localUv.y));

                float topRightT = saturate((localUv.y - .545) / (.910 - .545));
                float topRightL = lerp(.535, .625, topRightT);
                float topRightR = lerp(.770, .745, topRightT);
                float topRight = smoothstep(topRightL - edge, topRightL + edge, localUv.x) *
                    (1.0 - smoothstep(topRightR - edge, topRightR + edge, localUv.x)) *
                    smoothstep(.545 - edge, .545 + edge, localUv.y) *
                    (1.0 - smoothstep(.910 - edge, .910 + edge, localUv.y));

                float bottomLeftT = saturate((localUv.y - .125) / (.570 - .125));
                float bottomLeftL = lerp(.145, .235, bottomLeftT);
                float bottomLeftR = lerp(.485, .475, bottomLeftT);
                float bottomLeft = smoothstep(bottomLeftL - edge, bottomLeftL + edge, localUv.x) *
                    (1.0 - smoothstep(bottomLeftR - edge, bottomLeftR + edge, localUv.x)) *
                    smoothstep(.125 - edge, .125 + edge, localUv.y) *
                    (1.0 - smoothstep(.570 - edge, .570 + edge, localUv.y));

                float bottomRightT = saturate((localUv.y - .125) / (.560 - .125));
                float bottomRightL = lerp(.515, .535, bottomRightT);
                float bottomRightR = lerp(.855, .775, bottomRightT);
                float bottomRight = smoothstep(bottomRightL - edge, bottomRightL + edge, localUv.x) *
                    (1.0 - smoothstep(bottomRightR - edge, bottomRightR + edge, localUv.x)) *
                    smoothstep(.125 - edge, .125 + edge, localUv.y) *
                    (1.0 - smoothstep(.560 - edge, .560 + edge, localUv.y));

                float core = smoothstep(.395 - edge, .395 + edge, localUv.x) *
                    (1.0 - smoothstep(.615 - edge, .615 + edge, localUv.x)) *
                    smoothstep(.105 - edge, .105 + edge, localUv.y) *
                    (1.0 - smoothstep(.900 - edge, .900 + edge, localUv.y));
                float spineT = saturate((localUv.y - .115) / (.900 - .115));
                float spineHalfWidth = lerp(.090, .055, spineT);
                float spine = 1.0 - smoothstep(spineHalfWidth - edge,
                    spineHalfWidth + edge, abs(localUv.x - .505));
                spine *= smoothstep(.115 - edge, .115 + edge, localUv.y) *
                    (1.0 - smoothstep(.900 - edge, .900 + edge, localUv.y));
                float base = smoothstep(.100 - edge, .100 + edge, localUv.x) *
                    (1.0 - smoothstep(.900 - edge, .900 + edge, localUv.x)) *
                    smoothstep(.040 - edge, .040 + edge, localUv.y) *
                    (1.0 - smoothstep(.190 - edge, .190 + edge, localUv.y));

                float solidPlates = max(max(topLeft, topRight),
                    max(bottomLeft, bottomRight));
                float articulatedMatter = keyedMatter * max(core, base);
                // Las placas permanecen completamente opacas. En el núcleo y la
                // base sólo se conserva materia real para que el fondo negro de la
                // hoja no forme un bloque detrás del laboratorio.
                source.a *= max(max(solidPlates, spine), articulatedMatter);
                return source;
            }
            ENDCG
        }
    }
}
