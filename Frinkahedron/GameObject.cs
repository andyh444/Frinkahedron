using System.Numerics;

namespace Frinkahedron.Core
{
    public sealed class GameObject(Vector3 position, Behaviour? behaviour = null)
    {
        public Vector3 Position { get; private set; } = position;

        public float RotateX { get; private set; }

        public float RotateY { get; private set; }

        public float RotateZ { get; private set; }

        public Behaviour? Behaviour { get; } = behaviour;

        public void Update(GameState gameState)
        {
            RotateX += 0.1f * gameState.DeltaTime;
            RotateY += 0.4f * gameState.DeltaTime;
            RotateZ += 0.2f * gameState.DeltaTime;

            Behaviour?.Update(gameState);
        }

        public void Draw(IRenderer renderer)
        {
            Matrix4x4 transform = Matrix4x4.CreateFromYawPitchRoll(RotateY, RotateX, RotateZ) * Matrix4x4.CreateTranslation(Position);
            renderer.DrawCuboid(transform);
        }
    }
}