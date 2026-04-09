using System.Numerics;

namespace Frinkahedron.Core
{
    public enum Primitive
    {
        None,
        Box,
        Ellipsoid,
        Cylinder,
        Disc
    }

    public interface IRenderContext
    {
        public void DrawPrimitiveWireframe(Primitive primitive, Matrix4x4 transform);

        public void DrawModel(string modelID, Matrix4x4 transform);

        public void DrawModelEntity(string modelID, int entityIndex, Matrix4x4 transform);
    }

    public class DummyRenderContext : IRenderContext
    {
        public void DrawModel(string modelID, Matrix4x4 transform)
        {
        }

        public void DrawModelEntity(string modelID, int entityIndex, Matrix4x4 transform)
        {
        }

        public void DrawPrimitiveWireframe(Primitive primitive, Matrix4x4 transform)
        {
        }
    }
}