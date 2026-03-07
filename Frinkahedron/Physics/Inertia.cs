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
        public static Matrix3x3 CalculateFilledCubeInertia(Vector3 dimensions, float mass)
        {
            const float TWELFTH = 1f / 12f;

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

            return new Matrix3x3(
                new Vector3(ix, 0, 0),
                new Vector3(0, iy, 0),
                new Vector3(0, 0, iz));
        }

        public static Matrix3x3 CalculateFilledSphereInertia(float radius, float mass)
        {
            float i = (2f / 5f) * mass * radius * radius;
            return new Matrix3x3(
                new Vector3(i, 0, 0),
                new Vector3(0, i, 0),
                new Vector3(0, 0, i);
        }
    }
}
