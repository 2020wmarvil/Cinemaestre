Shader "Hidden/ScreenFadeEffect" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _FadeProgress ("Fade Progress", float) = 0
    }
    SubShader {
        Cull Off ZWrite Off ZTest Always

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct VertIn {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VertOut {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            VertOut vert (VertIn v) {
                VertOut o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _FadeProgress;

            fixed4 frag (VertOut i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);

                col.rgb *= 1 - _FadeProgress;

                return col;
            }
            ENDCG
        }
    }
}
