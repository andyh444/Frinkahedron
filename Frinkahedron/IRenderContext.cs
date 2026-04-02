using System.Numerics;

namespace Frinkahedron.Core
{
    public interface IRenderContext
    {
        public void DrawCuboid(Matrix4x4 transform);

        public void DrawEllipsoid(Matrix4x4 transform);

        public void DrawCylinder(Matrix4x4 transform);

        public void DrawDisc(Matrix4x4 transform);

        public void DrawModel(string modelID, Matrix4x4 transform);
    }

    public class DummyRenderContext : IRenderContext
    {
        public void DrawCuboid(Matrix4x4 transform)
        {
        }

        public void DrawCylinder(Matrix4x4 transform)
        {
        }

        public void DrawDisc(Matrix4x4 transform)
        {
        }

        public void DrawEllipsoid(Matrix4x4 transform)
        {
        }

        public void DrawModel(string modelID, Matrix4x4 transform)
        {
        }
    }
}