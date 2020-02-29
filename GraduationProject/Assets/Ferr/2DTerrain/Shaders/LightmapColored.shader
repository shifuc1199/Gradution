Shader "Ferr/2D Terrain/Pixel Lit/Lightmap Textured Vertex Color (8 lights|lightmap +0 light)" {
	Properties {
		_MainTex ("Texture (RGB) Alpha (A)", 2D) = "white" {}
	}
	 
	SubShader {
		Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque"}
		Blend Off
		
		LOD 100
		Cull      Off
		Fog {Mode Off}
	 
		Pass {
			Tags { LightMode = Vertex } 
			CGPROGRAM
			#pragma  vertex   vert  
			#pragma  fragment frag
			#pragma  fragmentoption ARB_precision_hint_fastest
			#pragma  multi_compile_fog
			#pragma  target 3.0
			
			#define  MAX_LIGHTS 8
			
			#include "UnityCG.cginc"
			#include "Ferr2DTCommon.cginc"
			
			ENDCG
		}
		Pass {
			Tags { LightMode = VertexLMRGBM } 
			CGPROGRAM
			#pragma  vertex   vert
			#pragma  fragment frag
			#pragma  fragmentoption ARB_precision_hint_fastest
			
			#define  MAX_LIGHTS 0
			#define  FERR2DT_LIGHTMAP
			
			#include "UnityCG.cginc"
			#include "Ferr2DTCommon.cginc"
			
			ENDCG
		}
	}
	Fallback "Ferr/Lit Textured Vertex Color (4 lights|lightmap +1 light)"
}