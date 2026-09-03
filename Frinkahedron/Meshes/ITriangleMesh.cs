namespace Frinkahedron.Core.Meshes
{
    public interface ITriangleMesh<TVertex> where TVertex : unmanaged, IVertex
    {
        TVertex[] Vertices { get; }

        IndexTriangle[] Triangles { get; }
    }
}
