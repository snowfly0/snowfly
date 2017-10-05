// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:5,bsrc:3,bdst:0,culm:2,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1570,x:32763,y:32697|emission-1688-OUT,alpha-1686-OUT;n:type:ShaderForge.SFN_Panner,id:1672,x:34354,y:32659,spu:0.01,spv:0.02;n:type:ShaderForge.SFN_Tex2d,id:1674,x:34158,y:32659,ptlb:node_787_copy,ptin:_node_787_copy,tex:59a67708bc53183468124dd4134131b6,ntxv:0,isnm:False|UVIN-1672-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:1676,x:33834,y:33176,ptlb:node_958,ptin:_node_958,tex:f14b2455226c6244bb2d6bebd81090db,ntxv:0,isnm:False|UVIN-1678-UVOUT;n:type:ShaderForge.SFN_Panner,id:1678,x:34092,y:33157,spu:0.01,spv:-0.02;n:type:ShaderForge.SFN_Tex2d,id:1680,x:34092,y:33349,ptlb:node_983,ptin:_node_983,tex:d7e1c17319f735b4f98e243422d15343,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Power,id:1682,x:33812,y:33408|VAL-1680-B,EXP-1684-OUT;n:type:ShaderForge.SFN_Vector1,id:1684,x:34092,y:33530,v1:3;n:type:ShaderForge.SFN_Multiply,id:1686,x:33471,y:33229|A-1676-G,B-1682-OUT;n:type:ShaderForge.SFN_Multiply,id:1688,x:33170,y:32754|A-1708-OUT,B-1690-OUT;n:type:ShaderForge.SFN_Vector3,id:1690,x:33440,y:32946,v1:0.3235294,v2:1,v3:0.748073;n:type:ShaderForge.SFN_Sin,id:1692,x:34143,y:32896|IN-1694-T;n:type:ShaderForge.SFN_Time,id:1694,x:34367,y:32877;n:type:ShaderForge.SFN_Add,id:1696,x:33752,y:32893|A-1692-OUT,B-1698-OUT;n:type:ShaderForge.SFN_Vector1,id:1698,x:34143,y:33055,v1:2;n:type:ShaderForge.SFN_Power,id:1700,x:33948,y:32676|VAL-1674-R,EXP-1702-OUT;n:type:ShaderForge.SFN_Vector1,id:1702,x:34158,y:32831,v1:1.25;n:type:ShaderForge.SFN_Multiply,id:1704,x:33752,y:32676|A-1700-OUT,B-1706-OUT;n:type:ShaderForge.SFN_Vector1,id:1706,x:33935,y:32831,v1:2;n:type:ShaderForge.SFN_Multiply,id:1708,x:33520,y:32753|A-1704-OUT,B-1696-OUT;proporder:1674-1676-1680;pass:END;sub:END;*/

Shader "Custom/shui" {
    Properties {
        _node_787_copy ("node_787_copy", 2D) = "white" {}
        _node_958 ("node_958", 2D) = "white" {}
        _node_983 ("node_983", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _node_787_copy; uniform float4 _node_787_copy_ST;
            uniform sampler2D _node_958; uniform float4 _node_958_ST;
            uniform sampler2D _node_983; uniform float4 _node_983_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_1722 = _Time + _TimeEditor;
                float2 node_1721 = i.uv0;
                float2 node_1672 = (node_1721.rg+node_1722.g*float2(0.01,0.02));
                float4 node_1694 = _Time + _TimeEditor;
                float3 emissive = (((pow(tex2D(_node_787_copy,TRANSFORM_TEX(node_1672, _node_787_copy)).r,1.25)*2.0)*(sin(node_1694.g)+2.0))*float3(0.3235294,1,0.748073));
                float3 finalColor = emissive;
                float2 node_1678 = (node_1721.rg+node_1722.g*float2(0.01,-0.02));
/// Final Color:
                return fixed4(finalColor,(tex2D(_node_958,TRANSFORM_TEX(node_1678, _node_958)).g*pow(tex2D(_node_983,TRANSFORM_TEX(node_1721.rg, _node_983)).b,3.0)));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
