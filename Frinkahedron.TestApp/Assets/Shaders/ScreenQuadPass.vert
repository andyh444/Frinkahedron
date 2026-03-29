#version 450

layout(location = 0) in vec2 Position;
layout(location = 1) in vec2 TexCoord;

layout(location = 0) out vec2 fsin_texCoord;

void main()
{
	fsin_texCoord = TexCoord;
	gl_Position = vec4(Position, 0, 1);
}