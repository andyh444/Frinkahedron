using System.Numerics;

namespace Frinkahedron.Core
{
    public interface IRenderer
    {
        public void DrawCuboid(Matrix4x4 transform);
    }
}