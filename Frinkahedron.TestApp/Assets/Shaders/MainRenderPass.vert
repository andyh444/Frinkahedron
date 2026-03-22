#version 450

layout(location = 0) in vec3 Position;
layout(location = 1) in vec3 Normal;
layout(location = 2) in vec2 TexCoord;

layout(location = 0) out vec3 fsin_normal;
layout(location = 1) out vec2 fsin_texCoord;
layout(location = 2) out vec4 fsin_worldPos;

layout(set = 0, binding = 0) uniform ModelMatrices
{
    mat4 model;
};

layout(set = 1, binding = 0) uniform CameraMatrices
{
    mat4 view;
    mat4 projection;
};

layout(set = 5, binding = 0) uniform LightMatrices
{
    mat4 lightView;
    mat4 lightProjection;
};

void main()
{
    fsin_worldPos = model * vec4(Position, 1);
    gl_Position = projection * view * fsin_worldPos;
    fsin_normal = normalize(mat3(transpose(inverse(model))) * Normal);
    fsin_texCoord = TexCoord;
}