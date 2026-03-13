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
    }
}
