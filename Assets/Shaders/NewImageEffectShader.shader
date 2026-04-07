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
        _ShadowmapCamPos("Shadowmap Cam Pos", Vector) = (0, 0, 0)
        _OrthoSize("Orphographic Size", float) = 500
        _FogNoiseTex("Noise texture", 3D) = "white" {}
        _FogNoiseTile("Noise Tiling", float) = 1
        _FogNoiseSpeed("Noise Speed", float) = 1
        _FogNoiseFactor("Noise Factor", Range(0, 10)) = 0.1
        _FogLerp("Noise Lerp", Range(0, 1)) = 0


        [Toggle] _ShadowmapUse("Use shadowmap", Float) = 0
        [Toggle] _SetShadow("Test Volumetric Shadows", Float) = 0
        [Toggle] _SetEnabled("Enabled", Float) = 1
        // _ShadowmapMatrix("_ShadowmapMatrix", Matrix)
        
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
            float _FogNoiseSpeed;
            float _FogHeight;
            float _FogFloor;
            float _FogHeightTransition;
            float _FogMaxDensity;
            float _FogLerp;
            float _OrthoSize;
            
            TEXTURE2D(_ShadowmapTex);
            float _ShadowmapUse;
            float _SetShadow;
            float _SetEnabled;

            float4x4 _ShadowmapMatrix;
            float3 _ShadowmapCamPos;

            // sampler2D _CameraDepthTexture;

            float4 frag(Varyings IN) : SV_Target
            {
                Texture2D inputTexture = _BlitTexture;
                float4 inputColor = SAMPLE_TEXTURE2D(inputTexture, sampler_LinearClamp, IN.texcoord);
                if(_SetEnabled == 0){return inputColor;}

                float depth = SampleSceneDepth(IN.texcoord);
                float3 position = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);
                
                float3 dir = position -_WorldSpaceCameraPos;
                float3 dist = length(dir);
                dir = normalize(dir);
                float targetDistance = min(dist, _MaxRange);

                float finalColorFactor = 1;
                float density = _FogDensity * 0.01;
                float4 finalColor = _FogColor;
                // float4 finalColor = _FogColor;

                float currentDist = _MinRange + InterleavedGradientNoise(IN.texcoord * _BlitTexture_TexelSize.zw, (int)(_Time.y / max(HALF_EPS, unity_DeltaTime.x))) * _BandingNoiseIntensity;
                
                // [unroll(500)]
                [loop]
                while(currentDist < targetDistance){
                    float3 currentPosition = _WorldSpaceCameraPos + dir * currentDist;
                    
                    float4 noiseValue = _FogNoiseTex.SampleLevel(sampler_TrilinearRepeat, (currentPosition * 0.01 * _FogNoiseTile) + _Time * 0.01 * _FogNoiseSpeed, 0);
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
                        float shadowAttenuation = 1;
                        float3 colorTst = float3(0, 0, 0);
                        if(_ShadowmapUse == 1){
                            float3 shadowLocalPos = currentPosition; 
                            shadowLocalPos -= _ShadowmapCamPos;
                            shadowLocalPos = mul(shadowLocalPos, _ShadowmapMatrix);

                            float2 shadowTexCoords = 
                                float2( 
                                    shadowLocalPos.x/_OrthoSize + 0.5,
                                    shadowLocalPos.y/_OrthoSize + 0.5 
                                );

                                // colorTst = float3(1, 0, 0);
                                
                            colorTst = clamp(shadowLocalPos.z/350, 0, 1);
                            float inputDepth = shadowLocalPos.z/350;
                            // shadowAttenuation = saturate(shadowLocalPos.x/_OrthoSize + 0.5);
                            float shadowDepthIn = SAMPLE_TEXTURE2D(_ShadowmapTex, sampler_LinearClamp, shadowTexCoords).r;

                            // shadowAttenuation = 1-shadowDepthIn;
                            // if(shadowDepthIn < inputDepth){
                            //     shadowAttenuation = 0;
                            // }
                            // shadowAttenuation = shadowDepthIn - inputDepth;
                            // shadowAttenuation = ;
                            // shadowAttenuation = inputDepth - shadowDepthIn;
                            // shadowAttenuation = shadowDepthIn;
                            if(1-shadowDepthIn < inputDepth ){
                                // continue;
                                shadowAttenuation = 0;
                            }
                        }
                        
                        if(_SetShadow == 1){
                            inputDensity = inputDensity - (shadowAttenuation * inputDensity);
                        }
                        // half shadow = AdditionalLightRealtimeShadow(0, currentPosition);
                        // finalColor.rgb += colorTst.rgb * inputDensity * _Interval;
                        
                        if(_ShadowmapUse == 1){
                            finalColor.rgb += _LightEffectColor.rgb * inputDensity * _Interval * shadowAttenuation;
                        }
                        else{
                            Light mainLight = GetAdditionalLight(0, currentPosition);
                            finalColor.rgb += mainLight.color.rgb * _LightEffectColor.rgb * inputDensity * _Interval * shadowAttenuation * mainLight.distanceAttenuation;
                        }
                        finalColorFactor *= exp(-inputDensity);
                        // finalColor.rgb = colorTst.rgb;
                        // Light mainLight = GetMainLight(TransformWorldToShadowCoord(currentPosition));
                        // break;
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
