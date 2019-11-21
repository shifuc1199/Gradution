Shader "Slash/BlendDistortionCutout"
{
	Properties
	{
		[HDR]_TintColor("TintColor", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		[Toggle(USE_ALPHA_CUTOUT)] _UseAlphaCutout ("Use Alpha Cutout from PS", int) = 0
			_CutoutTex("Cutout Texture", 2D) = "white" {}
		_DistTex("Distortion Texture", 2D) = "white" {}
		_DistStrength("Distortion Strength", Vector) = (1,1,1,1)
			_SoftFade("Soft Fade", Float) = 3
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
#pragma multi_compile_particles
			#pragma multi_compile_fog
			#pragma shader_feature USE_ALPHA_CUTOUT
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				float4 color : COLOR0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 cutoutUV : TEXCOORD1;
				float2 rotation : TEXCOORD2;
				float4 color : COLOR0;
				UNITY_FOG_COORDS(3)
					float2 uvDistort : TEXCOORD4;
				float4 screenPos : TEXCOORD5;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _CutoutTex;
			sampler2D _DistTex;
			float4 _MainTex_ST;
			float4 _CutoutTex_ST;
			float4 _DistTex_ST;
			float4 _TintColor;
			float4 _DistStrength;
			float _SoftFade;
			sampler2D _CameraDepthTexture;

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv.xy, _MainTex);
				o.cutoutUV = TRANSFORM_TEX(v.uv.xy, _CutoutTex);
				o.uvDistort = TRANSFORM_TEX(v.uv.xy, _DistTex);
				o.color = v.color;
				o.rotation = v.uv.zw;

				o.screenPos = ComputeScreenPos(o.vertex);
				COMPUTE_EYEDEPTH(o.screenPos.z);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half fade = 1;
#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos))));
				float partZ = i.screenPos.z;
				fade = saturate(_SoftFade * (sceneZ - partZ));
				fade = _SoftFade > 0.01 ? fade : 1;
#endif
				half2 distort = tex2D(_DistTex, i.uvDistort + _Time.x * _DistStrength.zw) * _DistStrength.xy;

				fixed4 col = tex2D(_MainTex, i.uv + i.rotation + distort);
				col = 2 * col * _TintColor * i.color;

#if USE_ALPHA_CUTOUT
				half cut = tex2D(_CutoutTex, i.cutoutUV - i.rotation);
				col.a = saturate(col.a * 4) *  step(cut - i.color.a, col.a);
#endif

				
				UNITY_APPLY_FOG(i.fogCoord, col);
				col.a = saturate(col.a * fade);
				return col;
			}
			ENDCG
		}
	}
}
