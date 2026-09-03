using System.Numerics;

namespace Frinkahedron.Core.Meshes
{
    public sealed class BasicMesh(Vector3[] vertices, IndexTriangle[] indexTriangles)
    {
        public Vector3[] Vertices { get; } = vertices;
        public IndexTriangle[] Triangles { get; } = indexTriangles;
    }
}
