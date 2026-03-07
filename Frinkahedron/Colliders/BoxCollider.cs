using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using System.Numerics;

namespace Frinkahedron.Core.Colliders
{
    public sealed class BoxCollider(Vector3 dimensions) : ICollider
    {
        public Vector3 Dimensions { get; } = dimensions;

        public CollisionManifold CheckForCollisions(Position position, ICollider other, Position otherPosition)
        {
            if (other is SphereCollider sphereCollider)
            {
                if (position.Orientation.IsIdentity)
                {
                    return Collisions.AABBSphereCollision(this, position.Centre, sphereCollider, otherPosition);
                }
            }
            if (other is BoxCollider boxCollider)
            {
                return Collisions.BoxBoxCollision(this, position, boxCollider, otherPosition);
                //return Collisions.BoxBoxCollision(boxCollider, otherPosition, this, position);
            }
            return CollisionManifold.NoCollision();
        }

        public void Draw(IRenderer renderer, Matrix4x4 position)
        {
            Matrix4x4 scale = Matrix4x4.CreateScale(Dimensions);
            renderer.DrawCuboid(scale * position);
        }

        
    }
}
