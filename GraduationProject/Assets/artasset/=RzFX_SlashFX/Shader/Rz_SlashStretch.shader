Shader "RzShader/Rz_SlashStretch" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _Emission ("Emission", Float ) = 1
        _HighlightStrength ("HighlightStrength", Float ) = 1
        _FlowMap ("FlowMap", 2D) = "white" {}
        _FlowStrength ("FlowStrength", Float ) = 0.25
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Emission;
            uniform float _HighlightStrength;
            uniform sampler2D _FlowMap; uniform float4 _FlowMap_ST;
            uniform float _FlowStrength;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float node_8758 = 1.0;
                float node_6744 = 0.0;
                float2 node_6376 = float2(((node_6744 + ( (i.uv0.r - i.uv1.g) * (node_8758 - node_6744) ) / (node_8758 - i.uv1.g))+i.uv1.r),i.uv0.g);
                float4 node_1324 = tex2D(_MainTex,TRANSFORM_TEX(node_6376, _MainTex));
                float3 emissive = (((node_1324.r*_HighlightStrength)+(_Color.rgb*node_1324.g*_Emission))*i.uv1.a*i.vertexColor.rgb);
                float3 finalColor = emissive;
                float4 node_1050 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float2 UV = i.uv0;
                float2 node_4830 = UV;
                float4 node_7072 = _Time + _TimeEditor;
                float2 node_7932 = (node_4830+node_7072.g*float2(-0.5,0.05));
                float4 node_5406 = tex2D(_FlowMap,TRANSFORM_TEX(node_7932, _FlowMap));
                float2 node_6917 = (node_4830+node_7072.g*float2(-0.3,-0.1));
                float4 node_5169 = tex2D(_FlowMap,TRANSFORM_TEX(node_6917, _FlowMap));
                float2 node_3861 = (node_4830+(((node_5406.rgb+node_5169.rgb)/2.0).rg*_FlowStrength));
                float4 node_319 = tex2D(_MainTex,TRANSFORM_TEX(node_3861, _MainTex));
                return fixed4(finalColor,(i.vertexColor.a*(node_1324.g*node_1050.b*saturate(step(0.5,(node_319.a+i.uv1.b))))));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
