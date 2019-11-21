Shader "RzShader/Rz_Slash" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _EmissStrength ("EmissStrength", Float ) = 1
        _MainTex ("MainTex", 2D) = "white" {}
        _FlowMap ("FlowMap", 2D) = "white" {}
        _FlowStrength ("FlowStrength", Float ) = 0.2
        _FlowSpeed ("FlowSpeed", Float ) = 0.5
        _BlackAmount ("BlackAmount", Float ) = 1
        _RefractionStrength ("RefractionStrength", Float ) = 1
        _SharpEdge ("SharpEdge", Float ) = 0
        [MaterialToggle] _KeepEdge_NonFlowmap ("KeepEdge_NonFlowmap", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ "Refraction" }
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
            #pragma only_renderers d3d9 d3d11 
            #pragma target 3.0
            uniform sampler2D Refraction;
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform float _EmissStrength;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _FlowMap; uniform float4 _FlowMap_ST;
            uniform float _FlowStrength;
            uniform float _FlowSpeed;
            uniform float _BlackAmount;
            uniform float _RefractionStrength;
            uniform float _SharpEdge;
            uniform fixed _KeepEdge_NonFlowmap;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float2 UV = i.uv0;
                float2 node_8688 = UV;
                float4 node_367 = _Time + _TimeEditor;
                float node_3627 = (node_367.g*_FlowSpeed);
                float2 node_1691 = (node_8688+node_3627*float2(1,-0.05));
                float4 node_116 = tex2D(_FlowMap,TRANSFORM_TEX(node_1691, _FlowMap));
                float2 node_8085 = (node_8688+node_3627*float2(0.5,0.05));
                float4 node_2698 = tex2D(_FlowMap,TRANSFORM_TEX(node_8085, _FlowMap));
                float2 node_3772 = (((node_116.rgb+node_2698.rgb)/2.0).rg*_FlowStrength*abs(i.uv1.a));
                float2 node_5346 = (node_8688+lerp( node_3772, (float2(saturate((i.uv0.r*-5.0+5.0)),1.0)*node_3772), _KeepEdge_NonFlowmap ));
                float2 node_5101 = (node_5346+i.uv1.r*float2(1,0));
                float4 node_8211 = tex2D(_MainTex,TRANSFORM_TEX(node_5101, _MainTex));
                float2 node_9633 = UV;
                float4 node_6216 = tex2D(_MainTex,TRANSFORM_TEX(node_9633, _MainTex));
                float node_5099 = (((node_8211.g+node_8211.b)/2.0)*node_6216.a);
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (UV*node_5099*(_RefractionStrength*(i.uv1.r*0.5+0.5)*i.uv2.r));
                float4 sceneColor = tex2D(Refraction, sceneUVs);
////// Lighting:
////// Emissive:
                float3 emissive = (((((_Color.rgb*node_8211.g)+(node_8211.r*_SharpEdge))*_EmissStrength)*i.vertexColor.rgb)*saturate(i.uv1.b));
                float3 finalColor = emissive;
                float2 node_4219 = (node_5346+i.uv1.r*float2(0.25,0));
                float4 node_3522 = tex2D(_FlowMap,TRANSFORM_TEX(node_4219, _FlowMap));
                return fixed4(lerp(sceneColor.rgb, finalColor,saturate((i.vertexColor.a*node_5099*_BlackAmount*saturate((node_3522.b+i.uv1.g))))),1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
