namespace Frinkahedron.Core.Meshes
{
    public interface IVertex
    {
        static abstract uint SizeInBytes { get; }

        static abstract IEnumerable<(string description, int floatCount)> GetLayout();
    }
}
