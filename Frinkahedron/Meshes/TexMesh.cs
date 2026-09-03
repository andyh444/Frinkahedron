namespace Frinkahedron.Core.Meshes
{
    public sealed class TexMesh(TexVertex3[] vertices, IndexTriangle[] indices) : ITriangleMesh<TexVertex3>
    {
        public TexVertex3[] Vertices { get; } = vertices;

        public IndexTriangle[] Triangles { get; } = indices;
    }
}
