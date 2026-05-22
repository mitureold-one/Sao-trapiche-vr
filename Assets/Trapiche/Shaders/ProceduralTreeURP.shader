Shader "Nature/Procedural Tree URP"
{
    Properties
    {
        _BarkMap("Bark Texture", 2D) = "white" {}
        _BarkColor("Bark Color", Color) = (1,1,1,1)
        _BarkSmoothness("Bark Smoothness", Range(0,1)) = 0.12
        _LeafMap("Leaf Texture", 2D) = "white" {}
        _LeafColor("Leaf Color", Color) = (1,1,1,1)
        _LeafBottomTint("Leaf Bottom Tint", Color) = (0.24,0.42,0.20,1)
        _LeafTopTint("Leaf Top Tint", Color) = (0.58,0.82,0.34,1)
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.4
        _LeafTranslucency("Leaf Translucency", Range(0,2)) = 0.4
        _LeafSmoothness("Leaf Smoothness", Range(0,1)) = 0.05
        _WindStrength("Wind Strength", Range(0,0.5)) = 0.025
        _WindSpeed("Wind Speed", Range(0,10)) = 1.6
        _WindScale("Wind Scale", Range(0.1,10)) = 1.8
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="AlphaTest" "RenderType"="TransparentCutout" }
        Cull Off
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BarkMap); SAMPLER(sampler_BarkMap);
            TEXTURE2D(_LeafMap); SAMPLER(sampler_LeafMap);
            CBUFFER_START(UnityPerMaterial)
                float4 _BarkMap_ST; float4 _LeafMap_ST;
                half4 _BarkColor; half4 _LeafColor;
                half4 _LeafBottomTint; half4 _LeafTopTint;
                half _BarkSmoothness; half _LeafSmoothness;
                half _LeafTranslucency; half _Cutoff;
                half _WindStrength; half _WindSpeed; half _WindScale;
            CBUFFER_END

            struct Attr { float4 pos:POSITION; float3 norm:NORMAL; float2 uv0:TEXCOORD0; float2 uv1:TEXCOORD1; float4 col:COLOR; };
            struct Vary { float4 cs:SV_POSITION; float2 uvB:TEXCOORD0; float2 uvL:TEXCOORD1; float4 vc:TEXCOORD2; float3 ws:TEXCOORD3; float3 nw:TEXCOORD4; float fog:TEXCOORD5; };

            Vary vert(Attr v)
            {
                Vary o;
                half lm = saturate(v.col.a);
                half iw = saturate(v.col.b);
                half lf = saturate(v.uv0.y) * lm;
                float t = _Time.y * _WindSpeed;
                float wA = sin((v.pos.x+v.pos.z)*_WindScale+t);
                float wB = cos(v.pos.z*(_WindScale*1.37)+t*1.19);
                float wC = sin(v.pos.y*(_WindScale*0.73)+t*0.81);
                float sw = (wA+wB+wC)*0.33333334;
                v.pos.x += sw*(_WindStrength*0.55)*iw + sw*_WindStrength*lf;
                v.pos.z += wA*(_WindStrength*0.28)*iw + wA*(_WindStrength*0.45)*lf;
                v.pos.y += wB*(_WindStrength*0.08)*iw + wB*(_WindStrength*0.18)*lf;
                VertexPositionInputs pi = GetVertexPositionInputs(v.pos.xyz);
                VertexNormalInputs   ni = GetVertexNormalInputs(v.norm);
                o.cs  = pi.positionCS; o.ws = pi.positionWS; o.nw = ni.normalWS;
                o.uvB = TRANSFORM_TEX(v.uv0,_BarkMap);
                o.uvL = TRANSFORM_TEX(v.uv1,_LeafMap);
                o.vc  = v.col;
                o.fog = ComputeFogFactor(pi.positionCS.z);
                return o;
            }

            half4 frag(Vary i, half face:VFACE) : SV_Target
            {
                half lm = saturate(i.vc.a);
                half4 bk = SAMPLE_TEXTURE2D(_BarkMap,sampler_BarkMap,i.uvB)*_BarkColor;
                half4 lf = SAMPLE_TEXTURE2D(_LeafMap,sampler_LeafMap,i.uvL)*_LeafColor;
                half  al = lerp(1.0h, lf.a, lm);
                clip(al - _Cutoff);
                half3 grad = lerp(_LeafBottomTint.rgb,_LeafTopTint.rgb,saturate(i.uvL.y));
                half  var  = lerp(0.9h,1.1h,saturate(i.vc.r));
                half3 lAlb = lf.rgb*grad*var;
                half3 alb  = lerp(bk.rgb, lAlb, lm);
                half  smt  = lerp(_BarkSmoothness,_LeafSmoothness,lm);
                InputData ld = (InputData)0;
                ld.positionWS = i.ws;
                ld.normalWS   = normalize(i.nw)*(face>=0?1:-1);
                ld.viewDirectionWS = GetWorldSpaceNormalizeViewDir(i.ws);
                ld.shadowCoord = TransformWorldToShadowCoord(i.ws);
                ld.fogCoord    = i.fog;
                ld.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.cs);
                SurfaceData sd = (SurfaceData)0;
                sd.albedo=alb; sd.smoothness=smt; sd.occlusion=1; sd.alpha=al;
                Light ml = GetMainLight();
                half bl = saturate(dot(-ld.normalWS, ml.direction));
                sd.emission = lAlb*bl*_LeafTranslucency*lm;
                half4 col = UniversalFragmentPBR(ld,sd);
                col.rgb = MixFog(col.rgb,i.fog);
                return col;
            }
            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }
            Cull Off
            HLSLPROGRAM
            #pragma vertex vs
            #pragma fragment fs
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            TEXTURE2D(_LeafMap); SAMPLER(sampler_LeafMap);
            CBUFFER_START(UnityPerMaterial)
                float4 _BarkMap_ST; float4 _LeafMap_ST;
                half4 _BarkColor; half4 _LeafColor; half4 _LeafBottomTint; half4 _LeafTopTint;
                half _BarkSmoothness; half _LeafSmoothness; half _LeafTranslucency;
                half _Cutoff; half _WindStrength; half _WindSpeed; half _WindScale;
            CBUFFER_END
            struct A { float4 p:POSITION; float3 n:NORMAL; float2 u:TEXCOORD1; float4 c:COLOR; };
            struct V { float4 p:SV_POSITION; float2 u:TEXCOORD0; half m:TEXCOORD1; };
            V vs(A v) { V o; o.u=TRANSFORM_TEX(v.u,_LeafMap); o.m=saturate(v.c.a); float3 ws=TransformObjectToWorld(v.p.xyz); float3 ns=TransformObjectToWorldNormal(v.n); o.p=TransformWorldToHClip(ApplyShadowBias(ws,ns,_MainLightPosition.xyz)); return o; }
            half4 fs(V i):SV_Target { clip(lerp(1.0h,SAMPLE_TEXTURE2D(_LeafMap,sampler_LeafMap,i.u).a,i.m)-_Cutoff); return 0; }
            ENDHLSL
        }
    }
}
