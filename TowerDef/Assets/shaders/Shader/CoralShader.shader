Shader "Custom/CoralShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (1, 1, 1, 1)
        _WaveStrength ("Wave Strength", Float) = 0.1
        _WaveSpeed ("Wave Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _WaveStrength;
            float _WaveSpeed;

            v2f vert(appdata_t v)
            {
                v2f o;
                float wave = sin(v.vertex.x * 10.0 + _Time.y * _WaveSpeed) * _WaveStrength; // Simple wave effect
                v.vertex.y += wave; // Displace the vertex position
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                return tex * _Color; // Apply base color
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
