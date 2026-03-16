using Frinkahedron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Frinkahedron.TestApp
{
    public class Entity(MeshInfo mesh, Matrix4x4 transform) : IDisposable
    {
        public MeshInfo Mesh { get; } = mesh;

        public Matrix4x4 Transform { get; } = transform;

        public void Dispose() => Mesh.Dispose();
    }

    public class MeshInfo : IDisposable
    {
        private readonly DeviceBuffer _vertexBuffer;
        private readonly DeviceBuffer _indexBuffer;
        private readonly Mesh _mesh;

        private MeshInfo(DeviceBuffer vertexBuffer, DeviceBuffer indexBuffer, Mesh mesh)
        {
            _vertexBuffer = vertexBuffer;
            _indexBuffer = indexBuffer;
            _mesh = mesh;
        }

        public static VertexLayoutDescription GetVertexLayoutDescription()
        {
            return new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                new VertexElementDescription("TexCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));
        }

        public static MeshInfo Create(Mesh mesh, GraphicsDevice graphicsDevice)
        {
            ResourceFactory factory = graphicsDevice.ResourceFactory;
            var vertexBuffer = factory.CreateBuffer(new BufferDescription((uint)mesh.Vertices.Length * Vertex.SizeInBytes, BufferUsage.VertexBuffer));
            var indexBuffer = factory.CreateBuffer(new BufferDescription((uint)mesh.Indices.Length * sizeof(ushort), BufferUsage.IndexBuffer));

            graphicsDevice.UpdateBuffer(vertexBuffer, 0, mesh.Vertices);
            graphicsDevice.UpdateBuffer(indexBuffer, 0, mesh.Indices);

            return new MeshInfo(vertexBuffer, indexBuffer, mesh);
        }

        public void Draw(CommandList commandList)
        {
            //commandList.Bui
            commandList.SetVertexBuffer(0, _vertexBuffer);
            commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            commandList.DrawIndexed(
                indexCount: (uint)_mesh.Indices.Length,
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);
        }

        public void Dispose()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }
    }
}
