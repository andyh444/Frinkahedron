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
            float Iaxis_shift = Ihem_axis + hemisphereMass * d * d;
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
            renderer.DrawCylinder(cylinderScale * position);

            Matrix4x4 sphereScale = Matrix4x4.CreateScale(Radius);
            Matrix4x4 sphere1Translation = Matrix4x4.CreateTranslation(0, 0.5f * PointToPointLength, 0);
            Matrix4x4 sphere2Translation = Matrix4x4.CreateTranslation(0, -0.5f * PointToPointLength, 0);

            renderer.DrawEllipsoid(sphereScale * sphere1Translation * position);
            renderer.DrawEllipsoid(sphereScale * sphere2Translation * position);
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

        public bool RayIntersection(Position position, Vector3 rayPosition, Vector3 rayDirection, out Vector3 result)
        {
            result = Vector3.Zero;
            return false;
        }
    }
}
