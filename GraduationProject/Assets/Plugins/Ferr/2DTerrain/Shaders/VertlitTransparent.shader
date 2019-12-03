Shader "Ferr/2D Terrain/Vertex Lit/Textured Transparent" {
	Properties {
		_MainTex("Texture (RGB)", 2D   ) = "white" {}
		_Color  ("Tint",          Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"  }
		Blend SrcAlpha OneMinusSrcAlpha
		
		LOD 100
		Cull      Off
		ZWrite    Off
		Fog {Mode Off}
		
		Pass {
			Tags { LightMode = Vertex } 
			CGPROGRAM
			#pragma vertex         vert
			#pragma fragment       frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fog

			#define FERR2DT_TINT
			#define FERR2DT_VERTEXLIT
			
			#include "UnityCG.cginc"
			#include "Ferr2DTCommon.cginc"
			ENDCG
		}
	}
	Fallback "Unlit/Texture"
}
