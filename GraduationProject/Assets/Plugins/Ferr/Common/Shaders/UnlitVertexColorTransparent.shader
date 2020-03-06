Shader "Ferr/Common/Unlit Vertex Color Transparent" {
	Properties {
		_MainTex("Texture (RGBA)", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent"  }
		Blend SrcAlpha OneMinusSrcAlpha

		LOD 200
		Lighting Off
		
		Pass {
			CGPROGRAM
			#pragma multi_compile USE_TEX NO_TEX
			#pragma vertex   vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			#if USE_TEX
			sampler2D _MainTex;
			float4    _MainTex_ST;
			#endif
			
			struct appdata_ferr {
			    float4 position : POSITION;
			    fixed4 color    : COLOR;
			    #if USE_TEX
			    float2 uv       : TEXCOORD0;
			    #endif
			};
			struct VS_OUT {
				float4 position : SV_POSITION;
				fixed4 color    : COLOR;
				#if USE_TEX
			    float2 uv       : TEXCOORD0;
			    #endif
			};

			VS_OUT vert (appdata_ferr input) {
				VS_OUT result;
				result.position = UnityObjectToClipPos (input.position);
				result.color    = input.color;
				#if USE_TEX
				result.uv       = TRANSFORM_TEX (input.uv, _MainTex);
				#endif
				
				return result;
			}

			fixed4 frag (VS_OUT input) : COLOR {
				#if USE_TEX
				return tex2D(_MainTex, input.uv) * input.color;
				#else
				return input.color;
				#endif
				
			}
			ENDCG
		}
	}
	CustomEditor "Ferr.UnlitVertexColorEditor"
}
