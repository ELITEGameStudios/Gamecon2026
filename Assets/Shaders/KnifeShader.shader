Shader "Knife/WeaponShaders"
{
    Properties
    {
        _offset("Offset", Vector) = (0, 0, 0, 0) 
        _mainCol("Main Color", Color) = (0, 1, 1, 1)
        _scaleFactor("Border scale", float) = 1.2
    }

    SubShader
    {
        Tags{"RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}
        LOD 100

        Pass
        {
            Name "Unlit"
            Tags{"LightMode"="UniversalForward"}

            /* --------------- Beginning of HLSL script ------------------- */
            HLSLPROGRAM
            #pragma vertex vertexShader
            #pragma fragment fragmentShader
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _mainCol;
            float4 _offset;
            float _scaleFactor = 1.5;
            
            // Essential Universal Structs
            struct Attributes{
                float4 objectPosition : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct Varyings{ 
                float4 clipSpacePosition : SV_POSITION;
                // float2 uv;
            };
            




            Varyings vertexShader(Attributes data){
                Varyings varyings;
                // varyings.objectPosition = data.objectPosition;
                _offset.x*= _SinTime.w;
                _offset.y*= _SinTime.w;
                varyings.clipSpacePosition = TransformObjectToHClip(data.objectPosition.xyz) + TransformWViewToHClip(_offset);
                varyings.clipSpacePosition *= _scaleFactor;
                // varyings.uv = data.uv;
                return varyings;
            }

            float4 fragmentShader(Varyings data) : SV_TARGET{
                return _mainCol;
            }

            ENDHLSL
            /* --------------- End of of HLSL script ------------------- */
        }
    }
    FallBack Off
}