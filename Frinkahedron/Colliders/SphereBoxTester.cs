using System.Numerics;

namespace Frinkahedron.Core.Colliders
{
    public struct SphereBoxTester : ICollisionPairTester<Sphere, Box>
    {
        public static CollisionManifold Test(Collidable<Sphere> shapeA, Collidable<Box> shapeB)
        {
            return SphereBoxCollision(shapeB, shapeA).Invert();
        }

        public static CollisionManifold SphereBoxCollision(Collidable<Box> boxA, Collidable<Sphere> sphereB)
        {
            // treat the box as being at the origin with no rotation

            /*var transformA = boxA.Position.ToMatrix();
            var transformB = Matrix4x4.CreateTranslation(sphereB.Position.Centre); // ignore orientation

            _ = Matrix4x4.Invert(transformA, out var inverseA);
            var transformBToA = transformB * inverseA;// * inverseA;
            */
            Vector3 sphereCentre = boxA.Position.ToLocal(sphereB.Position.Centre);

            Vector3 centreA = new Vector3();
            Vector3 centreB = sphereCentre;
            float radius = sphereB.Shape.Radius;
            float radiusSq = radius * radius;
            Vector3 half = boxA.Shape.Dimensions * 0.5f;

            Vector3 min = -half;
            Vector3 max = half;

            Vector3 closestPoint = Vector3.Clamp(centreB, min, max);

            Vector3 difference = centreB - closestPoint;
            float distanceSq = difference.LengthSquared();

            if (distanceSq > radiusSq)
            {
                return CollisionManifold.NoCollision();
            }

            float distance = MathF.Sqrt(distanceSq);

            Vector3 normal;
            float penetration;

            if (distance > 0f)
            {
                normal = -difference / distance;
                penetration = radius - distance;
            }
            else
            {
                // Sphere center inside AABB – find nearest face
                Vector3 local = centreB - centreA;

                float dx = half.X - MathF.Abs(local.X);
                float dy = half.Y - MathF.Abs(local.Y);
                float dz = half.Z - MathF.Abs(local.Z);

                if (dx < dy && dx < dz)
                    normal = new Vector3(MathF.Sign(local.X), 0, 0);
                else if (dy < dz)
                    normal = new Vector3(0, MathF.Sign(local.Y), 0);
                else
                    normal = new Vector3(0, 0, MathF.Sign(local.Z));

                penetration = radius + MathF.Min(dx, MathF.Min(dy, dz));
                closestPoint = centreB - normal * radius;
            }

            normal = Vector3.Transform(normal, boxA.Position.Orientation);
            closestPoint = boxA.Position.ToWorld(closestPoint);

            return new CollisionManifold([closestPoint], normal, penetration);
        }
    }

    public struct BoxSphereTester : ICollisionPairTester<Box, Sphere>
    {
        public static CollisionManifold Test(Collidable<Box> shapeA, Collidable<Sphere> shapeB)
        {
            return SphereBoxTester.Test(shapeB, shapeA).Invert();
        }
    }
}
