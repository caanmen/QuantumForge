Shader "UI/QuantumForge/GalaxyWarp"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _ArmsTex ("Luminous Arms", 2D) = "black" {}
        _DustTex ("Dust Lanes", 2D) = "black" {}
        _CoreTex ("Compact Core", 2D) = "black" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _GalaxyCenter ("Galaxy Center", Vector) = (0.47,0.53,0,0)
        _AxisAngleDeg ("Axis Angle", Float) = 0
        _AxisRatio ("Axis Ratio", Range(0.35,1)) = 0.68
        _ArmSpeedDeg ("Arm Speed Degrees", Range(0,6)) = 2.0
        _DustSpeedDeg ("Dust Speed Degrees", Range(-3,3)) = -0.65
        _BaseVisibility ("Base Nebula Visibility", Range(0,1)) = 0.30
        _ArmIntensity ("Arm Intensity", Range(0,2)) = 1.45
        _DustIntensity ("Dust Intensity", Range(0,1)) = 0.72
        _CoreIntensity ("Core Intensity", Range(0,2)) = 1.0
        _CorePeriod ("Core Period", Range(3,8)) = 4.8
        _CoreBrightness ("Core Brightness", Range(0,0.15)) = 0.065
        _AnimTime ("Animation Time", Float) = 0

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
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
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
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "GalaxyWarpUI"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            sampler2D _ArmsTex;
            sampler2D _DustTex;
            sampler2D _CoreTex;
            fixed4 _Color;
            float4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _GalaxyCenter;
            half _AxisAngleDeg;
            half _AxisRatio;
            half _ArmSpeedDeg;
            half _DustSpeedDeg;
            half _BaseVisibility;
            half _ArmIntensity;
            half _DustIntensity;
            half _CoreIntensity;
            half _CorePeriod;
            half _CoreBrightness;
            float _AnimTime;

            v2f vert(appdata_t input)
            {
                v2f output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.worldPosition = input.vertex;
                output.vertex = UnityObjectToClipPos(output.worldPosition);
                output.texcoord = input.texcoord;
                output.color = input.color * _Color;
                return output;
            }

            float2 Rotate(float2 value, float radians)
            {
                float sineValue;
                float cosineValue;
                sincos(radians, sineValue, cosineValue);
                return float2(
                    value.x * cosineValue - value.y * sineValue,
                    value.x * sineValue + value.y * cosineValue);
            }

            float2 GalaxyPlane(float2 uv)
            {
                float axisAngle = _AxisAngleDeg * 0.01745329252;
                float2 plane = Rotate(uv - _GalaxyCenter.xy, -axisAngle);
                plane.y /= max(_AxisRatio, 0.001);
                return plane;
            }

            float2 GalaxyUv(float2 plane)
            {
                plane.y *= max(_AxisRatio, 0.001);
                float axisAngle = _AxisAngleDeg * 0.01745329252;
                return Rotate(plane, axisAngle) + _GalaxyCenter.xy;
            }

            float2 RotatedLayerUv(float2 uv, float speedDegrees)
            {
                float boundedTurns = frac(_AnimTime * speedDegrees / 360.0);
                float sampleAngle = -boundedTurns * 6.28318530718;
                return GalaxyUv(Rotate(GalaxyPlane(uv), sampleAngle));
            }

            fixed4 frag(v2f input) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, input.texcoord) + _TextureSampleAdd;
                fixed4 armsColor = tex2D(_ArmsTex, RotatedLayerUv(input.texcoord, _ArmSpeedDeg));
                fixed4 dustColor = tex2D(_DustTex, RotatedLayerUv(input.texcoord, _DustSpeedDeg));
                fixed4 coreColor = tex2D(_CoreTex, input.texcoord);

                // The dim disk and its alpha silhouette never move. Only isolated
                // arm and dust layers rotate continuously in the deprojected plane.
                // Multiplying by the fixed base alpha prevents any layer escaping.
                half armBlend = saturate(armsColor.a * _ArmIntensity) * baseColor.a;
                half dustBlend = saturate(dustColor.a * _DustIntensity) * baseColor.a;
                half3 animatedColor = baseColor.rgb * _BaseVisibility;
                animatedColor += armsColor.rgb * armBlend;
                animatedColor = lerp(animatedColor, dustColor.rgb, dustBlend);

                float2 coreDelta = input.texcoord - _GalaxyCenter.xy;
                coreDelta.y /= max(_AxisRatio, 0.001);
                half coreMask = 1.0 - smoothstep(0.035, 0.16, length(coreDelta));
                half corePulse = sin(_AnimTime * 6.2831853 / max(_CorePeriod, 0.1));
                half coreBlend = saturate(coreColor.a * _CoreIntensity) * baseColor.a;
                half coreGain = 1.0 + corePulse * _CoreBrightness;
                animatedColor = lerp(animatedColor, coreColor.rgb * coreGain, coreBlend * coreMask);

                // Let the starfield show through most of the static disk. Moving
                // arms and the compact core provide the dominant coverage.
                half visibleAlpha = saturate(
                    baseColor.a * _BaseVisibility +
                    armsColor.a * 0.92 +
                    coreColor.a * coreMask * 0.96) * baseColor.a;
                fixed4 result = fixed4(animatedColor, visibleAlpha) * input.color;
                #ifdef UNITY_UI_CLIP_RECT
                result.a *= UnityGet2DClipping(input.worldPosition.xy, _ClipRect);
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                clip(result.a - 0.001);
                #endif
                return result;
            }
            ENDCG
        }
    }
}
