Shader "Slash/Blended"
{
	Properties
	{
		[HDR]_TintColor("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_AlphaPow("Alpha Pow", Float) = 1
		_AlphaMul("Alpha Mul", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 color : COLOR0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			float _AlphaPow;
			float _AlphaMul;

			v2f vert (appdata vertexInput)
			{
				v2f pixelInput;
				pixelInput.vertex = UnityObjectToClipPos(vertexInput.vertex);
				pixelInput.uv = TRANSFORM_TEX(vertexInput.uv, _MainTex);
				UNITY_TRANSFER_FOG(pixelInput, pixelInput.vertex);
				pixelInput.color = vertexInput.color;
				return pixelInput;
			}
			
			fixed4 frag (v2f pixelInput) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, pixelInput.uv)*_TintColor*pixelInput.color;
				col.a = saturate(pow(col.a, _AlphaPow) * _AlphaMul);
				// apply fog
				UNITY_APPLY_FOG(pixelInput.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
