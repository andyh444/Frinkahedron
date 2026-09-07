#version 450

layout(location = 0) in vec2 Position;
layout(location = 1) in vec4 Colour;

layout(location = 0) out vec4 fsin_colour;

layout(set = 0, binding = 0) uniform ModelMatrices
{
	vec2 xAxis;
	vec2 yAxis;
    vec2 translation;
	float pad0;
	float pad1;
};

void main()
{
	fsin_colour = Colour;
	vec2 worldPos =
		xAxis * Position.x +
		yAxis * Position.y +
		translation;
	gl_Position = vec4(worldPos, 0.0, 1.0);
}