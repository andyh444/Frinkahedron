using Frinkahedron.Core;
using Veldrid;

namespace Frinkahedron.VeldridImplementation
{
    public class WireframeInfo : IDisposable
    {
        private readonly DeviceBuffer _vertexBuffer;
        private readonly DeviceBuffer _indexBuffer;
        private readonly WireframeMesh _mesh;

        private WireframeInfo(DeviceBuffer vertexBuffer, DeviceBuffer indexBuffer, WireframeMesh mesh)
        {
            _vertexBuffer = vertexBuffer;
            _indexBuffer = indexBuffer;
            _mesh = mesh;
        }

        public static VertexLayoutDescription GetVertexLayoutDescription()
        {
            return new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                new VertexElementDescription("Colour", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));
        }

        public static WireframeInfo Create(WireframeMesh mesh, GraphicsDevice graphicsDevice)
        {
            ResourceFactory factory = graphicsDevice.ResourceFactory;
            var vertexBuffer = factory.CreateBuffer(new BufferDescription((uint)mesh.Vertices.Length * TexVertex.SizeInBytes, BufferUsage.VertexBuffer));
            var indexBuffer = factory.CreateBuffer(new BufferDescription((uint)mesh.Indices.Length * IndexLine.SizeInBytes, BufferUsage.IndexBuffer));

            graphicsDevice.UpdateBuffer(vertexBuffer, 0, mesh.Vertices);
            graphicsDevice.UpdateBuffer(indexBuffer, 0, mesh.Indices);

            return new WireframeInfo(vertexBuffer, indexBuffer, mesh);
        }

        public void Draw(CommandList commandList)
        {
            commandList.SetVertexBuffer(0, _vertexBuffer);
            commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            commandList.DrawIndexed(
                indexCount: (uint)_mesh.Indices.Length * 2, // 2 indices per triangle
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
