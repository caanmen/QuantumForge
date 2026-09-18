Shader "UI/QuantumForge/MonolithBlackKey"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _SectorIndex ("Sector Index", Float) = 0
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
            float _SectorIndex;

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
                fixed4 sampleColor = tex2D(_MainTex, input.texcoord) * input.color;
                float brightness = max(sampleColor.r, max(sampleColor.g, sampleColor.b));
                // Clasificación binaria: lo que supera el umbral es totalmente
                // opaco. Nunca se usa el brillo como transparencia gradual.
                float keyedAlpha = step(.006, brightness);
                float2 uv = frac(input.texcoord * 2.0);
                float edge = .006;
                float bottomL = .250;
                float bottomR = .790;
                float topL = .405;
                float topR = .735;
                float topYLeft = .910;
                float topYRight = .965;
                float sideMin = .80;
                float sideMax = .96;

                if (_SectorIndex > .5 && _SectorIndex < 1.5)
                {
                    bottomL = .205; bottomR = .865;
                    topL = .235; topR = .595;
                    topYLeft = .925; topYRight = .885;
                    sideMin = .035; sideMax = .205;
                }
                else if (_SectorIndex > 1.5 && _SectorIndex < 2.5)
                {
                    bottomL = .135; bottomR = .875;
                    topL = .190; topR = .720;
                    topYLeft = .945; topYRight = .825;
                    sideMin = .05; sideMax = .17;
                }
                else if (_SectorIndex > 2.5)
                {
                    bottomL = .025; bottomR = .875;
                    topL = .380; topR = .705;
                    topYLeft = .900; topYRight = .940;
                    sideMin = .79; sideMax = .96;
                }
                else
                {
                    bottomL = .035; bottomR = .805;
                    topL = .215; topR = .765;
                }

                float topY = max(topYLeft, topYRight);
                float t = saturate((uv.y - .055) / (topY - .055));
                float left = lerp(bottomL, topL, t);
                float right = lerp(bottomR, topR, t);
                float topSpan = max(.001, topR - topL);
                float topT = saturate((uv.x - topL) / topSpan);
                float topLimit = lerp(topYLeft, topYRight, topT);
                float face = smoothstep(left - edge, left + edge, uv.x) *
                    (1.0 - smoothstep(right - edge, right + edge, uv.x)) *
                    smoothstep(.055 - edge, .055 + edge, uv.y) *
                    (1.0 - smoothstep(topLimit - edge, topLimit + edge, uv.y));
                float innerFace = smoothstep(left + .050 - edge,
                    left + .050 + edge, uv.x) *
                    (1.0 - smoothstep(right - .050 - edge,
                    right - .050 + edge, uv.x)) *
                    smoothstep(.095 - edge, .095 + edge, uv.y) *
                    (1.0 - smoothstep(topLimit - .045 - edge,
                    topLimit - .045 + edge, uv.y));
                float baseRegion = smoothstep(.02 - edge, .02 + edge, uv.x) *
                    (1.0 - smoothstep(.98 - edge, .98 + edge, uv.x)) *
                    smoothstep(.025, .045, uv.y) *
                    (1.0 - smoothstep(.215, .235, uv.y));
                float sideAssembly = smoothstep(sideMin - edge, sideMin + edge, uv.x) *
                    (1.0 - smoothstep(sideMax - edge, sideMax + edge, uv.x)) *
                    smoothstep(.04 - edge, .04 + edge, uv.y) *
                    (1.0 - smoothstep(.98 - edge, .98 + edge, uv.y));

                // La cara utiliza una silueta física completamente opaca. No se
                // deriva la transparencia del brillo del PNG: así desaparecen los
                // huecos, motas y tuberías flotantes causados por el fondo oscuro.
                float artifact = face;
                sampleColor.a *= artifact;
                return sampleColor;
            }
            ENDCG
        }
    }
}
