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

            Matrix4x4 cap1Transform = Matrix4x4.CreateScale(-Radius, 1, Radius) * Matrix4x4.CreateTranslation(0, Height / 2, 0);
            renderer.DrawDisc(cap1Transform * position);

            Matrix4x4 cap2Transform = Matrix4x4.CreateScale(Radius, 1, Radius) * Matrix4x4.CreateTranslation(0, -Height / 2, 0);
            renderer.DrawDisc(cap2Transform * position);
        }

        public bool RayIntersection(Position position, Vector3 rayPosition, Vector3 rayDirection, out Vector3 result)
        {
            result = Vector3.Zero;
            return false;
        }
    }
}
