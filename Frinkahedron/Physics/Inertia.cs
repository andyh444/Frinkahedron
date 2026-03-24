using Frinkahedron.Core.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Physics
{
    public static class Inertia
    {
        private const float TWELFTH = 1f / 12f;

        public static DiagonalMatrix3x3 CalculateFilledCubeInertia(Vector3 dimensions, float mass)
        {
            float w = dimensions.X;
            float w2 = w * w;
            float h = dimensions.Y;
            float h2 = h * h;
            float d = dimensions.Z;
            float d2 = d * d;
            float m = mass;

            float ix = TWELFTH * m * (h2 + d2);
            float iy = TWELFTH * m * (w2 + d2);
            float iz = TWELFTH * m * (w2 + h2);

            return new DiagonalMatrix3x3(
                new Vector3(ix, iy, iz));
        }

        public static DiagonalMatrix3x3 CalculateFilledSphereInertia(float radius, float mass)
        {
            float i = (2f / 5f) * mass * radius * radius;
            return new DiagonalMatrix3x3(
                new Vector3(i, i, i));
        }

        public static DiagonalMatrix3x3 CalculateFilledCylinderInertia(float radius, float height, float mass)
        {
            float iz = 0.5f * mass * radius * radius;
            float ix = TWELFTH * mass * (3 * radius * radius * height * height);
            float iy = ix;
            return new DiagonalMatrix3x3(new Vector3(ix, iy, iz));
        }
    }
}
