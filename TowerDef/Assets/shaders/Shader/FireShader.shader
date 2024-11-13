Shader "Custom/FireShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Fire Color", Color) = (1, 0.5, 0, 1)
        _Speed ("Speed", Float) = 1.0
        _Intensity ("Intensity", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

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
            float _Speed;
            float _Intensity;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float wave = sin(i.uv.y * 10.0 + _Time.y * _Speed) * 0.5 + 0.5; // Flicker effect
                fixed4 tex = tex2D(_MainTex, i.uv);
                return tex * _Color * wave * _Intensity; // Combine texture color with fire color and flicker
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
