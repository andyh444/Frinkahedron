using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Physics;
using System.Numerics;

namespace Frinkahedron.Core
{
    public sealed class GameObject(Vector3 position, Behaviour? behaviour = null, IShape? colliderShape = null, RigidBody? rigidBody = null)
    {
        public Position Position { get; } = new Position(position, Quaternion.Identity);

        public Behaviour? Behaviour { get; } = behaviour;

        public IShape? Collider { get; } = colliderShape;

        public RigidBody? RigidBody { get; } = rigidBody;

        public void Update(GameState gameState)
        {
            Behaviour?.Update(this, gameState);
            RigidBody?.IntegratePosition(gameState.DeltaTime, Position);
        }

        public void Draw(IRenderer renderer)
        {
            Collider?.Draw(renderer, Position.ToMatrix());
        }
    }
}