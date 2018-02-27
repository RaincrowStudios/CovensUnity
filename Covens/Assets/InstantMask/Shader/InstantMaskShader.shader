// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/InstantMaskShader" {

	Properties
	{
		[HideInInspector] _MainTex ("Base Image", 2D) = "white" {} //[PerRendererData] //[NoScaleOffset]
		[HideInInspector] _MaskTex ("Mask1 Image", 2D) = "white" {}
		[HideInInspector] _MaskTex2 ("Mask2 Image", 2D) = "white" {}
		[HideInInspector] _MaskTex3 ("Mask3 Image", 2D) = "white" {}
		[HideInInspector] [MaterialToggle] _UseMask1Check ("Use Mask1", Int) = 1
		[HideInInspector] [MaterialToggle] _UseMask2Check ("Use Mask2", Int) = 0
		[HideInInspector] [MaterialToggle] _UseMask3Check ("Use Mask3", Int) = 0
		[HideInInspector] _ResultOpacity ("Result Opacity", Range (0,100)) = 100
		[HideInInspector] _AlphaCutoff ("Alpha Cutoff", Range (0,1)) = 0
		[HideInInspector] [MaterialToggle] _MaskVisibility ("Mask Visibility", Int) = 0
		[HideInInspector] _VisibleMaskNumber ("Visible MaskNumber", Int) = 0
		[HideInInspector] _VisibleColor ("Visible Color", Color) = (0,0.6,0,0.6)
		[HideInInspector] [MaterialToggle] _Mask1TilingPivotToCenter ("Mask1 Tiling Pivot To Center", Int) = 0
		[HideInInspector] _Mask1Degree ("Mask1 Degree", Range (0,100)) = 100
		[HideInInspector] _Mask1PosX("Mask1 PosX", Float) = 0
		[HideInInspector] _Mask1PosY("Mask1 PosY", Float) = 0
		[HideInInspector] _Mask1Angle ("Mask1 Angle", Float) = 0
		[HideInInspector] [MaterialToggle] _ReverseMask1 ("Reverse Mask1 Area", Int) = 0
		[HideInInspector] [MaterialToggle] _GrayscaleMask1 ("Grayscale Mask1", Int) = 0
		[HideInInspector] [MaterialToggle] _Mask2TilingPivotToCenter ("Mask2 Tiling Pivot To Center", Int) = 0
		[HideInInspector] _Mask2Degree ("Mask2 Degree", Range (0,100)) = 100
		[HideInInspector] _Mask2PosX("Mask2 PosX", Float) = 0
		[HideInInspector] _Mask2PosY("Mask2 PosY", Float) = 0
		[HideInInspector] _Mask2Angle ("Mask2 Angle", Float) = 0
		[HideInInspector] _Mask2Prescribe ("Mask2 Prescribe", Int) = 0
		[HideInInspector] [MaterialToggle] _ReverseMask2 ("Reverse Mask2 Area", Int) = 0
		[HideInInspector] [MaterialToggle] _GrayscaleMask2 ("Grayscale Mask2", Int) = 0
		[HideInInspector] [MaterialToggle] _Mask3TilingPivotToCenter ("Mask3 Tiling Pivot To Center", Int) = 0
		[HideInInspector] _Mask3Degree ("Mask3 Degree", Range (0,100)) = 100
		[HideInInspector] _Mask3PosX("Mask3 PosX", Float) = 0
		[HideInInspector] _Mask3PosY("Mask3 PosY", Float) = 0
		[HideInInspector] _Mask3Angle ("Mask3 Angle", Float) = 0
		[HideInInspector] _Mask3Prescribe ("Mask3 Prescribe", Int) = 0
		[HideInInspector] [MaterialToggle] _ReverseMask3 ("Reverse Mask3 Area", Int) = 0
		[HideInInspector] [MaterialToggle] _GrayscaleMask3 ("Grayscale Mask3", Int) = 0
		[KeywordEnum(None, Front, Back)] _Cull("Culling", Int) = 0
	}
	
	SubShader
	{
		LOD 100
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType"="Plane"
		}

		Cull [_Cull]
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#define PI 3.14159265359

			struct appdata_t
			{
				float4 vertex : POSITION0;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float2 texcoord3 : TEXCOORD3;
				float2 texcoord4 : TEXCOORD4;
				float2 texcoord5 : TEXCOORD5;
				fixed4 color : COLOR;
			};

	
			sampler2D _MainTex;
			sampler2D _MaskTex;
			sampler2D _MaskTex2;
			sampler2D _MaskTex3;
			float4 _MainTex_ST;
			float4 _MaskTex_ST;
			float4 _MaskTex2_ST;
			float4 _MaskTex3_ST;
			fixed _UseMask1Check;
			fixed _UseMask2Check;
			fixed _UseMask3Check;
			fixed _ResultOpacity;
			fixed _MaskVisibility;
			fixed _VisibleMaskNumber;
			fixed4 _VisibleColor;
			fixed _Mask1TilingPivotToCenter;
			fixed _Mask1Degree;
			float _Mask1PosX;
			float _Mask1PosY;
			float _Mask1Angle;
			fixed _ReverseMask1;
			fixed _GrayscaleMask1;
			fixed _Mask2TilingPivotToCenter;
			fixed _Mask2Degree;
			float _Mask2PosX;
			float _Mask2PosY;
			float _Mask2Angle;
			fixed _Mask2Prescribe;
			fixed _ReverseMask2;
			fixed _GrayscaleMask2;
			fixed _Mask3TilingPivotToCenter;
			fixed _Mask3Degree;
			float _Mask3PosX;
			float _Mask3PosY;
			float _Mask3Angle;
			fixed _Mask3Prescribe;
			fixed _ReverseMask3;
			fixed _GrayscaleMask3;
			fixed _AlphaCutoff;

			v2f vert (appdata_t v)
			{
				float _RotVal = PI/180 * _Mask1Angle;//円周率で半回転
				float sinX = sin (_RotVal);
				float cosX = cos (_RotVal);
				float2x2 rotationMatrix = float2x2(cosX, -sinX, sinX, cosX);
				float2 mask1uv = v.texcoord.xy;//マスクを画像サイズでなくモデルのポリゴン形(最終出力画像と同じ形)に
				mask1uv -= float2(_Mask1PosX, _Mask1PosY);
				//画像中心を基点にして回転させ元の位置に戻す
				mask1uv -= 0.5;
				mask1uv = mul (mask1uv, rotationMatrix);
				mask1uv += 0.5;
				if(_Mask1TilingPivotToCenter) {
					float tempTilingX1 =  abs(_MaskTex_ST.x);
					float tempTilingY1 =  abs(_MaskTex_ST.y);
					_MaskTex_ST.z -= (_MaskTex_ST.x*0.5) - (tempTilingX1*0.5f/tempTilingX1);
					_MaskTex_ST.w -= (_MaskTex_ST.y*0.5) - (tempTilingY1*0.5f/tempTilingY1);
				}
					
				float _RotVal2 = PI/180 * _Mask2Angle;
				float sinX2 = sin (_RotVal2);
				float cosX2 = cos (_RotVal2);
				float2x2 rotMatrix2 = float2x2(cosX2, -sinX2, sinX2, cosX2);
				float2 mask2uv = v.texcoord.xy;
				mask2uv -= float2(_Mask2PosX, _Mask2PosY);
				mask2uv -= 0.5;
				mask2uv = mul (mask2uv, rotMatrix2);
				mask2uv += 0.5;
				if(_Mask2TilingPivotToCenter) {
					float tempTilingX2 =  abs(_MaskTex2_ST.x);
					float tempTilingY2 =  abs(_MaskTex2_ST.y);
					_MaskTex2_ST.z -= (_MaskTex2_ST.x*0.5) - (tempTilingX2*0.5f/tempTilingX2);
					_MaskTex2_ST.w -= (_MaskTex2_ST.y*0.5) - (tempTilingY2*0.5f/tempTilingY2);
				}
				
				float _RotVal3 = PI/180 * _Mask3Angle;
				float sinX3 = sin (_RotVal3);
				float cosX3 = cos (_RotVal3);
				float2x2 rotMatrix3 = float2x2(cosX3, -sinX3, sinX3, cosX3);
				float2 mask3uv = v.texcoord.xy;
				mask3uv -= float2(_Mask3PosX, _Mask3PosY);
				mask3uv -= 0.5;
				mask3uv = mul (mask3uv, rotMatrix3);
				mask3uv += 0.5;
				if(_Mask3TilingPivotToCenter) {
					float tempTilingX3 =  abs(_MaskTex3_ST.x);
					float tempTilingY3 =  abs(_MaskTex3_ST.y);
					_MaskTex3_ST.z -= (_MaskTex3_ST.x*0.5) - (tempTilingX3*0.5f/tempTilingX3);
					_MaskTex3_ST.w -= (_MaskTex3_ST.y*0.5) - (tempTilingY3*0.5f/tempTilingY3);
				}
					
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoord3 = TRANSFORM_TEX(mask1uv, _MaskTex);
				o.texcoord4 = TRANSFORM_TEX(mask2uv, _MaskTex2);
				o.texcoord5 = TRANSFORM_TEX(mask3uv, _MaskTex3);
				o.color = v.color;
				#ifdef UNITY_HALF_TEXEL_OFFSET
				o.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
				#endif

				return o;
			}
				
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				col =  col * i.color;
				fixed4 mcol = tex2D(_MaskTex, i.texcoord3);
				fixed4 mcol2 = tex2D(_MaskTex2, i.texcoord4);
				fixed4 mcol3 = tex2D(_MaskTex3, i.texcoord5);
				fixed4 base = col;
				fixed AlphaValue;
				fixed Tempa;
				
				if(_UseMask1Check>0){
					if(_GrayscaleMask1>0){
						if(mcol.a<=0){
							AlphaValue = 0;
						}else{
							AlphaValue=max(mcol.r,max(mcol.g,mcol.b));
							//if(mcol.r>mcol.g){
							//	if(mcol.r>mcol.b){
							//		AlphaValue = mcol.r;
							//	}else{
							//		if(mcol.b>mcol.g){
							//			AlphaValue = mcol.b;
							//		}else{
							//			AlphaValue = mcol.g;
							//		}
							//	}
							//}else{
							//	if(mcol.g>mcol.b){
							//		AlphaValue = mcol.g;
							//	}else{
							//		if(mcol.b>mcol.r){
							//			AlphaValue = mcol.b;
							//		}else{
							//			AlphaValue = mcol.r;
							//		}
							//	}
							//}
						}
					}else{
						AlphaValue = mcol.a;
					}
					if(_ReverseMask1>0){
						//col.a=col.a*mcol.a - 0.01;
						col.a = col.a - (AlphaValue*(_Mask1Degree/100));
					}else{
						//col.a=col.a-(col.a*mcol.a) - 0.01;
						col.a = col.a - ((1-AlphaValue)*(_Mask1Degree/100));
					}
				}
				if(_UseMask2Check>0){
					if(_GrayscaleMask2>0){
						if(mcol2.a<=0){
							AlphaValue = 0;
						}else{
							AlphaValue=max(mcol2.r,max(mcol2.g,mcol2.b));
						}
					}else{
						AlphaValue = mcol2.a;
					}
					if(_ReverseMask2>0){
						if(_Mask2Prescribe<=0){
							//非表示部分を引く処理
							col.a = col.a - (AlphaValue*(_Mask2Degree/100));
						}else if(0<_Mask2Prescribe && _Mask2Prescribe<=1){
							//表示部分を加える処理
							col.a = col.a + ((1-AlphaValue)*(_Mask2Degree/100));
						}
					}else{
						if(_Mask2Prescribe<=0){
							//非表示部分を引く処理
							col.a = col.a - ((1-AlphaValue)*(_Mask2Degree/100));
						}else if(0<_Mask2Prescribe && _Mask2Prescribe<=1){
							//表示部分を加える処理
							col.a = col.a + (AlphaValue*(_Mask2Degree/100));
						}else if(1<_Mask2Prescribe && _Mask2Prescribe<=2){
							//ここまでのマスクで非表示域なら加える処理,表示域なら引く処理
							Tempa = col.a - (AlphaValue*(_Mask2Degree/100));
							if(Tempa<0){
								Tempa = -(Tempa);
							}
							col.a = Tempa;
						}
					}
				}
				if(_UseMask3Check>0){
					if(_GrayscaleMask3>0){
						if(mcol3.a<=0){
							AlphaValue = 0;
						}else{
							AlphaValue=max(mcol3.r,max(mcol3.g,mcol3.b));
						}
					}else{
						AlphaValue = mcol3.a;
					}
					if(_ReverseMask3>0){
						if(_Mask3Prescribe<=0){
							//非表示部分を引く処理
							col.a = col.a - (AlphaValue*(_Mask3Degree/100));
						}else if(0<_Mask3Prescribe && _Mask3Prescribe<=1){
							//表示部分を加える処理
							col.a = col.a + ((1-AlphaValue)*(_Mask3Degree/100));
						}
					}else{
						if(_Mask3Prescribe<=0){
							//非表示部分を引く処理
							col.a = col.a - ((1-AlphaValue)*(_Mask3Degree/100));
						}else if(0<_Mask3Prescribe && _Mask3Prescribe<=1){
							//表示部分を加える処理
							col.a = col.a + (AlphaValue*(_Mask3Degree/100));
						}else if(1<_Mask3Prescribe && _Mask3Prescribe<=2){
							//ここまでのマスクで非表示域なら加える処理,表示域なら引く処理
							Tempa = col.a - (AlphaValue*(_Mask3Degree/100));
							if(Tempa<0){
								Tempa = -(Tempa);
							}
							col.a = Tempa;
						}
					}
				}
				
				col.a = base.a*col.a;// BaseImageに元からある透明部分を表示ピクセルにしてしまわないように
				col.a = col.a*(_ResultOpacity/100)-0.01;

				if(_MaskVisibility>0){
					if(_VisibleMaskNumber<=0) {
						if(_ReverseMask1>0){
							col.r = col.r + ((mcol.r-col.r)*mcol.a) - ((1-_VisibleColor.r)*mcol.a*_VisibleColor.a);
							col.g = col.g + ((mcol.g-col.g)*mcol.a) - ((1-_VisibleColor.g)*mcol.a*_VisibleColor.a);
							col.b = col.b + ((mcol.b-col.b)*mcol.a) - ((1-_VisibleColor.b)*mcol.a*_VisibleColor.a);
						}else{
							col.r = col.r + ((mcol.r-col.r)*mcol.a*_VisibleColor.a) - (1-_VisibleColor.r)*mcol.a*_VisibleColor.a;
							col.g = col.g + ((mcol.g-col.g)*mcol.a*_VisibleColor.a) - (1-_VisibleColor.g)*mcol.a*_VisibleColor.a;
							col.b = col.b + ((mcol.b-col.b)*mcol.a*_VisibleColor.a) - (1-_VisibleColor.b)*mcol.a*_VisibleColor.a;
						}
							//Visibilityが半透明時にベース画像が見えないようにする場合は以下,この場合マスク反転していない時にベース画像が塗りつぶされる感じになる
							//col.r = col.r*(1-mcol.a) + (mcol.r*mcol.a*_VisibleColor.a) - (1-_VisibleColor.r)*mcol.a*_VisibleColor.a;
							//col.g = col.g*(1-mcol.a) + (mcol.g*mcol.a*_VisibleColor.a) - (1-_VisibleColor.g)*mcol.a*_VisibleColor.a;
							//col.b = col.b*(1-mcol.a) + (mcol.b*mcol.a*_VisibleColor.a) - (1-_VisibleColor.b)*mcol.a*_VisibleColor.a;
						col.a = col.a+(mcol.a*_VisibleColor.a);
					}else if(0<_VisibleMaskNumber && _VisibleMaskNumber<=1) {
						if(_ReverseMask2>0){
							col.r = col.r + ((mcol2.r-col.r)*mcol2.a) - ((1-_VisibleColor.r)*mcol2.a*_VisibleColor.a);
							col.g = col.g + ((mcol2.g-col.g)*mcol2.a) - ((1-_VisibleColor.g)*mcol2.a*_VisibleColor.a);
							col.b = col.b + ((mcol2.b-col.b)*mcol2.a) - ((1-_VisibleColor.b)*mcol2.a*_VisibleColor.a);
						}else{
							col.r = col.r + ((mcol2.r-col.r)*mcol2.a*_VisibleColor.a) - (1-_VisibleColor.r)*mcol2.a*_VisibleColor.a;
							col.g = col.g + ((mcol2.g-col.g)*mcol2.a*_VisibleColor.a) - (1-_VisibleColor.g)*mcol2.a*_VisibleColor.a;
							col.b = col.b + ((mcol2.b-col.b)*mcol2.a*_VisibleColor.a) - (1-_VisibleColor.b)*mcol2.a*_VisibleColor.a;
						}
						col.a = col.a+(mcol2.a*_VisibleColor.a);
					}else if(1<_VisibleMaskNumber && _VisibleMaskNumber<=2) {
						if(_ReverseMask3>0){
							col.r = col.r + ((mcol3.r-col.r)*mcol3.a) - ((1-_VisibleColor.r)*mcol3.a*_VisibleColor.a);
							col.g = col.g + ((mcol3.g-col.g)*mcol3.a) - ((1-_VisibleColor.g)*mcol3.a*_VisibleColor.a);
							col.b = col.b + ((mcol3.b-col.b)*mcol3.a) - ((1-_VisibleColor.b)*mcol3.a*_VisibleColor.a);
						}else{
							col.r = col.r + ((mcol3.r-col.r)*mcol3.a*_VisibleColor.a) - (1-_VisibleColor.r)*mcol3.a*_VisibleColor.a;
							col.g = col.g + ((mcol3.g-col.g)*mcol3.a*_VisibleColor.a) - (1-_VisibleColor.g)*mcol3.a*_VisibleColor.a;
							col.b = col.b + ((mcol3.b-col.b)*mcol3.a*_VisibleColor.a) - (1-_VisibleColor.b)*mcol3.a*_VisibleColor.a;
						}
						col.a = col.a+(mcol3.a*_VisibleColor.a);
					}
				}
				
				if(_AlphaCutoff>=1
					|| col.a<0.001
					|| _AlphaCutoff>0 && col.a<=_AlphaCutoff)
				{
				#if (SHADER_API_MOBILE)
					col.a = 0;
				#else
					discard;
				#endif
				}
				return col;
			}
			ENDCG
		}
	}
	Fallback Off
}
