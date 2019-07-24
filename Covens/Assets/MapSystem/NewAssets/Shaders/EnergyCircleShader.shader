Shader "Custom/MarkerEnergyCircle"
{
	Properties
	{
		_MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert nofog nolightmap nodynlightmap keepalpha 
			//noinstancing
		#pragma multi_compile _ PIXELSNAP_ON
		#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
		#include "UnitySprites.cginc"

		struct Input
		{
			float2 uv_MainTex;
			fixed4 color;
		};

		void vert(inout appdata_full v, out Input o)
		{
			//v.vertex = UnityFlipSprite(v.vertex, _Flip);

			//#if defined(PIXELSNAP_ON)
			//v.vertex = UnityPixelSnap(v.vertex);
			//#endif

			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color * _RendererColor;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			float x = IN.uv_MainTex.x - 0.5;
			float y = IN.uv_MainTex.y - 0.5;

			float angle = atan2(y,x);
			float endAngle = lerp(-3.14, 3.14, IN.color.a);
			
			if (angle <= endAngle) 
			{
				IN.color.a = 1;
				fixed4 c = SampleSpriteTexture(IN.uv_MainTex) * IN.color;
				o.Albedo = c.rgb * c.a;
				o.Alpha = c.a;
			}
			else 
			{
				o.Albedo = (0, 0, 0);
				o.Alpha = 0;
			}
		}
		ENDCG
	}

	Fallback "Transparent/VertexLit"
}