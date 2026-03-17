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

void main()
{
    vec4 ambient = vec4(0.2, 0.2, 0.2, 1);
    vec4 diffuseColour = vec4(0, 0, 0, 1);
    for (int i = 0; i < _PointLights.NumActiveLights; i++)
    {
        PointLightInfo pli = _PointLights.PointLights[i];
        vec3 ptLightDir = normalize(pli.Position - fsin_worldPos.xyz);
        float intensity = clamp(dot(normalize(fsin_normal), ptLightDir), 0, 1);
        float lightDistance = distance(pli.Position, fsin_worldPos.xyz);
        intensity = clamp(intensity * (1 - (lightDistance / pli.Range)), 0, 1);
        diffuseColour += intensity * vec4(pli.Colour, 1);
    }

    fsout_Color = (ambient + diffuseColour) * texture(sampler2D(Texture, TextureSampler), fsin_texCoord);
}