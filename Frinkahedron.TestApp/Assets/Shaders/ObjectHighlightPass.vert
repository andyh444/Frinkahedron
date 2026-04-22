#version 450

layout(location = 0) in vec3 Position;
layout(location = 1) in vec3 Normal;

layout(set = 0, binding = 0) uniform ModelMatrices
{
    mat4 model;
};

layout(set = 1, binding = 0) uniform CameraMatrices
{
    mat4 view;
    mat4 projection;
};

layout(std140, set = 2, binding = 0) uniform HighlightParamsUB
{
    vec4 Color;
    vec4 Params; // Params.x = OutlineWidth
} highlightParams;

void main()
{
    vec4 worldPos = model * vec4(Position, 1.0);

    mat3 normalMatrix = transpose(inverse(mat3(model)));
    vec3 worldNormal = normalize(normalMatrix * Normal);

    float outlineWidth = highlightParams.Params.x;
    /*worldPos.xyz += worldNormal * outlineWidth;

    gl_Position = projection * view * worldPos;*/

    vec4 clipPos = projection * view * worldPos;
    vec3 clipNormal = normalize((projection * view * vec4(worldNormal, 0.0)).xyz);

    clipPos.xy += clipNormal.xy * outlineWidth * clipPos.w;
    gl_Position = clipPos;
}