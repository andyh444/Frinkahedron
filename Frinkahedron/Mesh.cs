using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core
{
    public struct TexVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public TexVertex(Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            Position = position;
            Normal = Vector3.Normalize(normal);
            TexCoord = texCoord;
        }
        public static uint SizeInBytes => sizeof(float) * 8;
    }

    public readonly struct IndexTriangle(ushort index1, ushort index2, ushort index3)
    {
        public readonly ushort Index1 = index1;
        public readonly ushort Index2 = index2;
        public readonly ushort Index3 = index3;

        public static uint SizeInBytes => sizeof(ushort) * 3;
    }

    public sealed class BasicMesh(Vector3[] vertices, IndexTriangle[] indexTriangles)
    {
        public Vector3[] Vertices { get; } = vertices;
        public IndexTriangle[] IndexTriangles { get; } = indexTriangles;
    }

    public sealed class TexMesh(TexVertex[] vertices, IndexTriangle[] indices)
    {
        public TexVertex[] Vertices { get; } = vertices;

        public IndexTriangle[] Triangles { get; } = indices;
    }
}
