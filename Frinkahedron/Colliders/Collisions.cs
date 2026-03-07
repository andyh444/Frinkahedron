using Frinkahedron.Core.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Colliders
{
    public static class Collisions
    {
        public static CollisionManifold SphereSphereCollision(SphereCollider collider1, Position position1, SphereCollider collider2, Position position2)
        {
            float centreDistanceSq = Vector3.DistanceSquared(position1.Centre, position2.Centre);
            float radiusSum = collider1.Radius + collider2.Radius;
            float radiusSumSq = radiusSum * radiusSum;
            if (centreDistanceSq <= radiusSumSq)
            {
                float centreDistance = MathF.Sqrt(centreDistanceSq);
                var normal = Vector3.Normalize(position1.Centre - position2.Centre);
                var penetration = radiusSum - centreDistance;
                var contactPoint = position1.Centre - collider1.Radius * normal;
                return new CollisionManifold([contactPoint], normal, penetration);
            }
            return CollisionManifold.NoCollision();
        }

        public static CollisionManifold SphereAABBCollision(SphereCollider collider1, Position position1, BoxCollider collider2, Vector3 centre2)
        {
            Vector3 centre1 = position1.Centre;
            float radius = collider1.Radius;
            float radiusSq = radius * radius;
            Vector3 half = collider2.Dimensions * 0.5f;

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

        public static CollisionManifold AABBSphereCollision(BoxCollider collider1, Vector3 centre1, SphereCollider collider2, Position position2)
        {
            return SphereAABBCollision(collider2, position2, collider1, centre1).Invert();
        }
    }
}
