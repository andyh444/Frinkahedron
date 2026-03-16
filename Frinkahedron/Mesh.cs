using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core
{
    public struct VertexPositionUv
    {
        public Vector3 Position;
        public Vector2 TexCoord;
        public VertexPositionUv(Vector3 position, Vector2 texCoord)
        {
            Position = position;
            TexCoord = texCoord;
        }
        public static uint SizeInBytes => sizeof(float) * 5;
    }

    public sealed class Mesh
    {
        public VertexPositionUv[] Vertices { get; }

        public ushort[] Indices { get; }

        public Mesh(VertexPositionUv[] vertices, ushort[] indices)
        {
            Vertices = vertices;
            Indices = indices;
        }
    }
}
