Shader "Blood/BloodWallDripShader_URP"
{
    Properties
    {
        _MainDriverTexture("MainDriverTexture", 2D) = "white" {}
        _NormalMap("NormalMap", 2D) = "bump" {}
        _Color_Gloss("Color_Gloss", Color) = (0.588,0,0,0.872)
        _DryColor_Gloss("DryColor_Gloss", Color) = (0.588,0,0,0.872)
        _YOffset("YOffset", Range(-1,1)) = -0.1
        _BloodDrying("BloodDrying", Range(0,1)) = 0
        _Adjust("Adjust", Range(0,1)) = 0
        _Contrast("Contrast", Range(0,60)) = 6
        _Noise("Noise", Float) = 20
        _Fade("Fade", Range(0,1)) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200

        Pass
        {
            Name "FORWARD"
            HLSLPROGRAM
            // Vertex/Fragment shader for URP
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Define texture samplers and uniforms
            TEXTURE2D(_MainDriverTexture);
            SAMPLER(sampler_MainDriverTexture);

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            float4 _MainDriverTexture_ST;
            float _YOffset;
            float _BloodDrying;
            float _Adjust;
            float _Contrast;
            float4 _Color_Gloss;
            float4 _DryColor_Gloss;
            float _Fade;
            float _Noise;

            // Attributes from vertex stream
            struct Attributes
            {
                float4 vertex   : POSITION;
                float2 uv       : TEXCOORD0;
            };

            // Varyings passed to fragment
            struct Varyings
            {
                float4 pos   : SV_POSITION;
                float2 uv    : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };

            // Vertex shader: transform position and pass UVs.
            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.pos = TransformObjectToHClip(IN.vertex.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainDriverTexture);
                UNITY_TRANSFER_FOG(OUT, OUT.pos);
                return OUT;
            }

            // A simple placeholder noise function.
            float SimpleNoise(float2 uv)
            {
                // You can replace this with your own noise math.
                return frac(sin(dot(uv, float2(12.9898,78.233))) * 43758.5453);
            }

            // Fragment shader: sample textures and calculate color.
            half4 Frag(Varyings IN) : SV_Target
            {
                // Sample the main driver texture.
                float2 uv = IN.uv;
                float4 mainTex = SAMPLE_TEXTURE2D(_MainDriverTexture, sampler_MainDriverTexture, uv);

                // Modify UV based on Y-offset and driver texture blue channel.
                float2 offset = float2(0.0, _YOffset);
                uv += offset * mainTex.b;
                float4 driverSample = SAMPLE_TEXTURE2D(_MainDriverTexture, sampler_MainDriverTexture, uv);

                // Lerp based on _Adjust.
                float lerpValue = lerp(driverSample.g, driverSample.b, _Adjust);
                float powerValue = pow(lerpValue, _Contrast);
                float drying = clamp(_BloodDrying + (1.0 - powerValue), 0.0, 0.7);

                // Normal sampling (using URP’s derivative functions would be better).
                float3 normalSample = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv).rgb;
                // Blend toward default normal.
                float3 finalNormal = lerp(normalSample * 2.0 - 1.0, float3(0, 0, 1), drying);

                // Color calculation.
                float4 colA = _Color_Gloss * (driverSample.r * driverSample.g) * driverSample.g;
                float4 colB = _DryColor_Gloss * driverSample.r;
                float4 finalColor = lerp(colA, colB, _BloodDrying);

                // Calculate opacity using a simple noise.
                float noiseValue = SimpleNoise(uv * _Noise);
                float alpha = clamp(pow(powerValue + powerValue * noiseValue + powerValue, _Contrast), 0.0, 1.0);
                alpha *= (1.0 - _Fade);

                half4 outputColor = half4(finalColor.rgb, alpha);

                UNITY_APPLY_FOG(IN.fogCoord, outputColor);
                return outputColor;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Forward"
}
