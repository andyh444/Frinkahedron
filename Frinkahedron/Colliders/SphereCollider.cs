using Frinkahedron.Core.Physics;
using System.Numerics;

namespace Frinkahedron.Core.Colliders
{
    public sealed class SphereCollider(float radius) : ICollider
    {
        public float Radius { get; } = radius;

        public CollisionManifold CheckForCollisions(Position position, ICollider other, Position otherPosition)
        {
            if (other is SphereCollider otherCollider)
            {
                return Collisions.SphereSphereCollision(this, position, otherCollider, otherPosition);
            }
            if (other is BoxCollider otherBoxCollider)
            {
                if (otherPosition.Orientation.IsIdentity)
                {
                    return Collisions.SphereAABBCollision(this, position, otherBoxCollider, otherPosition.Centre);
                }
            }

            return CollisionManifold.NoCollision();
        }

        public void Draw(IRenderer renderer, Matrix4x4 position)
        {
            Matrix4x4 scale = Matrix4x4.CreateScale(2 * Radius);
            renderer.DrawEllipsoid(scale * position);
        }
    }
}
