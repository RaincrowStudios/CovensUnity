Shader "Custom/PostProcess"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_VRadius("Vignette Radius", Range(0.0, 1.0)) = 0.8
		_VSoft("Vignette Softness", Range(0.0, 1.0)) = 0.5
		_DisplaceTex("Displacement Texture",2D) = "white"{}
		_Magnitude("Magnitude",Range(0,.1)) =1 
		_LuminosityAmount ("Grayscale Amount", Range(0,1)) = 1.0
		_AnimateSpeed ("Animation Speed", Range(0,5)) = 1.0
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
            #include "UnityCG.cginc"
			
			// Properties
			sampler2D _MainTex;
			sampler2D _DisplaceTex;
			float4 _Color;
			float4 _GlitchColor;
			float _VRadius;
			float _VSoft;
			float _Magnitude;
			float _LuminosityAmount;
			float _AnimateSpeed;
			float4 frag(v2f_img input) : COLOR
			{

				

				float distFromCenter = distance(input.uv.xy, float2(0.5, 0.5));

				

				float vignette = smoothstep(_VRadius, _VRadius - _VSoft, distFromCenter);

			//	float2 distuv = float2(input.uv.x + _Time.x * _AnimateSpeed, input.uv.y + _Time.x * _AnimateSpeed);

				float2 disp = tex2D(_DisplaceTex,input.uv).xy;
				disp = ((disp*2)-1)*_Magnitude*(1-vignette);

				float4 base = tex2D(_MainTex, input.uv + disp);

				base = saturate(base * vignette);
				

				float luminosity = 0.299 * base.r + 0.587 * base.g + 0.114 * base.b;
				float4 finalColor = lerp(base, luminosity, _LuminosityAmount);


				return finalColor;
			}

			ENDCG
		}
	}
}