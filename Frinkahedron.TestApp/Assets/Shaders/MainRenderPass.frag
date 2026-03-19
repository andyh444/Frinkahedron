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
layout(location = 0) out vec4 fsout_Color;

layout(set = 1, binding = 0) uniform texture2D Texture;
layout(set = 1, binding = 1) uniform sampler TextureSampler;

layout(set = 2, binding = 0) uniform PointLights
{
    PointLightsInfo _PointLights;
};

layout(set = 3, binding = 0) uniform Camera
{
    CameraInfo _CameraInfo;
};

void main()
{
    vec3 fragPos = fsin_worldPos.xyz;
    vec3 normal = normalize(fsin_normal);
    vec3 viewDir = normalize(_CameraInfo.WorldPosition - fragPos);

    vec3 ambient = vec3(0.2);
    vec3 diffuse = vec3(0.0);
    vec3 specular = vec3(0.0);

    // TODO: Get these from material
    float specularPower = 64.0;
    float specularStrength = 0.5;

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

    vec4 texColor = texture(sampler2D(Texture, TextureSampler), fsin_texCoord);
    vec3 color = (ambient + diffuse) * texColor.rgb + specular;

    fsout_Color = vec4(color, texColor.a);
}