Shader "QuantumForgeLab/GalaxyFlow"
{
    Properties
    {
        _MainTex ("Galaxy RGBA", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Center ("Center", Vector) = (0.47,0.53,0,0)
        _AxisRatio ("Projected Axis Ratio", Range(0.35,1)) = 0.68
        _FlowSpeed ("Flow Speed", Range(0,1)) = 0.23
        _ArmFlow ("Arm Flow", Range(0,1)) = 0.72
        _DustFlow ("Dust Flow", Range(0,1)) = 0.36
        _CorePulse ("Core Pulse", Range(0,0.15)) = 0.055
        _LabTime ("Lab Time", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off Lighting Off ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float4 _Center;
            half _AxisRatio;
            half _FlowSpeed;
            half _ArmFlow;
            half _DustFlow;
            half _CorePulse;
            float _LabTime;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata input)
            {
                v2f output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            float2 Rotate(float2 value, float angle)
            {
                float sineValue, cosineValue;
                sincos(angle, sineValue, cosineValue);
                return float2(value.x * cosineValue - value.y * sineValue,
                              value.x * sineValue + value.y * cosineValue);
            }

            float2 FlowUv(float2 uv, float direction, float scale, float phase)
            {
                float2 plane = uv - _Center.xy;
                plane.y /= max(_AxisRatio, 0.001);
                float radius = saturate(length(plane) / 0.50);
                float inner = 1.0 - smoothstep(0.10, 0.92, radius);
                float wave = sin(_LabTime * 0.84 + radius * 8.2 + phase) * 0.012;
                float drift = _LabTime * _FlowSpeed * (0.035 + 0.050 * inner);
                float angle = direction * scale * (drift + wave * inner);
                plane = Rotate(plane, -angle);
                plane.y *= _AxisRatio;
                return saturate(plane + _Center.xy);
            }

            fixed4 frag(v2f input) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, input.uv);
                fixed4 armsA = tex2D(_MainTex, FlowUv(input.uv, 1.0, 1.0, 0.0));
                fixed4 armsB = tex2D(_MainTex, FlowUv(input.uv, 1.0, 1.0, 3.14159));
                fixed4 dustA = tex2D(_MainTex, FlowUv(input.uv, -1.0, 0.58, 1.7));

                half baseLuma = dot(baseColor.rgb, half3(0.2126, 0.7152, 0.0722));
                half armsLuma = dot(armsA.rgb, half3(0.2126, 0.7152, 0.0722));
                half dustLuma = dot(dustA.rgb, half3(0.2126, 0.7152, 0.0722));
                half armMask = smoothstep(0.24, 0.70, max(baseLuma, armsLuma)) * baseColor.a;
                half dustMask = smoothstep(0.05, 0.30, 0.38 - dustLuma) * baseColor.a;

                half3 movingArms = lerp(armsA.rgb, armsB.rgb, 0.28 + 0.22 * sin(_LabTime * 0.72));
                half3 color = baseColor.rgb * 0.84;
                color += movingArms * armMask * _ArmFlow;
                color -= max(baseColor.rgb - dustA.rgb, 0) * dustMask * _DustFlow;

                float2 core = input.uv - _Center.xy;
                core.y /= max(_AxisRatio, 0.001);
                half coreMask = 1.0 - smoothstep(0.035, 0.17, length(core));
                color *= 1.0 + coreMask * sin(_LabTime * 1.32) * _CorePulse;

                return fixed4(color, baseColor.a) * _Color;
            }
            ENDCG
        }
    }
}
