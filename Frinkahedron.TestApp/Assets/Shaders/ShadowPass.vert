#version 450

layout(location = 0) in vec3 Position;
layout(location = 1) in vec3 Normal;
layout(location = 2) in vec2 TexCoord;

layout(location = 0) out vec4 fsin_position;


layout(set = 0, binding = 0) uniform ModelMatrices
{
    mat4 model;
};

layout(set = 1, binding = 0) uniform CameraMatrices
{
    mat4 view;
    mat4 projection;
};

void main()
{
    vec4 fsin_worldPos = model * vec4(Position, 1);
    fsin_position = projection * view * fsin_worldPos;
    gl_Position = fsin_position;
}