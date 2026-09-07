#version 450

layout(location = 0) in vec4 fsin_colour;
layout(location = 0) out vec4 fsout_colour;

void main()
{
	fsout_colour = fsin_colour;
}