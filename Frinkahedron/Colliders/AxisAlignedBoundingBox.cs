using Frinkahedron.Core.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Colliders
{
    public readonly struct AxisAlignedBoundingBox(Vector3 min, Vector3 max)
    {
        public Vector3 Min { get; } = min;
        public Vector3 Max { get; } = max;

        public bool IntersectsWith(AxisAlignedBoundingBox other)
        {
            return Min.X <= other.Max.X
                && Max.X >= other.Min.X
                && Min.Y <= other.Max.Y
                && Max.Y >= other.Min.Y
                && Min.Z <= other.Max.Z
                && Max.Z >= other.Min.Z;
        }

        public void Project(Vector3 axis, out float min, out float max)
        {
            // OBB local axes in world space
            Vector3 u0 = Vector3.UnitX;
            Vector3 u1 = Vector3.UnitY;
            Vector3 u2 = Vector3.UnitZ;

            Vector3 position = 0.5f * (Min + Max);
            Vector3 halfExtent = 0.5f * (Max - Min);

            // Project center onto axis
            float centerProjection = Vector3.Dot(position, axis);

            // Compute projection radius
            float r =
                halfExtent.X * MathF.Abs(Vector3.Dot(axis, u0)) +
                halfExtent.Y * MathF.Abs(Vector3.Dot(axis, u1)) +
                halfExtent.Z * MathF.Abs(Vector3.Dot(axis, u2));

            min = centerProjection - r;
            max = centerProjection + r;
        }
    }
}
