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

        public const string PositionNormalUvVertexShader = @"
#version 450

layout(location = 0) in vec3 Position;
layout(location = 1) in vec3 Normal;
layout(location = 2) in vec2 TexCoord;

layout(location = 0) out vec3 fsin_normal;
layout(location = 1) out vec2 fsin_texCoord;

layout(set = 0, binding = 0) uniform Matrices
{
    mat4 model;
    mat4 view;
    mat4 projection;
};

void main()
{
    gl_Position = projection * view * model * vec4(Position, 1);
    fsin_normal = normalize(mat3(transpose(inverse(model))) * Normal);
    fsin_texCoord = TexCoord;
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

        public const string TextureNormalFragmentShader = @"
#version 450

layout(location = 0) in vec3 fsin_normal;
layout(location = 1) in vec2 fsin_texCoord;
layout(location = 0) out vec4 fsout_Color;

layout(set = 1, binding = 0) uniform texture2D Texture;
layout(set = 1, binding = 1) uniform sampler TextureSampler;

void main()
{
    vec3 lightDir = normalize(vec3(0.5, 1, 0));
    float ambientStrength = 0.2f;
    float diffuseStrength = 0.8f * max(dot(lightDir, fsin_normal), 0.0f);
    float scale = ambientStrength + diffuseStrength;
    fsout_Color = scale * texture(sampler2D(Texture, TextureSampler), fsin_texCoord);
}";
    }
}
