Shader "AkilliMum/SRP/PostProcessing/Glitch"
{
    Properties
    {
        _MainTex("(RGB)", 2D) = "" {}
        _Noise("Noise", 2D) = "" {}
        _Size("Size", float) = 16
        _BlockDensity("Block Density", float) = 0.2
        _LineDensity("Line Density", float) = 0.7
        _Speed("Speed", float) = 10
                        [Toggle]_IsScreenSpace("Is Screen Space", float) = 1

    }

        SubShader
        {
            // With SRP we introduce a new "RenderPipeline" tag in Subshader. This allows to create shaders
            // that can match multiple render pipelines. If a RenderPipeline tag is not set it will match
            // any render pipeline. In case you want your subshader to only run in LWRP set the tag to
            // "UniversalRenderPipeline"
            Tags{"RenderType" = "Transparent" "RenderPipeline" = "UniversalRenderPipeline" "IgnoreProjector" = "True"}
            LOD 300

            // ------------------------------------------------------------------
            // Forward pass. Shades GI, emission, fog and all lights in a single pass.
            // Compared to Builtin pipeline forward renderer, LWRP forward renderer will
            // render a scene with multiple lights with less drawcalls and less overdraw.
            Pass
            {
                // "Lightmode" tag must be "UniversalForward" or not be defined in order for
                // to render objects.
                Name "StandardLit"
                //Tags{"LightMode" = "UniversalForward"}

                //Blend[_SrcBlend][_DstBlend]
                //ZWrite Off ZTest Always
                //ZWrite[_ZWrite]
                //Cull[_Cull]

                HLSLPROGRAM
            // Required to compile gles 2.0 with standard SRP library
            // All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            //do not add LitInput, it has already BaseMap etc. definitions, we do not need them (manually described below)
            //#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

            float4 _MainTex_ST;
            TEXTURE2D(_MainTex);       SAMPLER(sampler_MainTex);
            float4 _Noise_ST;
            TEXTURE2D(_Noise);       SAMPLER(sampler_Noise);
           
            float _Size;
            float _BlockDensity;
            float _LineDensity;
            float _Speed;
            float _IsScreenSpace;


            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv                       : TEXCOORD0;
                float4 positionCS               : SV_POSITION;
                float4 screenPos                : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings LitPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                // VertexPositionInputs contains position in multiple spaces (world, view, homogeneous clip space)
                // Our compiler will strip all unused references (say you don't use view space).
                // Therefore there is more flexibility at no additional cost with this struct.
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

                // TRANSFORM_TEX is the same as the old shader library.
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                // We just use the homogeneous clip position from the vertex input
                output.positionCS = vertexInput.positionCS;
                output.screenPos = ComputeScreenPos(vertexInput.positionCS);
                return output;
            }

            #define FLT_MAX 3.402823466e+38
            #define FLT_MIN 1.175494351e-38
            #define DBL_MAX 1.7976931348623158e+308
            #define DBL_MIN 2.2250738585072014e-308

             #define iTimeDelta unity_DeltaTime.x
            // float;

            #define iFrame ((int)(_Time.y / iTimeDelta))
            // int;

           #define clamp(x,minVal,maxVal) min(max(x, minVal), maxVal)

           float mod(float a, float b)
           {
               return a - floor(a / b) * b;
           }
           float2 mod(float2 a, float2 b)
           {
               return a - floor(a / b) * b;
           }
           float3 mod(float3 a, float3 b)
           {
               return a - floor(a / b) * b;
           }
           float4 mod(float4 a, float4 b)
           {
               return a - floor(a / b) * b;
           }

           float3 makeDarker(float3 item) {
               return item *= 0.90;
           }

           float4 pointSampleTex2D(Texture2D sam, SamplerState samp, float2 uv)//, float4 st) st is aactually screenparam because we use screenspace
           {
               //float2 snappedUV = ((float2)((int2)(uv * st.zw + float2(1, 1))) - float2(0.5, 0.5)) * st.xy;
               float2 snappedUV = ((float2)((int2)(uv * _ScreenParams.zw + float2(1, 1))) - float2(0.5, 0.5)) * _ScreenParams.xy;
               return  SAMPLE_TEXTURE2D(sam, samp, float4(snappedUV.x, snappedUV.y, 0, 0));
           }

           half4 LitPassFragment(Varyings input) : SV_Target{
UNITY_SETUP_INSTANCE_ID(input);
UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
 half4 fragColor = half4 (1 , 1 , 1 , 1);
 float2 fragCoord = ((input.screenPos.xy) / (input.screenPos.w + FLT_MIN)) * _ScreenParams.xy;
      float2 uv = fragCoord.xy / _ScreenParams.xy;
      if (_IsScreenSpace < 0.5)
          uv = input.uv;

      float2 block = floor(fragCoord.xy / float2 (_Size , _Size));
      float2 uv_noise = block / float2 (64 , 64);
      uv_noise += floor(float2 (_Time.y, _Time.y) * float2 (1234.0 , 3543.0)) / float2 (64, 64);

      float block_thresh = pow(frac(_Time.y * 1236.0453) , 2.0) * _BlockDensity;
      float line_thresh = pow(frac(_Time.y * 2236.0453) , 3.0) * _LineDensity;

      float2 uv_r = uv , uv_g = uv , uv_b = uv;

      if (SAMPLE_TEXTURE2D(_Noise , sampler_Noise , uv_noise).r < block_thresh ||
           SAMPLE_TEXTURE2D(_Noise , sampler_Noise , float2 (uv_noise.y , 0.0)).g < line_thresh) {

           float2 dist = (frac(uv_noise) - 0.5) * 0.03 * _Speed;
           uv_r += dist * 0.1;
           uv_g += dist * 0.2;
           uv_b += dist * 0.125;
       }

      fragColor.r = SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , uv_r).r;
      fragColor.g = SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , uv_g).g;
      fragColor.b = SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , uv_b).b;

      if (SAMPLE_TEXTURE2D(_Noise , sampler_Noise , uv_noise).g < block_thresh)
           fragColor.rgb = fragColor.ggg;

      if (SAMPLE_TEXTURE2D(_Noise , sampler_Noise , float2 (uv_noise.y , 0.0)).b * 3.5 < line_thresh)
           fragColor.rgb = float3 (0.0 , dot(fragColor.rgb , float3 (1.0 , 1.0 , 1.0)) , 0.0);

      if (SAMPLE_TEXTURE2D(_Noise , sampler_Noise , uv_noise).g * 1.5 < block_thresh ||
           SAMPLE_TEXTURE2D(_Noise , sampler_Noise , float2 (uv_noise.y , 0.0)).g * 2.5 < line_thresh) {
           float lines = frac(fragCoord.y / 3.0);
           float3 mask = float3 (3.0 , 0.0 , 0.0);
           if (lines > 0.333)
                mask = float3 (0.0 , 3.0 , 0.0);
           if (lines > 0.666)
                mask = float3 (0.0 , 0.0 , 3.0);

           fragColor.xyz *= mask;
       }
  return fragColor;
           }

               //half4 LitPassFragment(Varyings input) : SV_Target
               //{
               //    [FRAGMENT]
               //    //float2 uv = input.uv;
               //    //SAMPLE_TEXTURE2D_LOD(_BaseMap, sampler_BaseMap, uv + float2(-onePixelX, -onePixelY), _Lod);
               //    //_ScreenParams.xy 
               //    //half4 color = half4(1, 1, 1, 1);
               //    //return color;
               //}
               ENDHLSL
                }
            }
        }