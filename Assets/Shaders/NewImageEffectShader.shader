Shader "Clouds/MainCloudShader"
{
    Properties
    {
        _MaxRange("Max Range", float) = 25
        _MinRange("Minimum Range", float) = 0
        _Interval("Raymarch Interval Length", Range(0.1, 10)) = 0.1
        _BandingNoiseIntensity("Banding Noise Intensity", float) = 1
        
        _FogDensity("Additive Fog Per Step", Range(0, 100)) = 0.005
        _FogMaxDensity("Max Fog Density", Range(0, 1)) = 1
        _FogColor("Additive Fog color", Color) = (1, 1, 1, 1)
        _ShadowmapTex("Shadowmap Render Texture", 2D) = "white" {}
        _FogNoiseTex("Noise texture", 3D) = "white" {}
        _FogNoiseTile("Noise Tiling", float) = 1
        _FogNoiseFactor("Noise Factor", Range(0, 10)) = 0.1
        _FogLerp("Noise Lerp", Range(0, 1)) = 0
        
        _FogHeight("Height Y", float) = 0
        _FogFloor("Min Y", float) = -200
        _FogHeightTransition("Height Exp", Range(1, 100)) = 1
        [HDR]_LightEffectColor("Light Factor", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

            // #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"


            float _MaxRange;
            float _MinRange;
            float _Interval;
            float _FogDensity;
            float4 _FogColor;
            float4 _LightEffectColor;
            float _BandingNoiseIntensity;

            TEXTURE3D(_FogNoiseTex);
            float _FogNoiseTile;
            float _FogNoiseFactor;
            float _FogHeight;
            float _FogFloor;
            float _FogHeightTransition;
            float _FogMaxDensity;
            float _FogLerp;

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
                float density = _FogDensity * 0.01;
                float4 finalColor = _FogColor;

                float currentDist = _MinRange + InterleavedGradientNoise(IN.texcoord * _BlitTexture_TexelSize.zw, (int)(_Time.y / max(HALF_EPS, unity_DeltaTime.x))) * _BandingNoiseIntensity;
                while(currentDist < targetDistance){
                    float3 currentPosition = _WorldSpaceCameraPos + dir * currentDist;
                    
                    
                    float4 noiseValue = _FogNoiseTex.SampleLevel(sampler_TrilinearRepeat, (currentPosition * 0.01 * _FogNoiseTile) + _Time * 0.01, 0);
                    // float noiseDensity = saturate(dot(noiseValue, noiseValue) - _FogNoiseFactor) * _FogDensity * lerp(1, 0, pow(currentPosition.y - _FogHeight, _FogHeightExp) );
                    float noiseDensity = saturate(
                        (dot(noiseValue, noiseValue) - _FogNoiseFactor) * density);
                    
                    float heightFactorA =  (currentPosition.y - _FogHeight);
                    float heightFactorB =  (_FogFloor - currentPosition.y);
                    float heightFactor;

                    if(abs(heightFactorA) > abs(heightFactorB)){
                        heightFactor = heightFactorB;
                    }
                    else{
                        heightFactor = heightFactorA;
                    }


                    float inputDensity = 
                    lerp(noiseDensity, density, _FogLerp)
                    * lerp(1, 0, heightFactor / _FogHeightTransition)
                    * _FogMaxDensity;
                        
                    if(inputDensity > 0){
                        // half shadow = AdditionalLightRealtimeShadow(0, currentPosition);
                        Light mainLight = GetAdditionalLight(0, currentPosition);
                        finalColor.rgb += mainLight.color.rgb * _LightEffectColor.rgb * inputDensity * _Interval * mainLight.shadowAttenuation * mainLight.distanceAttenuation;
                        finalColorFactor *= exp(-inputDensity);
                        // Light mainLight = GetMainLight(TransformWorldToShadowCoord(currentPosition));
                    }
                    
                    // if(final)
                    // finalColor += _FogDensity ;
                    
                    if(finalColorFactor < 1-_FogMaxDensity){
                        finalColorFactor = 1-_FogMaxDensity;
                        break;
                    }
                    
                    currentDist+=_Interval;
                }
                
                // finalColorFactor *= _FogMaxDensity;
                
                // finalColor.rgb = mainLight.color.rgb;// * _LightEffectColor.rgb * inputDensity * _Interval; 
                // return float4(dir/2, 1);
                // return float4(frac(position), 1);
                // finalColorFactor = saturate(finalColorFactor);
                // return finalColorFactor;
                // return float4(mainLight.color.rgb, 1);
                return lerp(finalColor, inputColor, finalColorFactor);
            }
            ENDHLSL
        }
    }
}
