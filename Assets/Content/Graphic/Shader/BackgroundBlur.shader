Shader "CustomRenderTexture/BackgroundBlur"
{
    Properties
    {
        _BlurSize ("Blur Size", Float) = 1
        _Darken ("Darken", Float) = 0.15
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
        }

        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "HorizontalBlur"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragHorizontal

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D_X(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            float4 _BlitTexture_TexelSize;
            float _BlurSize;
            float _Darken;

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;

                output.positionHCS = GetFullScreenTriangleVertexPosition(input.vertexID);
                output.uv = GetFullScreenTriangleTexCoord(input.vertexID);

                return output;
            }

            half4 FragHorizontal(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                float2 offset = float2(_BlitTexture_TexelSize.x * _BlurSize, 0);

                half4 col = 0;

                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv - offset * 4) * 0.0162162162;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv - offset * 3) * 0.0540540541;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv - offset * 2) * 0.1216216216;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv - offset * 1) * 0.1945945946;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv)            * 0.2270270270;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv + offset * 1) * 0.1945945946;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv + offset * 2) * 0.1216216216;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv + offset * 3) * 0.0540540541;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv + offset * 4) * 0.0162162162;

                return col;
            }
            ENDHLSL
        }

        Pass
        {
            Name "VerticalBlur"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FragVertical

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D_X(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            float4 _BlitTexture_TexelSize;
            float _BlurSize;
            float _Darken;

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;

                output.positionHCS = GetFullScreenTriangleVertexPosition(input.vertexID);
                output.uv = GetFullScreenTriangleTexCoord(input.vertexID);

                return output;
            }

            half4 FragVertical(Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                float2 offset = float2(0, _BlitTexture_TexelSize.y * _BlurSize);

                half4 col = 0;

                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv - offset * 4) * 0.0162162162;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv - offset * 3) * 0.0540540541;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv - offset * 2) * 0.1216216216;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv - offset * 1) * 0.1945945946;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv)            * 0.2270270270;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv + offset * 1) * 0.1945945946;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv + offset * 2) * 0.1216216216;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv + offset * 3) * 0.0540540541;
                col += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv + offset * 4) * 0.0162162162;

                col.rgb *= 1.0 - _Darken;

                return col;
            }
            ENDHLSL
        }
    }
}
