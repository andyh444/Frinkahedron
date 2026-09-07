namespace Frinkahedron.Core.Meshes
{
    public sealed class TexMesh2(TexVertex2[] vertices, IndexTriangle[] indices) : ITriangleMesh<TexVertex2>
    {
        public TexVertex2[] Vertices { get; } = vertices;

        public IndexTriangle[] Triangles { get; } = indices;
    }

    public sealed class ColourMesh2(ColourVertex2[] vertices, IndexTriangle[] indices) : ITriangleMesh<ColourVertex2>
    {
        public ColourVertex2[] Vertices { get; } = vertices;

        public IndexTriangle[] Triangles { get; } = indices;
    }
}
