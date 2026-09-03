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
    }
}
