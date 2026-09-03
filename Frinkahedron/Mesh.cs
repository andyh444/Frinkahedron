using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core
{
    public interface IVertex
    {
        static abstract uint SizeInBytes { get; }

        static abstract IEnumerable<(string description, int floatCount)> GetLayout();
    }

    public struct ColourVertex : IVertex
    {
        public Vector3 Position;
        public Vector4 Colour;

        public ColourVertex(Vector3 position, Vector4 colour)
        {
            Position = position;
            Colour = colour;
        }

        public static uint SizeInBytes => sizeof(float) * (3 + 4);

        public static IEnumerable<(string description, int floatCount)> GetLayout()
        {
            yield return ("Position", 3);
            yield return ("Colour", 4);
        }
    }

    public struct TexVertex : IVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public Vector4 Tangent;

        public TexVertex(Vector3 position, Vector3 normal, Vector2 texCoord, Vector4 tangent)
        {
            Position = position;
            Normal = Vector3.Normalize(normal);
            TexCoord = texCoord;
            Tangent = tangent;
        }

        public static uint SizeInBytes => sizeof(float) * (3 + 3 + 2 + 4);

        public static IEnumerable<(string description, int floatCount)> GetLayout()
        {
            yield return ("Position", 3);
            yield return ("Normal", 3);
            yield return ("TexCoord", 2);
            yield return ("Colour", 4);
        }
    }

    public struct TexVertex2 : IVertex
    {
        public Vector2 Position;
        public Vector2 TexCoord;

        public TexVertex2(Vector2 position, Vector2 texCoord)
        {
            Position = position;
            TexCoord = texCoord;
        }

        public static uint SizeInBytes => sizeof(float) * (3 + 2);

        public static IEnumerable<(string description, int floatCount)> GetLayout()
        {
            yield return ("Position", 2);
            yield return ("TexCoord", 2);
        }
    }

    public readonly struct IndexTriangle(ushort index1, ushort index2, ushort index3)
    {
        public readonly ushort Index1 = index1;
        public readonly ushort Index2 = index2;
        public readonly ushort Index3 = index3;

        public static uint SizeInBytes => sizeof(ushort) * 3;
    }

    public readonly struct IndexLine(ushort index1, ushort index2)
    {
        public readonly ushort Index1 = index1;
        public readonly ushort Index2 = index2;

        public static uint SizeInBytes => sizeof(ushort) * 2;
    }

    public interface ITriangleMesh<TVertex> where TVertex : unmanaged, IVertex
    {
        TVertex[] Vertices { get; }

        IndexTriangle[] Triangles { get; }
    }

    public sealed class BasicMesh(Vector3[] vertices, IndexTriangle[] indexTriangles)
    {
        public Vector3[] Vertices { get; } = vertices;
        public IndexTriangle[] Triangles { get; } = indexTriangles;
    }

    public sealed class TexMesh(TexVertex[] vertices, IndexTriangle[] indices) : ITriangleMesh<TexVertex>
    {
        public TexVertex[] Vertices { get; } = vertices;

        public IndexTriangle[] Triangles { get; } = indices;
    }

    public sealed class TexMesh2(TexVertex2[] vertices, IndexTriangle[] indices) : ITriangleMesh<TexVertex2>
    {
        public TexVertex2[] Vertices { get; } = vertices;

        public IndexTriangle[] Triangles { get; } = indices;
    }

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
