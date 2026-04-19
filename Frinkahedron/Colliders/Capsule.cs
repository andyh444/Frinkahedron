using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using System.Numerics;

namespace Frinkahedron.Core.Colliders
{
    public sealed class Capsule(float pointToPointLength, float radius) : IShape
    {
        public float PointToPointLength { get; } = pointToPointLength;

        public float Radius { get; } = radius;

        public DiagonalMatrix3x3 CalculateFilledInertia(float mass)
        {
            float r = Radius;
            float h = PointToPointLength;

            float cylinderVolume = MathF.PI * r * r * h;
            float sphereVolume = (4f / 3f) * MathF.PI * r * r * r;
            float totalVolume = cylinderVolume + sphereVolume;

            float cylinderMass = mass * cylinderVolume / totalVolume;
            float hemisphereMass = mass * (sphereVolume * 0.5f) / totalVolume;

            float r2 = r * r;

            // Cylinder inertia
            float Iaxis = 0.5f * cylinderMass * r2;
            float Iperp = (1f / 12f) * cylinderMass * (3f * r2 + h * h);

            float i1 = Iperp;
            float i2 = Iaxis;
            float i3 = Iperp;

            // Hemisphere COM offset
            float d = h * 0.5f + 3f * r / 8f;

            // Hemisphere inertia about its COM
            float Ihem_axis = (2f / 5f) * hemisphereMass * r2;
            float Ihem_perp = (83f / 320f) * hemisphereMass * r2;

            // Apply parallel axis theorem
            float Iaxis_shift = Ihem_axis;
            float Iperp_shift = Ihem_perp + hemisphereMass * d * d;

            i2 += 2f * Iaxis_shift;
            i1 += 2f * Iperp_shift;
            i3 += 2f * Iperp_shift;

            return new DiagonalMatrix3x3(new Vector3(i1, i2, i3));
        }

        public LineSegment GetPointToPointSegment()
        {
            return new LineSegment(
                new Vector3(0, 0.5f * PointToPointLength, 0),
                new Vector3(0, -0.5f * PointToPointLength, 0));
        }

        public float CalculateVolume()
        {
            float r = Radius;
            float h = PointToPointLength;

            float cylinderVolume = MathF.PI * r * r * h;
            float sphereVolume = (4f / 3f) * MathF.PI * r * r * r;
            return cylinderVolume + sphereVolume;
        }

        public void Draw(IRenderContext renderer, Matrix4x4 position)
        {
            Matrix4x4 cylinderScale = Matrix4x4.CreateScale(Radius, PointToPointLength, Radius);
            //renderer.DrawCylinder(cylinderScale * position);

            Matrix4x4 sphereScale = Matrix4x4.CreateScale(Radius);
            Matrix4x4 sphere1Translation = Matrix4x4.CreateTranslation(0, 0.5f * PointToPointLength, 0);
            Matrix4x4 sphere2Translation = Matrix4x4.CreateTranslation(0, -0.5f * PointToPointLength, 0);

            //renderer.DrawEllipsoid(sphereScale * sphere1Translation * position);
            //renderer.DrawEllipsoid(sphereScale * sphere2Translation * position);
        }

        public AxisAlignedBoundingBox CalculateAABB(Position position)
        {
            Vector3 point1 = position.ToWorld(new Vector3(0, 0.5f * PointToPointLength, 0));
            Vector3 point2 = position.ToWorld(new Vector3(0, -0.5f * PointToPointLength, 0));

            Vector3 min =
                Vector3.Min(
                    point1 - new Vector3(Radius, Radius, Radius),
                    point2 - new Vector3(Radius, Radius, Radius));

            Vector3 max =
                Vector3.Max(
                    point1 + new Vector3(Radius, Radius, Radius),
                    point2 + new Vector3(Radius, Radius, Radius));

            return new AxisAlignedBoundingBox(min, max);
        }

        public bool RayIntersection(Position position, Vector3 rayPosition, Vector3 rayDirection, out Vector3 result, out Vector3 normal)
        {
            result = Vector3.Zero;
            normal = Vector3.Zero;

            // Transform ray into local space
            Quaternion invRot = Quaternion.Inverse(position.Orientation);
            Vector3 ro = Vector3.Transform(rayPosition - position.Centre, invRot);
            Vector3 rd = Vector3.Normalize(Vector3.Transform(rayDirection, invRot));

            float halfHeight = PointToPointLength * 0.5f;
            float r = Radius;

            float tMin = float.MaxValue;
            bool hit = false;
            int hitType = 0; // 1 = top sphere, -1 = bottom sphere, 0 = side

            // --- 1. Cylinder side (infinite cylinder x^2 + z^2 = r^2), limited by y
            float a = rd.X * rd.X + rd.Z * rd.Z;
            float b = 2f * (ro.X * rd.X + ro.Z * rd.Z);
            float c = ro.X * ro.X + ro.Z * ro.Z - r * r;

            float discriminant = b * b - 4f * a * c;
            if (discriminant >= 0f && Math.Abs(a) > 1e-6f)
            {
                float sqrtD = MathF.Sqrt(discriminant);
                float t1 = (-b - sqrtD) / (2f * a);
                float t2 = (-b + sqrtD) / (2f * a);

                void CheckSide(float t)
                {
                    if (t < 0f) return;
                    float y = ro.Y + t * rd.Y;
                    if (y >= -halfHeight && y <= halfHeight)
                    {
                        if (t < tMin)
                        {
                            tMin = t;
                            hit = true;
                            hitType = 0;
                        }
                    }
                }

                CheckSide(t1);
                CheckSide(t2);
            }

            // --- 2. End spheres
            Vector3 topCenter = new Vector3(0f, halfHeight, 0f);
            Vector3 bottomCenter = new Vector3(0f, -halfHeight, 0f);

            void CheckSphere(Vector3 center, int type)
            {
                Vector3 oc = ro - center;
                float A = Vector3.Dot(rd, rd);
                float B = 2f * Vector3.Dot(oc, rd);
                float C = Vector3.Dot(oc, oc) - r * r;

                float disc = B * B - 4f * A * C;
                if (disc < 0f) return;

                float sqrtD = MathF.Sqrt(disc);
                float t0 = (-B - sqrtD) / (2f * A);
                float t1 = (-B + sqrtD) / (2f * A);

                if (t0 > 0f && t0 < tMin)
                {
                    tMin = t0;
                    hit = true;
                    hitType = type;
                }
                else if (t1 > 0f && t1 < tMin)
                {
                    tMin = t1;
                    hit = true;
                    hitType = type;
                }
            }

            CheckSphere(topCenter, 1);
            CheckSphere(bottomCenter, -1);

            if (!hit)
                return false;

            Vector3 localHit = ro + tMin * rd;

            result = Vector3.Transform(localHit, position.Orientation) + position.Centre;

            // Compute normal in local space
            Vector3 localNormal;
            if (hitType == 0)
            {
                localNormal = Vector3.Normalize(new Vector3(localHit.X, 0f, localHit.Z));
            }
            else if (hitType == 1)
            {
                localNormal = Vector3.Normalize(localHit - topCenter);
            }
            else
            {
                localNormal = Vector3.Normalize(localHit - bottomCenter);
            }

            normal = Vector3.Normalize(Vector3.Transform(localNormal, position.Orientation));

            return true;
        }
    }
}
