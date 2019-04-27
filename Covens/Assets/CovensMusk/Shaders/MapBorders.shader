
Shader "Covens/MapBorders"
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (0,0,0,0)
		_Screen ("Screen bounds", Vector) = (0, 0, 1280, 720)
	}
	SubShader 
	{
		Cull Off ZWrite Off ZTest Always
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
		
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD8;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);    
				o.screenPos = ComputeScreenPos(o.vertex);
				o.uv = v.uv;
				return o;
			}

			uniform sampler2D _MainTex;
			float4 _Color;
			float4 _Screen;

			fixed4 frag (v2f i) : SV_Target 
			{
				fixed4 base = tex2D(_MainTex, i.uv);

				if (i.screenPos.x <= _Screen.x)
					return _Color;
				if (i.screenPos.x >= _Screen.z)
					return _Color;

				if (i.screenPos.y <= _Screen.y)
					return _Color;
				if (i.screenPos.y >= _Screen.w)
					return _Color;

				return base;
			}
			ENDCG
		}
	}
}