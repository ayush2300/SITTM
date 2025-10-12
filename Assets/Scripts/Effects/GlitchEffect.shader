Shader "Custom/GlitchEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Intensity", Range(0,1)) = 0.2
        _ColorSplit ("Color Split", Range(0,1)) = 0.05
        _NoiseStrength ("Noise Strength", Range(0,1)) = 0.3
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            sampler2D _MainTex;
            float _Intensity;
            float _ColorSplit;
            float _NoiseStrength;
            float _TimeFactor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float random(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898,78.233))) * 43758.5453);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // RGB split glitch
                float rOffset = sin(_TimeFactor * 20.0) * _ColorSplit;
                float gOffset = cos(_TimeFactor * 15.0) * _ColorSplit;
                float bOffset = sin(_TimeFactor * 25.0) * _ColorSplit * -1.0;

                float3 col;
                col.r = tex2D(_MainTex, uv + float2(rOffset, 0)).r;
                col.g = tex2D(_MainTex, uv + float2(gOffset, 0)).g;
                col.b = tex2D(_MainTex, uv + float2(bOffset, 0)).b;

                // Noise overlay
                float noise = random(uv * _TimeFactor) * _NoiseStrength;
                col += noise;

                // Slight screen distortion
                uv.y += sin(uv.x * 40.0 + _TimeFactor * 10.0) * _Intensity * 0.02;
                uv.x += cos(uv.y * 60.0 + _TimeFactor * 8.0) * _Intensity * 0.02;

                return float4(col, 1);
            }
            ENDCG
        }
    }
}
