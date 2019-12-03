Shader "Ferr/Common/Unlit Solid Color" {
	Properties {
		_Color("Color (RGB)", Color) = (1,1,1,1)

	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent"  }
		Blend SrcAlpha OneMinusSrcAlpha

		LOD 200
		Lighting Off
		
		Pass {
			CGPROGRAM
			#pragma vertex   vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float4 _Color;

			struct appdata_ferr {
			    float4 vertex   : POSITION;
			};
			struct VS_OUT {
				float4 position : SV_POSITION;
			};

			VS_OUT vert (appdata_ferr input) {
				VS_OUT result;
				result.position = UnityObjectToClipPos (input.vertex);
				return result;
			}

			half4 frag (VS_OUT input) : COLOR {
				return _Color;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
