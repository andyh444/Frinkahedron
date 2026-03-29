#version 450

layout(location = 0) in vec2 fsin_texCoord;

layout(location = 0) out vec4 fsout_colour;

layout(set = 0, binding = 0) uniform texture2D Texture;
layout(set = 0, binding = 1) uniform sampler TextureSampler;

void main()
{
	fsout_colour = texture(sampler2D(Texture, TextureSampler), fsin_texCoord);
}