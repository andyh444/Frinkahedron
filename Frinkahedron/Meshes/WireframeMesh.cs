using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Meshes
{

    public sealed class WireframeMesh(ColourVertex[] vertices, IndexLine[] indices)
    {
        public ColourVertex[] Vertices { get; } = vertices;

        public IndexLine[] Indices { get; } = indices;

        public static WireframeMesh Combine(IEnumerable<WireframeMesh> meshes)
        {
            List<ColourVertex> vertices = new List<ColourVertex>();
            List<IndexLine> indices = new List<IndexLine>();
            foreach (var mesh in meshes)
            {
                int indexOffset = vertices.Count;
                vertices.AddRange(mesh.Vertices);
                indices.AddRange(mesh.Indices.Select(x => new IndexLine((ushort)(x.Index1 + indexOffset), (ushort)(x.Index2 + indexOffset))));
            }
            return new WireframeMesh(vertices.ToArray(), indices.ToArray());
        }
    }
}
