Shader "RzShader/Rz_Distortion" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _FlowStrength ("FlowStrength", Float ) = 0.2
        _FlowSpeed ("FlowSpeed", Float ) = 0.5
        _RefractionStrength ("RefractionStrength", Float ) = 1
        _FlowTex ("FlowTex", 2D) = "white" {}
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D Refraction;
            uniform float4 _TimeEditor;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _FlowStrength;
            uniform float _FlowSpeed;
            uniform float _RefractionStrength;
            uniform sampler2D _FlowTex; uniform float4 _FlowTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.screenPos = float4( o.pos.xy / o.pos.w, 0, 0 );
                o.screenPos.y *= _ProjectionParams.x;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                float2 UV = i.uv0;
                float2 node_8688 = UV;
                float4 node_367 = _Time + _TimeEditor;
                float node_3627 = (node_367.g*_FlowSpeed);
                float2 node_1691 = (node_8688+node_3627*float2(1,-0.05));
                float4 node_116 = tex2D(_FlowTex,TRANSFORM_TEX(node_1691, _FlowTex));
                float2 node_8085 = (node_8688+node_3627*float2(0.5,0.05));
                float4 node_2698 = tex2D(_FlowTex,TRANSFORM_TEX(node_8085, _FlowTex));
                float2 node_5101 = ((node_8688+(((node_116.rgb+node_2698.rgb)/2.0).rg*_FlowStrength*abs(i.uv1.g)))+i.uv1.r*float2(1,0));
                float4 node_8211 = tex2D(_MainTex,TRANSFORM_TEX(node_5101, _MainTex));
                float2 node_9633 = UV;
                float4 node_6216 = tex2D(_MainTex,TRANSFORM_TEX(node_9633, _MainTex));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (UV*(node_8211.b*node_6216.a)*(_RefractionStrength*i.uv1.b));
                float4 sceneColor = tex2D(Refraction, sceneUVs);
////// Lighting:
                float3 finalColor = 0;
                return fixed4(lerp(sceneColor.rgb, finalColor,0.0),1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
