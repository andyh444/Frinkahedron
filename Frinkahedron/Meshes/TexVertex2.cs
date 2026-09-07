using System.Numerics;

namespace Frinkahedron.Core.Meshes
{
    public struct TexVertex2 : IVertex
    {
        public Vector2 Position;
        public Vector2 TexCoord;

        public TexVertex2(Vector2 position, Vector2 texCoord)
        {
            Position = position;
            TexCoord = texCoord;
        }

        public static uint SizeInBytes => sizeof(float) * (2 + 2);

        public static IEnumerable<(string description, int floatCount)> GetLayout()
        {
            yield return ("Position", 2);
            yield return ("TexCoord", 2);
        }
    }

    public struct ColourVertex2 : IVertex
    {
        public Vector2 Position;
        public Vector4 Colour;

        public ColourVertex2(Vector2 position, Vector4 colour)
        {
            Position = position;
            Colour = colour;
        }

        public static uint SizeInBytes => sizeof(float) * (2 + 4);

        public static IEnumerable<(string description, int floatCount)> GetLayout()
        {
            yield return ("Position", 2);
            yield return ("Colour", 4);
        }
    }
}
