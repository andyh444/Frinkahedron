using System.Numerics;

namespace Frinkahedron.Core.Colliders
{
    public struct SphereSphereTester : ICollisionPairTester<Sphere, Sphere>
    {
        public static CollisionManifold Test(Collidable<Sphere> shapeA, Collidable<Sphere> shapeB)
        {
            float centreDistanceSq = Vector3.DistanceSquared(shapeA.Position.Centre, shapeB.Position.Centre);
            float radiusSum = shapeA.Shape.Radius + shapeB.Shape.Radius;
            float radiusSumSq = radiusSum * radiusSum;
            if (centreDistanceSq <= radiusSumSq)
            {
                float centreDistance = MathF.Sqrt(centreDistanceSq);
                var normal = Vector3.Normalize(shapeA.Position.Centre - shapeB.Position.Centre);
                var penetration = radiusSum - centreDistance;
                var contactPoint = shapeA.Position.Centre - shapeB.Shape.Radius * normal;
                return new CollisionManifold([contactPoint], normal, penetration);
            }
            return CollisionManifold.NoCollision();
        }
    }
}
