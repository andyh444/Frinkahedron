using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Meshes
{
    public static class TexMesh2Factory
    {
        public static TexMesh2 CreateUnitQuad(float halfDim = 0.5f)
        {
            TexVertex2[] vertices = new[]
            {
                new TexVertex2(new Vector2(-halfDim, halfDim), new Vector2(0, 0)),
                new TexVertex2(new Vector2(halfDim, halfDim), new Vector2(1, 0)),
                new TexVertex2(new Vector2(-halfDim, -halfDim), new Vector2(0, 1)),
                new TexVertex2(new Vector2(halfDim, -halfDim), new Vector2(1, 1))
            };
            IndexTriangle[] triangles = new IndexTriangle[]
            {
                new IndexTriangle(0, 1, 2),
                new IndexTriangle(1, 2, 3),
            };
            return new TexMesh2(vertices, triangles);
        }
    }
}
