Shader "AkilliMum/SRP/PostProcessing/EdgeGlow"
{
    Properties
    {
        _MainTex("(RGB)", 2D) = "" {}
        _Scale("Scale", float) = 0.5
        [HDR]_EdgeColor("Color", Color) = (1,1,1,1)
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
            
            float _Scale;
            float4 _EdgeColor;
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

float lookup(float2 p , float dx , float dy)
 {
    float2 uv = (p.xy + float2 (dx * _Scale , dy * _Scale)) / _ScreenParams.xy;
    float4 c = SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , uv.xy);

    return 0.2126 * c.r + 0.7152 * c.g + 0.0722 * c.b;
 }

half4 LitPassFragment(Varyings input) : SV_Target  {
UNITY_SETUP_INSTANCE_ID(input);
UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
 half4 fragColor = half4 (1 , 1 , 1 , 1);
 float2 fragCoord = ((input.screenPos.xy) / (input.screenPos.w + FLT_MIN)) * _ScreenParams.xy;
     float2 p = fragCoord.xy;
     if (_IsScreenSpace < 0.5)
         p = input.uv * _ScreenParams.xy;

     float gx = 0.0;
     gx += -1.0 * lookup(p , -1.0 , -1.0);
     gx += -2.0 * lookup(p , -1.0 , 0.0);
     gx += -1.0 * lookup(p , -1.0 , 1.0);
     gx += 1.0 * lookup(p , 1.0 , -1.0);
     gx += 2.0 * lookup(p , 1.0 , 0.0);
     gx += 1.0 * lookup(p , 1.0 , 1.0);

     float gy = 0.0;
     gy += -1.0 * lookup(p , -1.0 , -1.0);
     gy += -2.0 * lookup(p , 0.0 , -1.0);
     gy += -1.0 * lookup(p , 1.0 , -1.0);
     gy += 1.0 * lookup(p , -1.0 , 1.0);
     gy += 2.0 * lookup(p , 0.0 , 1.0);
     gy += 1.0 * lookup(p , 1.0 , 1.0);

     float g = gx + gy;
     //float g2 = g * (sin(_Time.y) / 2.0 + 0.5);

     float4 col = SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , p / _ScreenParams.xy);
     col = lerp(col, _EdgeColor, g);
     //col *= g; // float4 (0.0, g, g2, 1.0);
     col = _EdgeColor * g;

     fragColor = col;
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