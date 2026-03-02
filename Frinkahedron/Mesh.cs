using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core
{
    public struct VertexPositionColor
    {
        public Vector3 Position;
        public Vector4 Color;
        public VertexPositionColor(Vector3 position, Vector4 color)
        {
            Position = position;
            Color = color;
        }
        public const uint SizeInBytes = 28;
    }

    public sealed class Mesh
    {
        public VertexPositionColor[] Vertices { get; }

        public ushort[] Indices { get; }

        public Mesh(VertexPositionColor[] vertices, ushort[] indices)
        {
            Vertices = vertices;
            Indices = indices;
        }
    }
}
