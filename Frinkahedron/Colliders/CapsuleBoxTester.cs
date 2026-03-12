using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Colliders
{
    public struct CapsuleBoxTester : ICollisionPairTester<Capsule, Box>
    {
        public static CollisionManifold Test(Collidable<Capsule> shapeA, Collidable<Box> shapeB)
        {
            var transformA = shapeA.Position.ToMatrix();
            var transformB = shapeB.Position.ToMatrix();

            _ = Matrix4x4.Invert(transformB, out var inverseB);
            var transformAtoB = transformA * inverseB;

            LineSegment segment = LineSegment.Transform(shapeA.Shape.GetPointToPointSegment(), transformAtoB);
            ClosestPtSegmentAABB(segment.Point1, segment.Point2, -shapeB.Shape.Dimensions / 2, shapeB.Shape.Dimensions / 2, out var segPoint, out var boxPoint);
            float distSq = Vector3.DistanceSquared(segPoint, boxPoint);
            Vector3 difference = segPoint - boxPoint;

            float radius = shapeA.Shape.Radius;
            float radiusSq = radius * radius;

            if (distSq > radiusSq)
            {
                return CollisionManifold.NoCollision();
            }

            float distance = MathF.Sqrt(distSq);
            Vector3 normal;
            float penetration;

            if (distance > 0f)
            {
                normal = -difference / distance;
                penetration = radius - distance;
            }
            else
            {
                // closest point is inside the box
                Vector3 local = segPoint;

                float dx = shapeB.Shape.Dimensions.X / 2 - MathF.Abs(local.X);
                float dy = shapeB.Shape.Dimensions.Y / 2 - MathF.Abs(local.Y);
                float dz = shapeB.Shape.Dimensions.Z / 2 - MathF.Abs(local.Z);

                if (dx < dy && dx < dz)
                    normal = new Vector3(MathF.Sign(local.X), 0, 0);
                else if (dy < dz)
                    normal = new Vector3(0, MathF.Sign(local.Y), 0);
                else
                    normal = new Vector3(0, 0, MathF.Sign(local.Z));

                penetration = radius + MathF.Min(dx, MathF.Min(dy, dz));
                boxPoint = segPoint - normal * MathF.Min(dx, MathF.Min(dy, dz));
            }

            normal = Vector3.Normalize(Vector3.TransformNormal(normal, transformB));
            boxPoint = Vector3.Transform(boxPoint, transformB);

            return new CollisionManifold([boxPoint], -normal, penetration);
        }

        static void ClosestPtSegmentAABB(
        Vector3 p0,
        Vector3 p1,
        Vector3 min,
        Vector3 max,
        out Vector3 segPoint,
        out Vector3 boxPoint)
        {
            Vector3 d = p1 - p0;
            float t = 0f;

            segPoint = p0;

            boxPoint = Vector3.Clamp(segPoint, min, max);

            Vector3 diff = segPoint - boxPoint;
            float distSq = Vector3.Dot(diff, diff);

            float bestDistSq = distSq;
            float bestT = 0f;

            const int STEPS = 8;

            for (int i = 1; i <= STEPS; i++)
            {
                float testT = i / (float)STEPS;
                Vector3 point = p0 + d * testT;

                Vector3 clamped = Vector3.Clamp(point, min, max);

                Vector3 delta = point - clamped;
                float dsq = Vector3.Dot(delta, delta);

                if (dsq < bestDistSq)
                {
                    bestDistSq = dsq;
                    bestT = testT;
                    segPoint = point;
                    boxPoint = clamped;
                }
            }
        }
    }

    public struct BoxCapsuleTester : ICollisionPairTester<Box, Capsule>
    {
        public static CollisionManifold Test(Collidable<Box> shapeA, Collidable<Capsule> shapeB)
        {
            return CapsuleBoxTester.Test(shapeB, shapeA).Invert();
        }
    }

}
