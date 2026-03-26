using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core
{
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord)
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

    public sealed class Mesh
    {
        public Vertex[] Vertices { get; }

        public IndexTriangle[] Triangles { get; }

        public Mesh(Vertex[] vertices, IndexTriangle[] indices)
        {
            Vertices = vertices;
            Triangles = indices;
        }
    }
}
