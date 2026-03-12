using Frinkahedron.Core.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Colliders
{
    internal struct CapsuleCapsuleTester : ICollisionPairTester<Capsule, Capsule>
    {
        public static CollisionManifold Test(Collidable<Capsule> shapeA, Collidable<Capsule> shapeB)
        {
            Matrix4x4 transformA = shapeA.Position.ToMatrix();
            Matrix4x4 transformB = shapeB.Position.ToMatrix();

            LineSegment segA = LineSegment.Transform(shapeA.Shape.GetPointToPointSegment(), transformA);
            LineSegment segB = LineSegment.Transform(shapeB.Shape.GetPointToPointSegment(), transformB);

            Vector3 closestPointA = segA.ClosestPointTo(segB);
            Vector3 closestPointB = segB.ClosestPointTo(segA);

            float distanceSq = Vector3.DistanceSquared(closestPointA, closestPointB);

            float radiusSum = shapeA.Shape.Radius + shapeB.Shape.Radius;
            float radiusSumSq = radiusSum * radiusSum;
            if (distanceSq <= radiusSumSq)
            {
                float distance = MathF.Sqrt(distanceSq);
                var penetration = radiusSum - distance;
                var normal = Vector3.Normalize(closestPointA - closestPointB);
                var contactPoint = closestPointA - shapeA.Shape.Radius * normal;

                return new CollisionManifold([contactPoint], normal, penetration);
            }
            return CollisionManifold.NoCollision();
        }
    }
}
