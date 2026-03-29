#version 450

struct PointLightInfo
{
    vec3 Position;
    float _pad0;
    vec3 Colour;
    float _pad1;
    float Range;
    float _pad2;
    float _pad3;
    float _pad4;
};

struct PointLightsInfo
{
    PointLightInfo PointLights[4];
    int NumActiveLights;
    float _padding0;
    float _padding1;
    float _padding2;
};

struct DirectionalLightInfo
{
    int Enabled;
    float _pad0;
    float _pad1;
    float _pad2;
    vec3 Direction;
    float _pad3;
    vec3 Colour;
    float _pad4;
};

struct CameraInfo
{
    vec3 WorldPosition;
    float _padding1;
    vec3 LookDirection;
    float _padding2;
};

layout(location = 0) in vec3 fsin_normal;
layout(location = 1) in vec2 fsin_texCoord;
layout(location = 2) in vec4 fsin_worldPos;
layout(location = 3) in vec4 fsin_lightPos;
layout(location = 4) in vec4 fsin_tangent;
//layout(location = 4) in mat3 fsin_TBN; // TBN Matrix

layout(location = 0) out vec4 fsout_Color;

layout(set = 2, binding = 0) uniform texture2D Texture;
layout(set = 2, binding = 1) uniform sampler TextureSampler;

layout(set = 3, binding = 0) uniform PointLights
{
    PointLightsInfo _PointLights;
};

layout(set = 3, binding = 1) uniform DirectionalLight
{
    DirectionalLightInfo _DirectionalLight;
};


layout(set = 4, binding = 0) uniform Camera
{
    CameraInfo _CameraInfo;
};

layout(set = 6, binding = 0) uniform texture2D ShadowMap;
layout(set = 6, binding = 1) uniform sampler ShadowMapSampler;

layout(set = 7, binding = 0) uniform texture2D NormalMap;
layout(set = 7, binding = 1) uniform sampler NormalSampler;

layout(set = 8, binding = 0) uniform texture2D MetallicRoughnessMap;
layout(set = 8, binding = 1) uniform sampler MetallicRoughnessSampler;

const float PI = 3.14159265359;
  
float DistributionGGX(vec3 N, vec3 H, float roughness)
{
    float a      = roughness*roughness;
    float a2     = a*a;
    float NdotH  = max(dot(N, H), 0.0);
    float NdotH2 = NdotH*NdotH;
	
    float num   = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;
	
    return num / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r*r) / 8.0;

    float num   = NdotV;
    float denom = NdotV * (1.0 - k) + k;
	
    return num / denom;
}
float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2  = GeometrySchlickGGX(NdotV, roughness);
    float ggx1  = GeometrySchlickGGX(NdotL, roughness);
	
    return ggx1 * ggx2;
}
vec3 fresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}

void main()
{
    vec4 mr4 = texture(sampler2D(MetallicRoughnessMap, MetallicRoughnessSampler), fsin_texCoord);
    float metallic = mr4.b;
    float roughness = mr4.g;
    vec4 albedo4 = texture(sampler2D(Texture, TextureSampler), fsin_texCoord);
    vec3 albedo = albedo4.xyz;

    vec3 fragPos = fsin_worldPos.xyz;

    // in theory we should be able to calculate the tbn in the vertex shader and output it, but that makes the fragment colours go all screwy
    vec3 bitangent = cross(fsin_normal, fsin_tangent.xyz) * fsin_tangent.w;
    mat3 tbn = mat3(fsin_tangent.xyz, bitangent, fsin_normal);

    vec3 normalTS = texture(sampler2D(NormalMap, NormalSampler), fsin_texCoord).rgb * 2.0 - 1.0;
    vec3 normal = normalize(tbn * normalTS);

    vec3 viewDir = normalize(_CameraInfo.WorldPosition - fragPos);

    //vec3 ambient = vec3(0.4);
    vec3 diffuse = vec3(0.0);
    vec3 specular = vec3(0.0);

    // TODO: Get these from material
    float specularPower = 64.0; // aka shininess
    float specularStrength = 0.5;

    // point lights
    for (int i = 0; i < _PointLights.NumActiveLights; i++)
    {
        PointLightInfo pli = _PointLights.PointLights[i];

        vec3 lightDir = normalize(pli.Position - fragPos);
        float distance = length(pli.Position - fragPos);

        float attenuation = clamp(1.0 - (distance / pli.Range), 0.0, 1.0);
        attenuation *= attenuation;

        float diff = max(dot(normal, lightDir), 0.0);
        diffuse += diff * attenuation * pli.Colour;

        vec3 halfDir = normalize(lightDir + viewDir);
        float spec = pow(max(dot(normal, halfDir), 0.0), specularPower);
        specular += specularStrength * spec * attenuation * pli.Colour;
    }

    float shadow = 0;

    vec3 F0 = vec3(0.04); 
    F0 = mix(F0, albedo, metallic);

    // reflectance equation
    vec3 Lo = vec3(0.0);

    // directional light
    if (_DirectionalLight.Enabled > 0)
    {
        vec3 lightDir = normalize(-_DirectionalLight.Direction);

        //shadow
        vec3 projCoords = fsin_lightPos.xyz / fsin_lightPos.w;
        projCoords.y *= -1;
        projCoords = projCoords * 0.5 + 0.5; // [-1,1] → [0,1]
        float currentDepth = projCoords.z;

        float shadow = 0.0;
        float bias = 0.0001;

        // TODO: Get textureSize() working
        vec2 texelSize = 1.0 / vec2(4096, 4096);//textureSize(shadowMap);
        for(int x = -1; x <= 1; ++x)
        {
            for(int y = -1; y <= 1; ++y)
            {
                float pcfDepth = texture(sampler2D(ShadowMap, ShadowMapSampler), projCoords.xy + vec2(x, y) * texelSize).r; 
                shadow += currentDepth - bias > pcfDepth ? 1.0 : 0.0;        
            }    
        }
        shadow /= 9.0;

        //float closestDepth = texture(sampler2D(ShadowMap, ShadowMapSampler), projCoords.xy).r;

        //shadow = currentDepth - bias > closestDepth ? 1.0 : 0.0;

        if (projCoords.x < 0.0
            || projCoords.x > 1.0
            || projCoords.y < 0.0
            || projCoords.y > 1.0
            || projCoords.z > 1.0)
        {
            shadow = 0;
        }
        vec3 halfDir = normalize(lightDir + viewDir);
        float NDF = DistributionGGX(normal, halfDir, roughness);
        float G = GeometrySmith(normal, viewDir, lightDir, roughness);
        vec3 F = fresnelSchlick(max(dot(halfDir, viewDir), 0.0), F0);

        vec3 kS = F;
        vec3 kD = vec3(1.0) - kS;
        kD *= 1 - metallic;

        vec3 numerator = NDF * G * F;
        float denominator = 4 * max(dot(normal, viewDir), 0.0) * max(dot(normal, lightDir), 0) + 0.0001;
        vec3 specular = numerator / denominator;

        float NdotL = max(dot(normal, lightDir), 0.0);
        vec3 radiance = _DirectionalLight.Colour;
        Lo += (kD * albedo / PI + specular) * radiance * NdotL * (1 - shadow);
    }

    vec3 ambient = vec3(0.03) * albedo;
    vec3 color = ambient + Lo;

    color = color / (color + vec3(1.0));
    color = pow(color, vec3(1.0/2.2));  
   
    // do this to avoid screwiness
    color += diffuse + specular;

    fsout_Color = vec4(color, albedo4.a);
}