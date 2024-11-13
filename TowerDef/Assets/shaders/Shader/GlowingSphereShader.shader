Shader "Custom/GlowingSphereShader"
{
    Properties
    {
        _Color ("Glow Color", Color) = (1, 1, 1, 1)
        _GlowIntensity ("Glow Intensity", Range(0, 10)) = 1.0
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
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
                float3 worldPos : TEXCOORD1; // Pass the world position for fading
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _GlowIntensity;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = v.vertex.xyz; // Pass vertex position
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Calculate distance from center to fade out color
                float dist = length(i.worldPos);
                float fade = saturate(1.0 - dist / _GlowIntensity); // Fade based on distance

                fixed4 tex = tex2D(_MainTex, i.uv);
                return tex * _Color * fade; // Apply fade to the glow color
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
