Shader "Unlit/AnothaTest"
{
    Properties
    {
        _MainTex("Main Texture", Texture2D) = "white"
        _BorderColor("Border Color", Color) = (1, 1, 1, 1)
        _BorderWidth("Border Width", Range(0, 1)) = 1
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
            float _FogHeightTransition;
            float _FogMaxDensity;
            float _FogLerp;

            struct Attributes{
                float3 positionOS : POSITION;
                half4 UV : TEXCOORD0;
            }

            struct Varyings{
                float3 positionWS : SV_POSITION;
                half4 UV;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                Texture2D inputTexture = _BlitTexture;
                float4 inputColor = SAMPLE_TEXTURE2D(inputTexture, sampler_LinearClamp, IN.texcoord);
                float depth = SampleSceneDepth(IN.texcoord);
                float3 position = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);
                
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                Texture2D inputTexture = _BlitTexture;
                float4 inputColor = SAMPLE_TEXTURE2D(inputTexture, sampler_LinearClamp, IN.texcoord);
                float depth = SampleSceneDepth(IN.texcoord);
                float3 position = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);
                
                return inputColor;
            }
            ENDHLSL
        }
    }
}
