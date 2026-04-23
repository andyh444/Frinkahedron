using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Physics;
using System.Numerics;

namespace Frinkahedron.Core
{
    public sealed class GameObject(Vector3 position, Behaviour? behaviour = null, IShape? colliderShape = null, IRigidBody? rigidBody = null, IRenderable? renderable = null)
    {
        public Position Position { get; } = new Position(position, Quaternion.Identity);

        public Behaviour? Behaviour { get; } = behaviour;

        public IShape? Collider { get; } = colliderShape;

        public IRigidBody? RigidBody { get; } = rigidBody;

        public IRenderable? Renderable { get; } = renderable;

        public void Update(GameState gameState)
        {
            Behaviour?.Update(this, gameState);
            RigidBody?.IntegratePosition(gameState.DeltaTime, Position);
        }

        public void Draw(IRenderContext renderer)
        {
            Renderable?.Render(renderer, Position.ToMatrix());
#if DEBUG
            Collider?.Draw(renderer, Position.ToMatrix());
#endif
        }
    }
}