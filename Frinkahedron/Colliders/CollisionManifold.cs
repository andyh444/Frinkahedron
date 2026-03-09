using System.Numerics;

namespace Frinkahedron.Core.Colliders
{
    public readonly struct CollisionManifold(Vector3[] Points, Vector3 Normal, float Penetration)
    {
        public Vector3[] Points { get; } = Points;
        public Vector3 Normal { get; } = Normal;
        public float Penetration { get; } = Penetration;

        public bool CollisionFound => Points.Length > 0;

        public static CollisionManifold NoCollision() => new CollisionManifold([], default, default);

        public CollisionManifold Invert() => new CollisionManifold(Points, -Normal, Penetration);
    }
}
