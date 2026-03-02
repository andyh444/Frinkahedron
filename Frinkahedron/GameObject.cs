using System.Numerics;

namespace Frinkahedron.Core
{
    public sealed class GameObject
    {
        public Matrix4x4 Transform { get; private set; } = Matrix4x4.Identity;

        public void Update(float deltaTime)
        {
            Transform = Matrix4x4.CreateRotationX(0.01f * deltaTime) * Matrix4x4.CreateRotationY(0.04f * deltaTime) * Matrix4x4.CreateRotationZ(0.03f * deltaTime) * Transform;
        }

        public void Draw(IRenderer renderer)
        {
            renderer.DrawCuboid(Transform);
        }
    }
}