using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

    public sealed class Mesh
    {
        public Vertex[] Vertices { get; }

        public ushort[] Indices { get; }

        public Mesh(Vertex[] vertices, ushort[] indices)
        {
            Vertices = vertices;
            Indices = indices;
        }
    }
}
