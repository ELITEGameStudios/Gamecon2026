Shader "Clouds/MainCloudShader"
{
    Properties
    {
        _MaxRange("Max Range", float) = 25
        _Interval("Raymarch Interval Length", Range(0.1, 10)) = 0.1
        _BandingNoiseIntensity("Banding Noise Intensity", float) = 1
        
        _FogDensity("Additive Fog Per Step", Range(0, 1)) = 0.005
        _FogColor("Additive Fog color", Color) = (1, 1, 1, 1)
        _FogNoiseTex("Noise texture", 3D) = "white" {}
        _FogNoiseTile("Noise Tiling", float) = 1
        _FogNoiseFactor("Noise Factor", Range(0, 10)) = 0.1
        
        _FogHeight("Height Y", float) = 0
        _FogHeightTransition("Height Exp", Range(0.001, 100)) = 1
    }
    SubShader
    {
        // No culling or depth
        // Cull Off ZWrite Off ZTest Always
        
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"


            float _MaxRange;
            float _Interval;
            float _FogDensity;
            float4 _FogColor;
            float _BandingNoiseIntensity;

            TEXTURE3D(_FogNoiseTex);
            float _FogNoiseTile;
            float _FogNoiseFactor;
            float _FogHeight;
            float _FogHeightTransition;

            float4 frag(Varyings IN) : SV_Target
            {
                Texture2D inputTexture = _BlitTexture;
                float4 inputColor = SAMPLE_TEXTURE2D(inputTexture, sampler_LinearClamp, IN.texcoord);
                float depth = SampleSceneDepth(IN.texcoord);
                float3 position = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);
                
                float3 dir = position -_WorldSpaceCameraPos;
                float3 dist = length(dir);
                dir = normalize(dir);
                float targetDistance = min(dist, _MaxRange);

                float finalColorFactor = 1;

                float currentDist = InterleavedGradientNoise(IN.texcoord * _BlitTexture_TexelSize.zw, (int)(_Time.y / max(HALF_EPS, unity_DeltaTime.x))) * _BandingNoiseIntensity;
                while(currentDist < targetDistance){
                    float3 currentPosition = _WorldSpaceCameraPos + dir * currentDist;
                    float4 noiseValue = _FogNoiseTex.SampleLevel(sampler_TrilinearRepeat, (currentPosition * 0.01 * _FogNoiseTile) + _Time * 0.01, 0);
                    // float noiseDensity = saturate(dot(noiseValue, noiseValue) - _FogNoiseFactor) * _FogDensity * lerp(1, 0, pow(currentPosition.y - _FogHeight, _FogHeightExp) );
                    float noiseDensity = saturate(dot(noiseValue, noiseValue) - _FogNoiseFactor) * _FogDensity * lerp(1, 0, (currentPosition.y - _FogHeight) / _FogHeightTransition);
                    if(noiseDensity > 0){
                        finalColorFactor *= exp(-noiseDensity * _Interval);

                    }
                    
                    // if(final)
                    // finalColor += _FogDensity ;
                    
                    currentDist+=_Interval;
                }


                // return float4(dir/2, 1);
                // return float4(frac(position), 1);
                return lerp(inputColor,(_FogColor), (1-saturate(finalColorFactor)));
            }
            ENDHLSL
        }
    }
}
