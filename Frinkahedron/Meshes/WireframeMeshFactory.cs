using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Meshes
{
    public static class WireframeMeshFactory
    {
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

        public static WireframeMesh CreateUnitWireframeRing(Vector4 colour, Vector3 normal)
        {
            const int STEPS = 64;

            ColourVertex[] vertices = new ColourVertex[STEPS];
            IndexLine[] indices = new IndexLine[STEPS];

            Vector3 reference = MathF.Abs(Vector3.Dot(normal, Vector3.UnitX)) > 0.99f
                ? Vector3.UnitY
                : Vector3.UnitX;
            Vector3 firstPoint = Vector3.Normalize(Vector3.Cross(normal, reference));

            for (int i = 0; i < STEPS; i++)
            {
                float angle = 2 * MathF.PI * i / STEPS;
                var rot = Quaternion.CreateFromAxisAngle(normal, angle);

                vertices[i] = new ColourVertex(Vector3.Transform(firstPoint, rot), colour);
                indices[i] = new IndexLine((ushort)i, (ushort)((i + 1) % STEPS));
            }
            return new WireframeMesh(vertices, indices);
        }

        public static WireframeMesh CreateUnitWireframeCube(Vector4 colour)
        {
            ColourVertex[] vertices = new ColourVertex[]
            {
                // Front
                new ColourVertex(new Vector3(-0.5f,  0.5f,  0.5f), colour),
                new ColourVertex(new Vector3( 0.5f,  0.5f,  0.5f), colour),
                new ColourVertex(new Vector3( 0.5f, -0.5f,  0.5f), colour),
                new ColourVertex(new Vector3(-0.5f, -0.5f,  0.5f), colour),

                // Back
                new ColourVertex(new Vector3(-0.5f,  0.5f, -0.5f), colour),
                new ColourVertex(new Vector3( 0.5f,  0.5f, -0.5f), colour),
                new ColourVertex(new Vector3( 0.5f, -0.5f, -0.5f), colour),
                new ColourVertex(new Vector3(-0.5f, -0.5f, -0.5f), colour),
            };

            IndexLine[] indices = new IndexLine[]
            {
                new IndexLine(0, 1),
                new IndexLine(1, 2),
                new IndexLine(2, 3),
                new IndexLine(3, 0),

                new IndexLine(4, 5),
                new IndexLine(5, 6),
                new IndexLine(6, 7),
                new IndexLine(7, 4),

                new IndexLine(0, 4),
                new IndexLine(1, 5),
                new IndexLine(2, 6),
                new IndexLine(3, 7),
            };

            return new WireframeMesh(vertices, indices);
        }
    }
}
