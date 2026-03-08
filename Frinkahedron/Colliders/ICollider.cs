using Frinkahedron.Core.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Colliders
{
    public interface ICollider
    {
        public void Draw(IRenderer renderer, Matrix4x4 position);

        /// <summary>
        /// Checks for collisions between this collider and the specified other one
        /// </summary>
        /// <param name="position">World position of the object this collider represents</param>
        /// <param name="other">other collider</param>
        /// <param name="otherPosition">World position of the object the other collider represents</param>
        /// <returns>A CollisionManifold containing contact points (if any), a normal pointing from the other collider to this one, and the penetration of this collider into the other</returns>
        CollisionManifold CheckForCollisions(Position position, ICollider other, Position otherPosition);
    }

    public readonly struct CollisionManifold(Vector3[] Points, Vector3 Normal, float Penetration)
    {
        public Vector3[] Points { get; } = Points;
        public Vector3 Normal { get; } = Normal;
        public float Penetration { get; } = Penetration;

        public static CollisionManifold NoCollision() => new CollisionManifold([], default, default);

        public CollisionManifold Invert() => new CollisionManifold(Points, -Normal, Penetration);
    }
}
