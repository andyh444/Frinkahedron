using Frinkahedron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Frinkahedron.VeldridImplementation
{
    public class Entity(MeshInfo mesh, TextureInfo colourTexture, TextureInfo normalMap, Matrix4x4 transform) : IDisposable
    {
        public MeshInfo Mesh { get; } = mesh;

        public TextureInfo ColourTexture { get; } = colourTexture;
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
        public static Model LoadModel(ResourceFactory factory, GraphicsDevice graphicsDevice, string file)
        {
            var model = SharpGLTF.Schema2.ModelRoot.Load(file);
            List<Entity> entities = new List<Entity>();

            List<TextureInfo> textures = new List<TextureInfo>();
            foreach (var image in model.LogicalTextures)
            {
                using var stream = image.PrimaryImage.Content.Open();
                textures.Add(TextureInfo.Create(factory, graphicsDevice, stream));
            }

            foreach (var mesh in model.LogicalMeshes)
            {
                foreach (var primitive in mesh.Primitives)
                {
                    var positions = primitive.GetVertexAccessor("POSITION").AsVector3Array();
                    var normals = primitive.GetVertexAccessor("NORMAL").AsVector3Array();
                    var tangents = primitive.GetVertexAccessor("TANGENT").AsVector4Array();
                    var uvs = primitive.GetVertexAccessor("TEXCOORD_0").AsVector2Array();
                    var indices = primitive.GetIndexAccessor().AsIndicesArray();

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

                    entities.Add(new Entity(texMeshInfo, textures[0], textures[2], Matrix4x4.Identity));
                }
            }

            return new Model(entities);
        }
    }
}
