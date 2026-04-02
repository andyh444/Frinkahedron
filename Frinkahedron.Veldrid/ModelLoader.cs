using Frinkahedron.Core;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.OpenGLBinding;

namespace Frinkahedron.VeldridImplementation
{
    public class Entity(MeshInfo mesh, TextureInfo colourTexture, TextureInfo metallicRoughnessMap, TextureInfo normalMap, Matrix4x4 transform) : IDisposable
    {
        public MeshInfo Mesh { get; } = mesh;

        public TextureInfo ColourTexture { get; } = colourTexture;
        public TextureInfo MetallicRoughnessMap { get; } = metallicRoughnessMap;
        public TextureInfo NormalMap { get; } = normalMap;

        public Matrix4x4 Transform { get; } = transform;

        public void Dispose() => Mesh.Dispose();
    }

    public class Model
    {
        public IReadOnlyList<Entity> Entities { get; }

        public Model(IReadOnlyList<Entity> entities)
        {
            Entities = entities;
        }
    }

    public static class ModelLoader
    {
        public static Model LoadModel(ResourceFactory factory, GraphicsDevice graphicsDevice, string file, TextureInfo fallbackTexture)
        {
            var model = SharpGLTF.Schema2.ModelRoot.Load(file);
            List<Entity> entities = new List<Entity>();

            foreach (var mesh in model.LogicalMeshes.Take(2))
            {
                foreach (var primitive in mesh.Primitives)
                {


                    SharpGLTF.Memory.IAccessorArray<Vector3> positions = primitive.GetVertexAccessor("POSITION").AsVector3Array();
                    SharpGLTF.Memory.IAccessorArray<Vector3> normals = primitive.GetVertexAccessor("NORMAL").AsVector3Array();
                    SharpGLTF.Memory.IAccessorArray<Vector2> uvs = primitive.GetVertexAccessor("TEXCOORD_0").AsVector2Array();
                    SharpGLTF.Memory.IAccessorArray<uint> indices = primitive.GetIndexAccessor().AsIndicesArray();
                    var tangents = primitive.GetVertexAccessor("TANGENT")?.AsVector4Array().ToArray()
                        ?? GenerateTangents(positions, normals, uvs, indices);

                    TexVertex[] vertices = new TexVertex[positions.Count];
                    List<IndexTriangle> triangles = new List<IndexTriangle>();

                    for (int i = 0; i < positions.Count; i++)
                    {
                        vertices[i] = new TexVertex(
                            positions[i],
                            normals[i],
                            uvs[i],
                            tangents[i]);
                    }
                    foreach ((int i1, int i2, int i3) in primitive.GetTriangleIndices())
                    {
                        triangles.Add(new IndexTriangle(
                            (ushort)i2,
                            (ushort)i1,
                            (ushort)i3));
                    }

                    TexMesh texMesh = new TexMesh(vertices, triangles.ToArray());
                    MeshInfo texMeshInfo = MeshInfo.Create(texMesh, graphicsDevice);

                    TextureInfo albedo = GetTexture(primitive.Material, factory, graphicsDevice, "BaseColor", fallbackTexture);
                    TextureInfo metallicRoughness = GetTexture(primitive.Material, factory, graphicsDevice, "MetallicRoughness", fallbackTexture);
                    TextureInfo normalMap = GetTexture(primitive.Material, factory, graphicsDevice, "Normal", fallbackTexture);

                    // TODO: Replace hardcoded texture indices
                    entities.Add(new Entity(texMeshInfo, albedo, metallicRoughness, normalMap, Matrix4x4.Identity));
                }
            }

            return new Model(entities);
        }

        private static TextureInfo GetTexture(Material material, ResourceFactory factory, GraphicsDevice graphicsDevice, string channelID, TextureInfo fallbackTexture)
        {
            MaterialChannel? channel = material.FindChannel(channelID);
            if (channel?.Texture is null)
            {
                return fallbackTexture;
            }
            using var stream = channel.Value.Texture.PrimaryImage.Content.Open();
            return TextureInfo.Create(factory, graphicsDevice, stream, true);

        }

        private static Vector4[] GenerateTangents(
            IReadOnlyList<Vector3> vertices,
            IReadOnlyList<Vector3> normals,
            IReadOnlyList<Vector2> uvs,
            IReadOnlyList<uint> triangles)
        {
            int vertexCount = vertices.Count;

            Vector3[] tan1 = new Vector3[vertexCount];
            Vector3[] tan2 = new Vector3[vertexCount];
            Vector4[] tangents = new Vector4[vertexCount];

            for (int i = 0; i < triangles.Count; i += 3)
            {
                int i1 = (int)triangles[i];
                int i2 = (int)triangles[i + 1];
                int i3 = (int)triangles[i + 2];

                Vector3 v1 = vertices[i1];
                Vector3 v2 = vertices[i2];
                Vector3 v3 = vertices[i3];

                Vector2 w1 = uvs[i1];
                Vector2 w2 = uvs[i2];
                Vector2 w3 = uvs[i3];

                float x1 = v2.X - v1.X;
                float x2 = v3.X - v1.X;
                float y1 = v2.Y - v1.Y;
                float y2 = v3.Y - v1.Y;
                float z1 = v2.Z - v1.Z;
                float z2 = v3.Z - v1.Z;

                float s1 = w2.X - w1.X;
                float s2 = w3.X - w1.X;
                float t1 = w2.Y - w1.Y;
                float t2 = w3.Y - w1.Y;

                float r = (s1 * t2 - s2 * t1);
                if (Math.Abs(r) < 1e-6f)
                    r = 1.0f;
                else
                    r = 1.0f / r;

                Vector3 sdir = new Vector3(
                    (t2 * x1 - t1 * x2) * r,
                    (t2 * y1 - t1 * y2) * r,
                    (t2 * z1 - t1 * z2) * r);

                Vector3 tdir = new Vector3(
                    (s1 * x2 - s2 * x1) * r,
                    (s1 * y2 - s2 * y1) * r,
                    (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }

            for (int i = 0; i < vertexCount; i++)
            {
                Vector3 n = normals[i];
                Vector3 t = tan1[i];

                // Gram-Schmidt orthogonalize
                Vector3 tangent = Vector3.Normalize(t - n * Vector3.Dot(n, t));

                // Calculate handedness
                float w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f;

                tangents[i] = new Vector4(tangent.X, tangent.Y, tangent.Z, w);
            }
            return tangents;
        }
    }
}
