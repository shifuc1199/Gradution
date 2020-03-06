Shader "Ferr/2D Terrain/Unlit/Textured Vertex Color" {
	Properties {
		_MainTex("Texture (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque"}
		Blend Off
		
		LOD 100
		Cull      Off
		Lighting  Off
		Fog {Mode Off}
		
		Pass {
			CGPROGRAM
			#pragma vertex         vert
			#pragma fragment       frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			#include "Ferr2DTCommon.cginc"
			ENDCG
		}
	}
	Fallback "Unlit/Texture"
}
