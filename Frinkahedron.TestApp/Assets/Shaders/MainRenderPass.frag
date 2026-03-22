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


void main()
{
    vec3 fragPos = fsin_worldPos.xyz;
    vec3 normal = normalize(fsin_normal);
    vec3 viewDir = normalize(_CameraInfo.WorldPosition - fragPos);

    vec3 ambient = vec3(0.2);
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

    // directional light
    if (_DirectionalLight.Enabled > 0)
    {
        vec3 lightDir = normalize(-_DirectionalLight.Direction);
        float diff = max(dot(normal, lightDir), 0.0);
        diffuse += diff * _DirectionalLight.Colour;

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

        diffuse *= (1 - shadow);
        
        vec3 halfDir = normalize(lightDir + viewDir);
        float spec = pow(max(dot(normal, halfDir), 0.0), specularPower);
        specular += specularStrength * spec * _DirectionalLight.Colour;
        specular *= (1 - shadow);
    }

    vec4 texColor = texture(sampler2D(Texture, TextureSampler), fsin_texCoord);

    vec3 color = (ambient + diffuse) * texColor.rgb + specular;
    fsout_Color = vec4(color, texColor.a);
}