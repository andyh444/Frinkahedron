#version 450

layout(location = 0) in vec3 Position;
layout(location = 1) in vec3 Normal;
layout(location = 2) in vec2 TexCoord;

layout(set = 0, binding = 0) uniform Matrices
{
    mat4 model;
    mat4 view;
    mat4 projection;
    mat4 _unused;
};

void main()
{
    vec4 fsin_worldPos = model * vec4(Position, 1);
    gl_Position = projection * view * fsin_worldPos;
}