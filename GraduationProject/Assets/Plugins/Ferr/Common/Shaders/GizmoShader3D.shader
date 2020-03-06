Shader "Hidden/Ferr Gizmo Shader 3D" {
	Properties {
		_MainTex("Texture (RGBA)", 2D) = "white" {}
		_Color("Color (RGBA)", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent"  }
		Blend SrcAlpha OneMinusSrcAlpha

		LOD 200
		Lighting Off
		
		Pass {
			ZWrite Off
			ZTest  Always
		
			CGPROGRAM
			#pragma vertex   vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4    _MainTex_ST;
			float4    _Color;

			struct appdata_ferr {
			    float4 vertex   : POSITION;
			    float4 texcoord : TEXCOORD0;
			    fixed4 color    : COLOR;
			};
			struct VS_OUT {
				float4 position : SV_POSITION;
				float4 color    : COLOR;
				float2 uv       : TEXCOORD0;
			};

			VS_OUT vert (appdata_ferr input) {
				VS_OUT result;
				result.position = UnityObjectToClipPos (input.vertex);
				result.uv       = TRANSFORM_TEX (input.texcoord, _MainTex);
				result.color    = input.color * 0.6f;

				return result;
			}

			half4 frag (VS_OUT input) : COLOR {
				half4 color = tex2D(_MainTex, input.uv);
				return color * input.color * _Color;
			}
			ENDCG
		}
		Pass {
			ZWrite ON
			ZTest  Less
		
			CGPROGRAM
			#pragma vertex   vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4    _MainTex_ST;
			float4    _Color;

			struct appdata_ferr {
			    float4 vertex   : POSITION;
			    float4 texcoord : TEXCOORD0;
			    fixed4 color    : COLOR;
			};
			struct VS_OUT {
				float4 position : SV_POSITION;
				float4 color    : COLOR;
				float2 uv       : TEXCOORD0;
			};

			VS_OUT vert (appdata_ferr input) {
				VS_OUT result;
				result.position = UnityObjectToClipPos (input.vertex);
				result.uv       = TRANSFORM_TEX (input.texcoord, _MainTex);
				result.color    = input.color;

				return result;
			}

			half4 frag (VS_OUT input) : COLOR {
				half4 color = tex2D(_MainTex, input.uv);
				return color * input.color * _Color;
			}
			ENDCG
		}
	}
}
