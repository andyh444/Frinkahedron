namespace Frinkahedron.Core.Meshes
{
    public readonly struct IndexTriangle(ushort index1, ushort index2, ushort index3)
    {
        public readonly ushort Index1 = index1;
        public readonly ushort Index2 = index2;
        public readonly ushort Index3 = index3;

        public static uint SizeInBytes => sizeof(ushort) * 3;
    }
}
