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
            // TODO: Not sure this works properly if, for example, the capsule is rolling off a big box at an angle

            var transformA = shapeA.Position.ToMatrix();
            var transformB = shapeB.Position.ToMatrix();

            _ = Matrix4x4.Invert(transformB, out var inverseB);
            var transformAtoB = transformA * inverseB;

            //var transformAtoB = transformA * Matrix4x4.CreateTranslation(-shapeB.Position.Centre);

            LineSegment segment = LineSegment.Transform(shapeA.Shape.GetPointToPointSegment(), transformAtoB);
            ClosestPoint(segment, shapeB.Shape, out var segPoint, out var boxPoint);
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
                boxPoint = segPoint - normal * radius;
            }

            normal = Vector3.Transform(normal, shapeB.Position.Orientation);
            boxPoint = Vector3.Transform(boxPoint, transformB);

            return new CollisionManifold([boxPoint], -normal, penetration);
        }

        public static void ClosestPoint(
            LineSegment segment,
            Box box,
            out Vector3 segPoint,
            out Vector3 boxPoint)
        {
            Vector3 p0 = segment.Point1;
            Vector3 p1 = segment.Point2;
            Vector3 boxMax = box.Dimensions / 2;
            Vector3 boxMin = -boxMax;

            Vector3 d = p1 - p0;
            float t = 0f;

            // initial closest point on segment
            segPoint = p0;

            // iterate axes
            for (int i = 0; i < 3; i++)
            {
                float segCoord = segPoint[i];

                if (segCoord < boxMin[i])
                {
                    float denom = d[i];
                    if (MathF.Abs(denom) > 1e-6f)
                    {
                        float newT = (boxMin[i] - p0[i]) / denom;
                        t = Math.Clamp(newT, 0f, 1f);
                        segPoint = p0 + d * t;
                    }
                }
                else if (segCoord > boxMax[i])
                {
                    float denom = d[i];
                    if (MathF.Abs(denom) > 1e-6f)
                    {
                        float newT = (boxMax[i] - p0[i]) / denom;
                        t = Math.Clamp(newT, 0f, 1f);
                        segPoint = p0 + d * t;
                    }
                }
            }

            boxPoint = Vector3.Clamp(segPoint, boxMin, boxMax);
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
