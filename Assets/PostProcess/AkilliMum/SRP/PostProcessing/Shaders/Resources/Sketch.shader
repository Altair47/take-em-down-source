Shader "AkilliMum/SRP/PostProcessing/Sketch"
{
    Properties
    {
        _MainTex("(RGB)", 2D) = "" {}
        _Noise("Noise", 2D) = "" {}
        [HideInInspector] _RANGE("", float) = 16.
        [HideInInspector] _STEP("", float) = 2.
        [HideInInspector] _ANGLE("", float) = 1.65
        [HideInInspector] _THRESHOLD("", float) = 0.01
        _SENSITIVITY("Sensitivity", float) = 1.
        _COLOR("Color", float) = 1.
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
            
            float _RANGE;
            float _STEP;
            float _ANGLE;
            float _THRESHOLD;
            float _SENSITIVITY;
            float _COLOR;
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

#define PI2 6.28318530717959 

float4 getCol(float2 pos)
 {
    float2 uv = pos / _ScreenParams.xy;
    return SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , uv);
 }

float getVal(float2 pos)
 {
    float4 c = getCol(pos);
    return dot(c.xyz , float3 (0.2126 , 0.7152 , 0.0722));
 }

float2 getGrad(float2 pos , float eps)
 {
        float2 d = float2 (eps , 0);
    return float2 (
        getVal(pos + d.xy) - getVal(pos - d.xy) ,
        getVal(pos + d.yx) - getVal(pos - d.yx)
     ) / eps / 2.;
 }

void pR(inout float2 p , float a) {
     p = cos(a) * p + sin(a) * float2 (p.y , -p.x);
 }
float absCircular(float t)
 {
    float a = floor(t + 0.5);
    return mod(abs(a - t) , 1.0);
 }

half4 LitPassFragment(Varyings input) : SV_Target  {
UNITY_SETUP_INSTANCE_ID(input);
UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
 half4 fragColor = half4 (1 , 1 , 1 , 1);
 float2 fragCoord = ((input.screenPos.xy) / (input.screenPos.w + FLT_MIN)) * _ScreenParams.xy;
    if (_IsScreenSpace < 0.5)
        fragCoord = input.uv * _ScreenParams.xy;

     float2 pos = fragCoord;
     float weight = 1.0;

     //[loop]
     [unroll(4)]
     for (float j = 0.; j < _ANGLE; j += 1.)
      {
         float2 dir = float2 (1 , 0);
         pR(dir , j * PI2 / (2. * _ANGLE));

         float2 grad = float2 (-dir.y , dir.x);
         //[loop]
         [unroll(16)]
         for (float i = -_RANGE; i <= _RANGE; i += _STEP)
          {
             float2 pos2 = pos + normalize(dir) * i;

             if (pos2.y < 0. || pos2.x < 0. || pos2.x > _ScreenParams.x || pos2.y > _ScreenParams.y)
                 continue;

             float2 g = getGrad(pos2 , 1.);
             if (length(g) < _THRESHOLD)
                 continue;

             weight -= pow(abs(dot(normalize(grad) , normalize(g))) , _SENSITIVITY) / floor((2. * _RANGE + 1.) / _STEP) / _ANGLE;
          }
      }

 #ifndef GRAYSCALE 
     float4 col = getCol(pos);
 #else 
     float4 col = float4 (getVal(pos));
 #endif 

     float4 background = lerp(col , float4 (1 , 1 , 1 , 1) , _COLOR);

     /* float distToLine = absCircular ( fragCoord.y / ( _ScreenParams.y / 8. ) ) ;
    background = lerp ( float4 ( 0.6 , 0.6 , 1 , 1 ) , background , smoothstep ( 0. , 0.03 , distToLine ) ) ; */


    float r = length(pos - _ScreenParams.xy * .5) / _ScreenParams.x;
    float vign = 1. - r * r * r;

    float4 a = SAMPLE_TEXTURE2D(_Noise , sampler_Noise , pos / _ScreenParams.xy);

    fragColor = vign * lerp(float4 (0 , 0 , 0 , 0) , background , weight) + a.xxxx / 25.;
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