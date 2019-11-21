Shader "Hidden/KriptoFX/PostEffects/Slashes_Bloom"
{
    Properties
    {
        _MainTex("", 2D) = "" {}
        _BaseTex("", 2D) = "" {}
    }
    SubShader
    {
        // 0: Prefilter 
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            #include "Slashes_Bloom_Inc.cginc"
            #pragma vertex vert
            #pragma fragment frag_prefilter
            #pragma target 3.0
            ENDCG
        }
      
        // 1: First level downsampler 
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #include "Slashes_Bloom_Inc.cginc"
            #pragma vertex vert
            #pragma fragment frag_downsample
            #pragma target 3.0
            ENDCG
        }
       
        // 2: Upsampler 
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #include "Slashes_Bloom_Inc.cginc"
            #pragma vertex vert_multitex
            #pragma fragment frag_upsample
            #pragma target 3.0
            ENDCG
        }
       
        // 3: Combiner
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            #include "Slashes_Bloom_Inc.cginc"
            #pragma vertex vert_multitex
            #pragma fragment frag_upsample_final
            #pragma target 3.0
            ENDCG
        }

		// 4: Final
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			CGPROGRAM

			#include "Slashes_Bloom_Inc.cginc"
			#pragma vertex vert
			#pragma fragment frag_additive_final
			#pragma target 3.0
			ENDCG
		}
      
    }
}
