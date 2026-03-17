#version 450

layout(location = 0) in vec3 fsin_normal;
layout(location = 1) in vec2 fsin_texCoord;
layout(location = 0) out vec4 fsout_Color;

layout(set = 1, binding = 0) uniform texture2D Texture;
layout(set = 1, binding = 1) uniform sampler TextureSampler;

void main()
{
    vec3 lightDir = normalize(vec3(0.5, 1, 0));
    float ambientStrength = 0.2f;
    float diffuseStrength = 0.8f * max(dot(lightDir, fsin_normal), 0.0f);
    float scale = ambientStrength + diffuseStrength;
    fsout_Color = scale * texture(sampler2D(Texture, TextureSampler), fsin_texCoord);
}