Shader "Ferr/2D Terrain/Pixel Lit/Lightmap Textured Vertex Color Transparent (8 lights|lightmap +0 light)" {
	Properties {
		_MainTex ("Texture (RGB) Alpha (A)", 2D) = "white" {}
	}
	 
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend  SrcAlpha OneMinusSrcAlpha
		
		LOD 100
		ZWrite    Off
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
			#pragma  multi_compile_fog
			
			#define  MAX_LIGHTS 0
			#define  FERR2DT_LIGHTMAP
			
			#include "UnityCG.cginc"
			#include "Ferr2DTCommon.cginc"
			
			ENDCG
		}
	}
	Fallback "Ferr/Lit Textured Vertex Color Transparent (4 lights|lightmap +1 light)"
}