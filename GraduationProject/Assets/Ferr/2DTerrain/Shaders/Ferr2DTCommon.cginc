#define LIGHT_SCALE 4

#if defined(FERR2DT_TINT)
float4 _Color;
#endif

#if defined(FERR2DT_WAVY)
float _WaveSizeX;
float _WaveSizeY;
float _WaveSpeed;
float _PositionScale;
#endif

sampler2D _MainTex;
half4     _MainTex_ST;

struct appdata_ferr {
	float4 vertex     : POSITION;
	float2 texcoord0  : TEXCOORD0;
	#ifdef FERR2DT_LIGHTMAP
	half2  lightcoord : TEXCOORD1;
	#endif
	fixed4 color      : COLOR;
};
struct VS_OUT {
	float4 position: SV_POSITION;
	float2 uv      : TEXCOORD0;
	#ifdef FERR2DT_LIGHTMAP
	half2  lightuv : TEXCOORD1;
	#endif
	#if MAX_LIGHTS > 0
	half3  viewpos : TEXCOORD2;
	#endif
	#ifdef FERR2DT_VERTEXLIT
	float3 light   : TEXCOORD3;
	#endif
	UNITY_FOG_COORDS(4)
	fixed4 color   : COLOR;
};

float3 GetLight(int i, float3 aViewPos) {
	half3  toLight = unity_LightPosition[i].xyz - aViewPos * unity_LightPosition[i].w;
	half   distSq  = dot(toLight, toLight);
	half   atten   = 1.0 / ((distSq * unity_LightAtten[i].z) + 1.0);

	// this prevents areas outside the radius from getting lit, with a bit of a gradient to prevent it from being harsh
	float cutoff = saturate((unity_LightAtten[i].w - distSq) / unity_LightAtten[i].w);

	return unity_LightColor[i].rgb * atten * cutoff;
}

VS_OUT vert(appdata_ferr input) {
	VS_OUT result;

	#if defined(FERR2DT_WAVY)
	float4 world      = mul(unity_ObjectToWorld, input.vertex);
	float  waveOffset = (world.x + world.y + world.z) / _PositionScale;
	float  wave       = (_Time.z + waveOffset) * _WaveSpeed;
	result.position   = UnityObjectToClipPos(input.vertex + float4(cos(wave) * _WaveSizeX, sin(wave) * _WaveSizeY, 0, 0));
	#else
	result.position   = UnityObjectToClipPos(input.vertex);
	#endif
	
	#if defined(FERR2DT_VERTEXLIGHTBAKED)
	result.color = fixed4(0,0,0,1);
	#else
	result.color = input.color;
	#endif
	
	#if defined(FERR2DT_VERTEXLIT)
	float3 viewPos = UnityObjectToViewPos(input.vertex).xyz;
	float3 light   = UNITY_LIGHTMODEL_AMBIENT;
	
	#if defined(FERR2DT_VERTEXLIGHTBAKED)
	light += ((1-input.color.rgb) * LIGHT_SCALE);
	#endif
	
	for (int i = 0; i < 8; i++) {
		light += GetLight(i, viewPos);
	}
	result.light = light;
	#endif
	
	#ifdef FERR2DT_TINT
	result.color *= _Color;
	#endif
	
	#if MAX_LIGHTS > 0
	result.viewpos = UnityObjectToViewPos(input.vertex).xyz;
	#endif
	#ifdef FERR2DT_LIGHTMAP
	result.lightuv = input.lightcoord * unity_LightmapST.xy + unity_LightmapST.zw;
	#endif
	result.uv      = TRANSFORM_TEX(input.texcoord0, _MainTex);

	UNITY_TRANSFER_FOG(result, result.position);

	return result;
}

fixed4 frag(VS_OUT inp) : COLOR {
	fixed4 color = tex2D(_MainTex, inp.uv);
	#ifdef FERR2DT_LIGHTMAP
	fixed3 light = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, inp.lightuv));
	#elif  MAX_LIGHTS > 0
	fixed3 light = UNITY_LIGHTMODEL_AMBIENT;
	#endif

	color      = color * inp.color;
	#if MAX_LIGHTS > 0
	for (int i = 0; i < MAX_LIGHTS; i++) {
		light += GetLight(i, inp.viewpos);
	}
	#endif

	#if defined(FERR2DT_LIGHMAP) || MAX_LIGHTS > 0
	color.rgb *= light;
	#endif
	
	#if defined(FERR2DT_VERTEXLIT)
	color.rgb *= inp.light;
	#endif

	UNITY_APPLY_FOG(inp.fogCoord, color);

	return color;
}