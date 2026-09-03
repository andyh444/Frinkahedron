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

        public static uint SizeInBytes => sizeof(float) * (3 + 2);

        public static IEnumerable<(string description, int floatCount)> GetLayout()
        {
            yield return ("Position", 2);
            yield return ("TexCoord", 2);
        }
    }
}
