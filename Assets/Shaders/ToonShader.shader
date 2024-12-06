Shader "Custom/ToonShader"
{
    Properties
    {
        [Header(Base Parameters)]
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Specular ("Specular Color", Color) = (1,1,1,1)
        [HDR] _Emission ("Emission", Color) = (0,0,0,1)
        
        [Header(Lighting Parameters)]
        _ShadowTint("Shadow Color", Color) = (0.5,0.5,0.5,1)
        
        [IntRange] _StepAmount ("Shadow Steps", Range(1,16)) = 2
        _StepWidth("Step Size", Range(0,1)) = 0.25
        
        _SpecularSize ("Specular Size", Range(0,1)) = 0.1
        _SpecularFalloff ("Specular Falloff", Range(0,2)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Stepped fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;
        half3 _Emission;
        fixed4 _Specular;
 
        float3 _ShadowTint;
        float _StepWidth;
        float _StepAmount;
        float _SpecularSize;
        float _SpecularFalloff;

        struct ToonSurfaceOutput
        {
            fixed3 Albedo;
            half3 Emission;
            fixed3 Specular;
            fixed Alpha;
            fixed3 Normal;
        };

        float4 LightingStepped(ToonSurfaceOutput s, float3 lightDir, half3 viewDir, float shadowAttenuation)
        {
            float towardsLight = dot(s.Normal, lightDir);
            towardsLight = towardsLight / _StepWidth;
            float LightIntensity = floor(towardsLight);

            float change = fwidth(towardsLight);
            float smoothing = smoothstep(0,change, frac(towardsLight));
            LightIntensity += smoothing;

            LightIntensity = LightIntensity/ _StepAmount;
            LightIntensity = saturate(LightIntensity);

            #ifdef USING_DIRECTIONAL_LIGHT
                 float attenutionChange = fwidth(shadowAttenuation) * 0.5;
                 float shadow = smoothstep(0.5 - attenutionChange, 0.5 + attenutionChange, shadowAttenuation);
            #else
                 float attenutionChange = fwidth(shadowAttenuation);
                 float shadow = smoothstep(0, attenutionChange, shadowAttenuation);
            #endif

            LightIntensity *= shadow;

            float3 reflectionDirection = reflect(lightDir, s.Normal);
            float towardsReflection = dot(viewDir, -reflectionDirection);

            float specularFallOff = dot(viewDir, s.Normal);
            specularFallOff = pow(specularFallOff, _SpecularFalloff);
            towardsReflection *= specularFallOff;

            float speecularChange = fwidth(towardsReflection);
            float specularIntensity = smoothstep(1 - _SpecularSize,1 + speecularChange, towardsReflection);

            specularIntensity *= shadow;

            float4 colour;
            colour.rgb = s.Albedo * LightIntensity * _LightColor0.rgb;
            colour.rgb = lerp(colour.rgb, s.Specular * _LightColor0.rgb, saturate(specularIntensity));
            colour.a = s.Alpha;
            return colour;
            
            //towardsReflection
            //speecularChange
            //return float4(specularIntensity.xxx,1);
        }

        void surf (Input IN, inout ToonSurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Specular = _Specular;
            o.Alpha = c.a;
            float3 shadowColor = c.rgb *_ShadowTint;
            o.Emission = _Emission + shadowColor;
        }
        ENDCG
    }
    FallBack "Standard"
}
