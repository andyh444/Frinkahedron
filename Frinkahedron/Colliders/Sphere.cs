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
            return (4f / 3f) * Radius * Radius * Radius;
        }

        public void Draw(IRenderContext renderer, Matrix4x4 position)
        {
            Matrix4x4 scale = Matrix4x4.CreateScale(Radius);
            //renderer.DrawEllipsoid(scale * position);
        }

        public bool RayIntersection(Position position, Vector3 rayPosition, Vector3 rayDirection, out Vector3 result)
        {
            result = Vector3.Zero;
            return false;
        }
    }
}
