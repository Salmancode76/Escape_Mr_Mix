// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Blood/BloodDecalShader"
{
	Properties
	{
		_MainTexture("MainTexture", 2D) = "white" {}
		_TrasparencyPower("TrasparencyPower", Range( 0.1 , 1)) = 0.6
		_MainColorTintFadeA("MainColorTint-Fade(A)", Color) = (0.9558824,0.9277682,0.9277682,1)
		_Gloss("Gloss", Range( 0 , 1)) = 0.7
		_Metallic("Metallic", Range( 0 , 1)) = 0.25
		_MainNormalMap("MainNormalMap", 2D) = "bump" {}
		_Noise1("Noise1", Range( 0 , 50)) = 0
		_Noise2("Noise2", Range( 0 , 25)) = 3
		_Scale("Scale", Range( 0 , 1)) = 0
		_Bias("Bias", Range( 0 , 1)) = 0
		_DriedBloodEffectLevel("DriedBloodEffectLevel", Range( 0 , 1)) = 0.5
		_DriedBlood_Curvature("DriedBlood_Curvature", 2D) = "white" {}
		_DriedBlood_Normal("DriedBlood_Normal", 2D) = "bump" {}
		_DriedBloodColorGloss("DriedBloodColor-Gloss", Color) = (0.083045,0.08415466,0.08823532,0)
		_DriedBloodEffect_Tiling("DriedBloodEffect_Tiling", Range( 0 , 1000)) = 0
		_DriedBlood_Metallic("DriedBlood_Metallic", Range( 0 , 1)) = 0.5
		_DriedOpacityLevel("DriedOpacityLevel", Range( 0 , 5)) = 0
		_OverallNormalPower("OverallNormalPower", Range( 0 , 4)) = 0.8
		_Noise_Texture("Noise_Texture", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma surface surf Standard alpha:fade
		#pragma target 3.0

		sampler2D _MainTexture;
		sampler2D _MainNormalMap;
		sampler2D _DriedBlood_Normal;
		sampler2D _Noise_Texture;
		sampler2D _DriedBlood_Curvature;

		float4 _MainTexture_ST;
		float4 _MainNormalMap_ST;
		float4 _Noise_Texture_ST;
		float _TrasparencyPower;
		float4 _MainColorTintFadeA;
		float _Gloss;
		float _Metallic;
		float _Noise1;
		float _Noise2;
		float _Scale;
		float _Bias;
		float _DriedBloodEffectLevel;
		float _DriedBloodEffect_Tiling;
		float _DriedBlood_Metallic;
		float _DriedOpacityLevel;
		float _OverallNormalPower;
		float4 _DriedBloodColorGloss;

		struct Input
		{
			float2 uv_texcoord;
		};

		float snoise(float2 v)
		{
			const float4 C = float4(0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439);
			float2 i = floor(v + dot(v, C.yy));
			float2 x0 = v - i + dot(i, C.xx);
			float2 i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = i - floor(i * (1.0 / 289.0)) * 289.0;
			float3 p = frac(((i.y + float3(0.0, i1.y, 1.0)) * 34.0 + 1.0) * (i.x + float3(0.0, i1.x, 1.0))) * 289.0;
			float3 m = max(0.5 - float3(dot(x0, x0), dot(x12.xy, x12.xy), dot(x12.zw, x12.zw)), 0.0);
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac(p * C.www) - 1.0;
			float3 h = abs(x) - 0.5;
			float3 ox = floor(x + 0.5);
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * (a0 * a0 + h * h);
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot(m, g);
		}

		void surf(Input i, inout SurfaceOutputStandard o)
		{
			float2 uvMain = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			float2 uvNormal = i.uv_texcoord * _MainNormalMap_ST.xy + _MainNormalMap_ST.zw;
			float2 uvDried = i.uv_texcoord * _DriedBloodEffect_Tiling;

			float3 normalMain = UnpackNormal(tex2D(_MainNormalMap, uvNormal));
			float3 normalDried = UnpackNormal(tex2D(_DriedBlood_Normal, uvDried));
			o.Normal = lerp(float3(0, 0, 1), lerp(normalMain, normalDried, _DriedBloodEffectLevel), _OverallNormalPower);

			float2 uvNoise1 = i.uv_texcoord * _Noise1;
			float2 uvNoise2 = i.uv_texcoord * (_Noise1 * _Noise2);
			float noise1 = snoise(uvNoise1);
			float noise2 = snoise(uvNoise2);
			float2 uvNoiseTex = i.uv_texcoord * _Noise_Texture_ST.xy + _Noise_Texture_ST.zw;
			float noiseTex = tex2D(_Noise_Texture, uvNoiseTex).r;

			float combinedNoise = pow(((1.0 - (noise1 * noise2 * noiseTex)) + _Bias) * _Scale, 3.0 * _Bias);
			float3 noiseMix = clamp((combinedNoise * _DriedBloodEffectLevel) + (1.0 - _DriedBloodEffectLevel), 0, 1);

			float4 baseTex = tex2D(_MainTexture, uvMain);
			float4 finalColor = _MainColorTintFadeA * baseTex.r;
			finalColor.rgb *= noiseMix;
			finalColor.rgb = lerp(finalColor.rgb, _DriedBloodColorGloss.rgb, _DriedBloodEffectLevel);

			o.Albedo = finalColor.rgb;
			o.Metallic = lerp(_Metallic, _DriedBlood_Metallic, _DriedBloodEffectLevel);
			o.Smoothness = _Gloss;
			o.Alpha = saturate(finalColor.a * (1.0 - _TrasparencyPower)) * (1.0 - (_DriedOpacityLevel * _DriedBloodEffectLevel));
		}
		ENDCG
	}
	Fallback "Diffuse"
}
