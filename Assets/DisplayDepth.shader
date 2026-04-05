Shader "Clouds/DepthDisplay"
{
    Properties
    {

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

            // #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            // sampler2D _CameraDepthTexture;

            float4 frag(Varyings IN) : SV_Target
            {
                Texture2D inputTexture = _BlitTexture;
                float depth = SampleSceneDepth(IN.texcoord);
                return depth;
            }
            ENDHLSL
        }
    }
}
