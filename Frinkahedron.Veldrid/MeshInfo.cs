using Frinkahedron.Core.Meshes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Frinkahedron.VeldridImplementation
{
    public abstract class MeshInfo : IDisposable
    {
        public static MeshInfo<TVertex> Create<TVertex>(ITriangleMesh<TVertex> mesh, GraphicsDevice graphicsDevice) where TVertex : unmanaged, IVertex
        {
            return MeshInfo<TVertex>.Create(mesh, graphicsDevice);
        }

        public abstract void Dispose();

        public abstract void Draw(CommandList commandList);
    }

    public class MeshInfo<TVertex> : MeshInfo where TVertex : unmanaged, IVertex
    {
        private readonly DeviceBuffer _vertexBuffer;
        private readonly DeviceBuffer _indexBuffer;
        private readonly ITriangleMesh<TVertex> _mesh;

        private MeshInfo(DeviceBuffer vertexBuffer, DeviceBuffer indexBuffer, ITriangleMesh<TVertex> mesh)
        {
            _vertexBuffer = vertexBuffer;
            _indexBuffer = indexBuffer;
            _mesh = mesh;
        }

        public static VertexLayoutDescription GetVertexLayoutDescription()
        {
            return new VertexLayoutDescription(TVertex.GetLayout().Select(x => new VertexElementDescription(x.description, VertexElementSemantic.TextureCoordinate, FormatFromCount(x.floatCount))).ToArray());
        }

        private static VertexElementFormat FormatFromCount(int floatCount)
        {
            return floatCount switch
            {
                1 => VertexElementFormat.Float1,
                2 => VertexElementFormat.Float2,
                3 => VertexElementFormat.Float3,
                4 => VertexElementFormat.Float4,
                _ => throw new ArgumentException()
            };
        }

        public static MeshInfo<TVertex> Create(ITriangleMesh<TVertex> mesh, GraphicsDevice graphicsDevice)
        {
            ResourceFactory factory = graphicsDevice.ResourceFactory;
            var vertexBuffer = factory.CreateBuffer(new BufferDescription((uint)mesh.Vertices.Length * TVertex.SizeInBytes, BufferUsage.VertexBuffer));
            var indexBuffer = factory.CreateBuffer(new BufferDescription((uint)mesh.Triangles.Length * IndexTriangle.SizeInBytes, BufferUsage.IndexBuffer));

            graphicsDevice.UpdateBuffer(vertexBuffer, 0, mesh.Vertices);
            graphicsDevice.UpdateBuffer(indexBuffer, 0, mesh.Triangles);

            return new MeshInfo<TVertex>(vertexBuffer, indexBuffer, mesh);
        }

        public override void Draw(CommandList commandList)
        {
            commandList.SetVertexBuffer(0, _vertexBuffer);
            commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            commandList.DrawIndexed(
                indexCount: (uint)_mesh.Triangles.Length * 3, // 3 indices per triangle
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);
        }

        public override void Dispose()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }
    }
}
