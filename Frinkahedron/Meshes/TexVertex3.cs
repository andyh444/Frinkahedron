using System.Numerics;

namespace Frinkahedron.Core.Meshes
{
    public struct TexVertex3 : IVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public Vector4 Tangent;

        public TexVertex3(Vector3 position, Vector3 normal, Vector2 texCoord, Vector4 tangent)
        {
            Position = position;
            Normal = Vector3.Normalize(normal);
            TexCoord = texCoord;
            Tangent = tangent;
        }

        public static uint SizeInBytes => sizeof(float) * (3 + 3 + 2 + 4);

        public static IEnumerable<(string description, int floatCount)> GetLayout()
        {
            yield return ("Position", 3);
            yield return ("Normal", 3);
            yield return ("TexCoord", 2);
            yield return ("Colour", 4);
        }
    }
}
