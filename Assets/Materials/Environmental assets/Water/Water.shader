Shader "WWG/Water"
{
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_AnimSpeed ("Animation Speed", Range(0,10)) = 1
		_Scale ("Scale", Range(0,10)) = 1
	}

	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard alpha
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _AnimSpeed;
		float _Scale;
		void surf (Input IN, inout SurfaceOutputStandard o) {
			float2 uv = IN.uv_MainTex;
			uv.x *= 2 * _Scale;
			uv.x += _Time.y * 0.011;
			uv.y *= 1 * _Scale;
			uv.y -= 0.08 * sin(_Time.y * _AnimSpeed * (1/_Scale));
			float4 noise = tex2D(_MainTex, uv);

			float2 uv2 = IN.uv_MainTex;
			uv2.x -= _Time.y * 0.01;
			uv.y -= 0.081 * sin(_Time.y * _AnimSpeed * (1/_Scale));
			float4 noise2 = tex2D(_MainTex, uv2);

			float2 uv3 = IN.uv_MainTex;
			uv3 *= 0.5 * _Scale;
			uv3.y *= 1.2;
			uv3.x *= 0.07 * _Scale;
			uv3.y -= _Time.y * _AnimSpeed * 0.09 * (1/_Scale);
			float4 noise3 = tex2D(_MainTex, uv3); 

			float4 cmb = (noise.r * noise2.a);
			float sum = (cmb.r + cmb.g + cmb.b);
			if(noise3.r > sum*0.7 && noise3.r < sum*1.2) cmb.rgb *= 3;
			fixed4 c = saturate(_Color + cmb);
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1;//c.a + 0.5;
			//o.Normal = c;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
