using System.Numerics;

namespace Frinkahedron.Core
{
    public enum Primitive
    {
        Box,
        Ellipsoid,
        Cylinder,
        Disc
    }

    public interface IRenderContext
    {
        public void DrawPrimitiveWireframe(Primitive primitive, Matrix4x4 transform);

        public void DrawModel(string modelID, Matrix4x4 transform, bool[]? enabledEntities);
    }

    public class DummyRenderContext : IRenderContext
    {
        public void DrawModel(string modelID, Matrix4x4 transform, bool[]? enabledEntities)
        {
        }

        public void DrawPrimitiveWireframe(Primitive primitive, Matrix4x4 transform)
        {
        }
    }
}