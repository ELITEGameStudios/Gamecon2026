Shader "Test/Lambert"
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
        _FogNoiseTex("Noise texture", 3D) = "white" {}
        _FogNoiseTile("Noise Tiling", float) = 1
        _FogNoiseFactor("Noise Factor", Range(0, 10)) = 0.1
        _FogLerp("Noise Lerp", Range(0, 1)) = 0
        
        _FogHeight("Height Y", float) = 0
        _FogHeightTransition("Height Exp", Range(1, 100)) = 1
        [HDR]_LightEffectColor("Light Factor", Color) = (1, 1, 1, 1)
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
            // #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"


            float4 frag(Varyings IN) : SV_Target
            {
                Light mainLight = GetMainLight();
                return float4(mainLight.direction.rgb, 1);
            }
            ENDHLSL
        }
    }
}
