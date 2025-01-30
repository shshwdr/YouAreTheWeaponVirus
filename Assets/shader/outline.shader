Shader "Custom/Outline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness ("Outline Thickness", Float) = 1.0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 200

        Pass
        {
            ZWrite Off
            Cull Off
            Lighting Off
            Blend SrcAlpha OneMinusSrcAlpha

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
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _OutlineColor;
            float _OutlineThickness;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag(v2f i) : COLOR
            {
                float4 mainTexColor = tex2D(_MainTex, i.uv);
                if (mainTexColor.a == 0)
                {
                    float outline = 0.0;
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            float2 offset = float2(x, y) * _OutlineThickness * _ScreenParams.xy / _ScreenParams.zw;
                            if (tex2D(_MainTex, i.uv + offset).a > 0.0)
                            {
                                outline = 1.0;
                                break;
                            }
                        }
                    }
                    return float4(_OutlineColor.rgb, outline * _OutlineColor.a);
                }
                return mainTexColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
