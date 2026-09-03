namespace Frinkahedron.Core.Meshes
{
    public sealed class TexMesh2(TexVertex2[] vertices, IndexTriangle[] indices) : ITriangleMesh<TexVertex2>
    {
        public TexVertex2[] Vertices { get; } = vertices;

        public IndexTriangle[] Triangles { get; } = indices;
    }
}
