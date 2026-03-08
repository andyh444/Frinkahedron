using System.Numerics;

namespace Frinkahedron.Core.Colliders
{
    public struct SphereBoxTester : ICollisionPairTester<Sphere, Box>
    {
        public static CollisionManifold Test(Collidable<Sphere> shapeA, Collidable<Box> shapeB)
        {
            if (shapeB.Position.Orientation.IsIdentity)
            {
                return SphereAABBCollision(shapeA, shapeB);
            }
            return CollisionManifold.NoCollision();
        }

        public static CollisionManifold SphereAABBCollision(Collidable<Sphere> shapeA, Collidable<Box> shapeB)
        {
            Vector3 centre1 = shapeA.Position.Centre;
            Vector3 centre2 = shapeB.Position.Centre;
            float radius = shapeA.Shape.Radius;
            float radiusSq = radius * radius;
            Vector3 half = shapeB.Shape.Dimensions * 0.5f;

            Vector3 min = centre2 - half;
            Vector3 max = centre2 + half;

            Vector3 closestPoint = Vector3.Clamp(centre1, min, max);

            Vector3 difference = centre1 - closestPoint;
            float distanceSq = difference.LengthSquared();

            if (distanceSq > radiusSq)
                return CollisionManifold.NoCollision();

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
                Vector3 local = centre1 - centre2;

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
                closestPoint = centre1 - normal * radius;
            }

            return new CollisionManifold([closestPoint], -normal, penetration);
        }
    }
}
