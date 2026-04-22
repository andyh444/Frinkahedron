#version 450

layout(location = 0) out vec4 fsout_Color;

layout(std140, set = 2, binding = 0) uniform HighlightParamsUB
{
	vec4 Color;
	vec4 Params; // Params.x = OutlineWidth
} highlightParams;

void main()
{
    fsout_Color = highlightParams.Color;
}