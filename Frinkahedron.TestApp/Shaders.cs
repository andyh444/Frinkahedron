using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.TestApp
{
    internal static class Shaders
    {
        public const string PositionColourVertexShader = @"
#version 450

layout(location = 0) in vec3 Position;
layout(location = 1) in vec4 Color;

layout(location = 0) out vec4 fsin_Color;

layout(set = 0, binding = 0) uniform Matrices
{
    mat4 model;
    mat4 view;
    mat4 projection;
};

void main()
{
    gl_Position = projection * view * model * vec4(Position, 1);
    fsin_Color = Color;
}";

        public const string PositionUvVertexShader = @"
#version 450

layout(location = 0) in vec3 Position;
layout(location = 1) in vec2 texCoord;

layout(location = 0) out vec2 fsin_texCoord;

layout(set = 0, binding = 0) uniform Matrices
{
    mat4 model;
    mat4 view;
    mat4 projection;
};

void main()
{
    gl_Position = projection * view * model * vec4(Position, 1);
    fsin_texCoord = texCoord;
}";

        public const string ColourInFragmentShader = @"
#version 450

layout(location = 0) in vec4 fsin_Color;
layout(location = 0) out vec4 fsout_Color;

void main()
{
    fsout_Color = fsin_Color;
}";

        public const string TextureFragmentShader = @"
#version 450

layout(location = 0) in vec2 fsin_texCoord;
layout(location = 0) out vec4 fsout_Color;

layout(set = 1, binding = 0) uniform texture2D Texture;
layout(set = 1, binding = 1) uniform sampler TextureSampler;

void main()
{
    fsout_Color = texture(sampler2D(Texture, TextureSampler), fsin_texCoord);
}";
    }
}
