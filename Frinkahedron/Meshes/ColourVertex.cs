using System.Numerics;

namespace Frinkahedron.Core.Meshes
{
    public struct ColourVertex : IVertex
    {
        public Vector3 Position;
        public Vector4 Colour;

        public ColourVertex(Vector3 position, Vector4 colour)
        {
            Position = position;
            Colour = colour;
        }

        public static uint SizeInBytes => sizeof(float) * (3 + 4);

        public static IEnumerable<(string description, int floatCount)> GetLayout()
        {
            yield return ("Position", 3);
            yield return ("Colour", 4);
        }
    }
}
