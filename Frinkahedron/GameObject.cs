using System.Numerics;

namespace Frinkahedron.Core
{
    public sealed class GameObject(Vector3 position, Behaviour? behaviour = null)
    {
        public Vector3 Position { get; set; } = position;

        public float RotateX { get; set; }

        public float RotateY { get; set; }

        public float RotateZ { get; set; }

        public Behaviour? Behaviour { get; } = behaviour;

        public void Update(GameState gameState)
        {
            Behaviour?.Update(this, gameState);
        }

        public void Draw(IRenderer renderer)
        {
            Matrix4x4 transform = Matrix4x4.CreateFromYawPitchRoll(RotateY, RotateX, RotateZ) * Matrix4x4.CreateTranslation(Position);
            renderer.DrawCuboid(transform);
        }
    }
}