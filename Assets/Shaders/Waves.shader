Shader "Custom/Waves"
{
    Properties
    {
        //_Speed ("Speed", Range(0,1)) = 0
        //_Amplitude ("Amplitude", Float) = 1
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _WaterFogColor ("Water Fog Color", Color) = (0,0,0,0)
        _WaterFogDensity ("Water Fog Density", Range(0,2)) = 0.1
        _RefractionStrength ("Refraction Strength", Range(0,1)) = 0.25
        
        //_Steepness ("Steepness", Range(0,1)) = 0.5
        //_Wavelength ("Wavelength", Float) = 10
        //_Direction ("Direction (2D)", Vector) = (1,0,0,0)
        _WaveA("Wave A (dir, steepness, wavelenght)", Vector) = (1,1,1,1)
        _WaveB("Wave A (dir, steepness, wavelenght)", Vector) = (1,1,1,1)
        _WaveC("Wave A (dir, steepness, wavelenght)", Vector) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        
        GrabPass {"_WaterBackground"}

        CGPROGRAM
        #pragma surface surf Standard alpha vertex:vert finalcolor:ResetAlpha //addshadow
        #pragma target 3.0

        //#include "LookingThroughWater.cginc"

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        //float _Steepness, _Wavelength;// _Speed;
        //float2 _Direction;
        float4 _WaveA, _WaveB, _WaveC;

        float3 GerstnerWave(float4 wave, float3 p, inout float3 tangent, inout float3 binormal)
        {
            float2 direction = wave.xy;
            float steepness = wave.z;
            float wavelength = wave.w;
            
            float k = UNITY_TWO_PI / wavelength;
            float c = sqrt(9.8 / k);
            float2 d = normalize(direction);
            float f = (dot(d,p.xz) - c * _Time.y) * k;
            float a = steepness/ k;
            
            //p.x += (cos(f) * a) * d.x;
            //p.y = sin(f) * a;
            //p.z += (cos(f)* a) * d.y;

            tangent += float3(  - (sin(f) * steepness) * d.x * d.x,
                                                  (cos(f) * steepness) * d.x,
                                                  (sin(f) * steepness) * -d.x * d.y);
            binormal += float3 (             (sin(f) * steepness) * -d.x * d.y,
                                                  (cos(f) * steepness) * d.y,
                                                - (sin(f) * steepness) * d.y * d.y);
            return float3(cos(f) * a * d.x,
                          sin(f) * a,
                          cos(f) * a * d.y); 
        }

        void vert(inout appdata_full vertexData)
        {
            float3 gridPoint = vertexData.vertex.xyz;
            float3 tangent = float3(1,0,0);
            float3 binormal = float3(0,0,1);
            float3 p = gridPoint;
            p += GerstnerWave(_WaveA, gridPoint, tangent, binormal);
            p += GerstnerWave(_WaveB, gridPoint, tangent, binormal);
            p += GerstnerWave(_WaveC, gridPoint, tangent, binormal);
            
            float3 normal = normalize(cross(binormal, tangent));

            vertexData.vertex.xyz = p;
            vertexData.normal = normal;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            //o.Emission = ColorBelowWater(IN.screenPos, o.Normal) * (1 - c.a);
        }

        void ResetAlpha(Input IN, SurfaceOutputStandard o, inout fixed4 color)
        {
            color.a = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
