Shader "Ferr/2D Terrain/Pixel Lit/Wavy Textured Vertex Color Transparent" {
	Properties {
		_MainTex("Texture (RGBA)", 2D) = "white" {}
		_WaveSizeX("Wave Size X",  Float) = 0.25
		_WaveSizeY("Wave Size Y",  Float) = 0.25
		_WaveSpeed("Wave Speed", Float) = 4
		_PositionScale("Position Scale", Float) = 4
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent"  }
		Blend SrcAlpha OneMinusSrcAlpha

		LOD 100
		Cull      Off
		Lighting  Off
		ZWrite    Off
		Fog {Mode Off}
		
		Pass {
			Tags{ LightMode = Vertex }
			CGPROGRAM
			#pragma vertex         vert
			#pragma fragment       frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fog

			#define MAX_LIGHTS 4
			#define FERR2DT_WAVY

			#include "UnityCG.cginc"
			#include "Ferr2DTCommon.cginc"

			ENDCG
		}
	}
}
