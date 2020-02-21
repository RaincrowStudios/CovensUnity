// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-cutout shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Oktagon/Unlit-Transparent Cutout" {
	Properties{
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_Cutoff("Alpha cutoff", Range(0.0001,1)) = 0.5
	}
		SubShader{
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
			LOD 100

			Lighting Off
			ZWrite Off

			Pass {
				CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#pragma target 2.0
					#pragma multi_compile_fog
					#pragma multi_compile_instancing 

					#include "UnityCG.cginc"

					struct appdata_t {
						float4 vertex : POSITION;
						float4 color    : COLOR;
						float2 texcoord : TEXCOORD0;
						UNITY_VERTEX_INPUT_INSTANCE_ID
					};

					struct v2f {
						float4 vertex : SV_POSITION;
						float2 texcoord : TEXCOORD0;
						fixed4 color : COLOR;
						UNITY_FOG_COORDS(1)
						UNITY_VERTEX_OUTPUT_STEREO
					};

					sampler2D _MainTex;
					float4 _MainTex_ST;
					fixed4 _Color;
					//fixed _Cutoff;
					UNITY_INSTANCING_BUFFER_START(Props)
						UNITY_DEFINE_INSTANCED_PROP(fixed, _Cutoff)
					UNITY_INSTANCING_BUFFER_END(Props)

					v2f vert(appdata_t v)
					{
						v2f o;
						UNITY_SETUP_INSTANCE_ID(v);
						UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
						o.vertex = UnityObjectToClipPos(v.vertex);
						o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
						UNITY_TRANSFER_FOG(o,o.vertex);
						o.color = v.color * _Color;
						return o;
					}

					fixed4 frag(v2f i) : SV_Target
					{
						fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
						clip(col.a - UNITY_ACCESS_INSTANCED_PROP(Props, _Cutoff));
						UNITY_APPLY_FOG(i.fogCoord, col);
						return col;
					}
				ENDCG
			}
		}

}
