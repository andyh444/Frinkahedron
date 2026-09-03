namespace Frinkahedron.Core.Meshes
{
    public readonly struct IndexLine(ushort index1, ushort index2)
    {
        public readonly ushort Index1 = index1;
        public readonly ushort Index2 = index2;

        public static uint SizeInBytes => sizeof(ushort) * 2;
    }
}
