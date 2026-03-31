using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using System.Numerics;

namespace Frinkahedron.Core.Colliders
{
    public sealed class Cylinder(float radius, float height) : IShape
    {
        public float Radius { get; } = radius;

        public float Height { get; } = height;

        public AxisAlignedBoundingBox CalculateAABB(Position position)
        {
            Vector3 worldAxis = Vector3.Transform(Vector3.UnitY, position.Orientation);
            float halfHeight = 0.5f * Height;

            float ex = MathF.Abs(worldAxis.X) * halfHeight + Radius * MathF.Sqrt(1 - worldAxis.X * worldAxis.X);
            float ey = MathF.Abs(worldAxis.Y) * halfHeight + Radius * MathF.Sqrt(1 - worldAxis.Y * worldAxis.Y);
            float ez = MathF.Abs(worldAxis.Z) * halfHeight + Radius * MathF.Sqrt(1 - worldAxis.Z * worldAxis.Z);

            Vector3 extent = new Vector3(ex, ey, ez);

            Vector3 min = position.Centre - extent;
            Vector3 max = position.Centre + extent;

            return new AxisAlignedBoundingBox(min, max);
        }

        public DiagonalMatrix3x3 CalculateFilledInertia(float mass)
        {
            return Inertia.CalculateFilledCylinderInertia(Radius, Height, mass);
        }

        public float CalculateVolume()
        {
            return MathF.PI * Radius * Radius * Height;
        }

        public void Draw(IRenderer renderer, Matrix4x4 position)
        {
            Matrix4x4 scale = Matrix4x4.CreateScale(Radius, Height, Radius);
            renderer.DrawCylinder(scale * position);

            //Matrix4x4 cap1Transform = Matrix4x4.CreateScale(-Radius, 1, Radius) * Matrix4x4.CreateTranslation(0, Height / 2, 0);
            //renderer.DrawDisc(cap1Transform * position);

            //Matrix4x4 cap2Transform = Matrix4x4.CreateScale(Radius, 1, Radius) * Matrix4x4.CreateTranslation(0, -Height / 2, 0);
            //renderer.DrawDisc(cap2Transform * position);
        }

        public bool RayIntersection(Position position, Vector3 rayPosition, Vector3 rayDirection, out Vector3 hitPoint)
        {
            hitPoint = Vector3.Zero;

            // Transform ray into cylinder local space
            Quaternion invRot = Quaternion.Inverse(position.Orientation);

            Vector3 ro = Vector3.Transform(rayPosition- position.Centre, invRot);
            Vector3 rd = Vector3.Normalize(Vector3.Transform(rayDirection, invRot));

            float halfHeight = height * 0.5f;

            float tMin = float.MaxValue;
            bool hit = false;

            // --- 1. Intersection with infinite cylinder (x^2 + z^2 = r^2) ---
            float a = rd.X * rd.X + rd.Z * rd.Z;
            float b = 2 * (ro.X * rd.X + ro.Z * rd.Z);
            float c = ro.X * ro.X + ro.Z * ro.Z - radius * radius;

            float discriminant = b * b - 4 * a * c;

            if (discriminant >= 0 && Math.Abs(a) > 1e-6f)
            {
                float sqrtD = (float)Math.Sqrt(discriminant);

                float t1 = (-b - sqrtD) / (2 * a);
                float t2 = (-b + sqrtD) / (2 * a);

                CheckCylinderSide(t1);
                CheckCylinderSide(t2);
            }

            void CheckCylinderSide(float t)
            {
                if (t < 0) return;

                float y = ro.Y + t * rd.Y;
                if (y >= -halfHeight && y <= halfHeight)
                {
                    if (t < tMin)
                    {
                        tMin = t;
                        hit = true;
                    }
                }
            }

            // --- 2. Check caps ---
            if (Math.Abs(rd.Y) > 1e-6f)
            {
                // Bottom cap (y = -halfHeight)
                float tBottom = (-halfHeight - ro.Y) / rd.Y;
                CheckCap(tBottom, -halfHeight);

                // Top cap (y = +halfHeight)
                float tTop = (halfHeight - ro.Y) / rd.Y;
                CheckCap(tTop, halfHeight);
            }

            void CheckCap(float t, float yPlane)
            {
                if (t < 0) return;

                Vector3 p = ro + t * rd;

                if (p.X * p.X + p.Z * p.Z <= radius * radius)
                {
                    if (t < tMin)
                    {
                        tMin = t;
                        hit = true;
                    }
                }
            }

            // --- Final result ---
            if (!hit)
                return false;

            Vector3 localHit = ro + tMin * rd;

            // Transform back to world space
            hitPoint = Vector3.Transform(localHit, position.Orientation) + position.Centre;

            return true;
        }
    }
}
