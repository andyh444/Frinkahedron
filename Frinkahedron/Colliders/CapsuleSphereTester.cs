using Frinkahedron.Core.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Colliders
{
    public struct CapsuleSphereTester : ICollisionPairTester<Capsule, Sphere>
    {
        public static CollisionManifold Test(Collidable<Capsule> shapeA, Collidable<Sphere> shapeB)
        {
            var seg = LineSegment.Transform(shapeA.Shape.GetPointToPointSegment(), shapeA.Position);

            float centreDistanceSq = seg.DistanceToSquared(shapeB.Position.Centre);
            float radiusSum = shapeA.Shape.Radius + shapeB.Shape.Radius;
            float radiusSumSq = radiusSum * radiusSum;
            if (centreDistanceSq <= radiusSumSq)
            {
                var segPoint = seg.ClosestPointTo(shapeB.Position.Centre);

                float centreDistance = MathF.Sqrt(centreDistanceSq);
                var normal = Vector3.Normalize(segPoint - shapeB.Position.Centre);
                var penetration = radiusSum - centreDistance;
                var contactPoint = segPoint - shapeA.Shape.Radius * normal;
                return new CollisionManifold([contactPoint], normal, penetration);
            }
            return CollisionManifold.NoCollision();
        }
    }

    public struct SphereCapsuleTester : ICollisionPairTester<Sphere, Capsule>
    {
        public static CollisionManifold Test(Collidable<Sphere> shapeA, Collidable<Capsule> shapeB)
        {
            return CapsuleSphereTester.Test(shapeB, shapeA).Invert();
        }
    }
}
