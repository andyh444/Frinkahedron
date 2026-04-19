using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using System.Numerics;

namespace Frinkahedron.Core.Colliders
{
    public sealed class Sphere(float radius) : IShape
    {
        public float Radius { get; } = radius;

        public AxisAlignedBoundingBox CalculateAABB(Position position)
        {
            float radiusPlus = 1.1f * Radius;

            return new AxisAlignedBoundingBox(
                position.Centre - new Vector3(radiusPlus, radiusPlus, radiusPlus),
                position.Centre + new Vector3(radiusPlus, radiusPlus, radiusPlus));
        }

        public DiagonalMatrix3x3 CalculateFilledInertia(float mass)
        {
            return Inertia.CalculateFilledSphereInertia(Radius, mass);
        }

        public float CalculateVolume()
        {
            return (4f / 3f) * MathF.PI * Radius * Radius * Radius;
        }

        public void Draw(IRenderContext renderer, Matrix4x4 position)
        {
            Matrix4x4 scale = Matrix4x4.CreateScale(Radius);
            //renderer.DrawEllipsoid(scale * position);
        }

        public bool RayIntersection(Position position, Vector3 rayPosition, Vector3 rayDirection, out Vector3 result, out Vector3 normal)
        {
            result = Vector3.Zero;
            normal = Vector3.Zero;

            Vector3 oc = rayPosition - position.Centre;
            Vector3 rd = Vector3.Normalize(rayDirection);

            float a = Vector3.Dot(rd, rd);
            float b = 2f * Vector3.Dot(oc, rd);
            float c = Vector3.Dot(oc, oc) - Radius * Radius;

            float discriminant = b * b - 4f * a * c;
            if (discriminant < 0f)
                return false;

            float sqrtD = MathF.Sqrt(discriminant);
            float t0 = (-b - sqrtD) / (2f * a);
            float t1 = (-b + sqrtD) / (2f * a);

            float t = float.MaxValue;
            if (t0 > 0f) t = t0;
            else if (t1 > 0f) t = t1;

            if (t == float.MaxValue)
                return false;

            result = rayPosition + rd * t;
            normal = Vector3.Normalize(result - position.Centre);

            return true;
        }
    }
}
