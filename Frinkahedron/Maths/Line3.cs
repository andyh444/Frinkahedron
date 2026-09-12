using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Maths
{
    public sealed class Line3(Vector3 origin, Vector3 direction)
    {
        public Vector3 Origin { get; } = origin;

        public Vector3 Direction { get; } = Vector3.Normalize(direction);

        public Vector3 ClosestPointTo(Line3 other)
        {
            Vector3 p1 = Origin;
            Vector3 d1 = Direction;
            Vector3 p2 = other.Origin;
            Vector3 d2 = other.Direction;

            // First line: p1 + t * d1
            // Second line: p2 + s * d2

            Vector3 r = p1 - p2;

            float a = Vector3.Dot(d1, d1);
            float b = Vector3.Dot(d1, d2);
            float c = Vector3.Dot(d2, d2);
            float d = Vector3.Dot(d1, r);
            float e = Vector3.Dot(d2, r);

            float denominator = a * c - b * b;

            // Parallel lines
            if (MathF.Abs(denominator) < 1e-6f)
            {
                // Any point on the first line is equally close in the
                // direction perpendicular to the two parallel lines.
                float t = -d / a;
                return p1 + t * d1;
            }
            else
            {
                // Parameter of closest point on the first line
                float t = (b * e - c * d) / denominator;

                return p1 + t * d1;
            }
        }
    }
}
