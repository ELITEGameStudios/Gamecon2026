Shader "Knife/WeaponShaders"
{
    Properties
    {
        _MainCol("Can a clanka borrow a french fry?", Color) = (0, 1, 1, 1)
    }

    SubShader
    {
        // Tags{"RenderType"="Transparent" "RenderPipeline"="UniversalPipeline"}
        LOD 100

        Pass
        {
            Name "KnifeShader"
            Tags{"LightMode"="UniversalForward"}

            /* --------------- Beginning of HLSL script ------------------- */
            HLSLPROGRAM
            #pragma vertex vertexShader
            #pragma fragment fragmentShader
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 color;
            

            // Supporting Structs 
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                // float2 uv : TEXCOORD0;
            };
            
            // Variables
            float4 myColor;

            Varyings vertexShader(Varyings IN) : SV_TARGET{
                Varyings output;
                output.positionHCS = TransformWorldToHClip(attributes.positionOS.xyz);
                return output;
            }

            float4 fragmentShader() : SV_TARGET{
                return color;
            }

            ENDHLSL
            /* --------------- End of of HLSL script ------------------- */
        }
    }
}