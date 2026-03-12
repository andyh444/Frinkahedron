using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Maths
{
    public struct LineSegment(Vector3 point1, Vector3 point2)
    {
        public Vector3 Point1 = point1;
        public Vector3 Point2 = point2;

        public static LineSegment Transform(LineSegment segment, Matrix4x4 transform)
        {
            return new LineSegment(
                Vector3.Transform(segment.Point1, transform),
                Vector3.Transform(segment.Point2, transform));
        }

        public Vector3 ClosestPointTo(LineSegment other)
        {
            const float EPSILON = 1e-6f;

            Vector3 p1 = Point1;
            Vector3 q1 = Point2;
            Vector3 p2 = other.Point1;
            Vector3 q2 = other.Point2;

            Vector3 d1 = q1 - p1; // direction of this segment
            Vector3 d2 = q2 - p2; // direction of other segment
            Vector3 r = p1 - p2;

            float a = Vector3.Dot(d1, d1); // length^2 of segment1
            float e = Vector3.Dot(d2, d2); // length^2 of segment2
            float f = Vector3.Dot(d2, r);

            float s, t;

            if (a <= EPSILON && e <= EPSILON)
            {
                return p1;
            }

            if (a <= EPSILON)
            {
                s = 0f;
                t = Math.Clamp(f / e, 0f, 1f);
            }
            else
            {
                float c = Vector3.Dot(d1, r);

                if (e <= EPSILON)
                {
                    t = 0f;
                    s = Math.Clamp(-c / a, 0f, 1f);
                }
                else
                {
                    float b = Vector3.Dot(d1, d2);
                    float denom = a * e - b * b;

                    if (denom != 0f)
                        s = Math.Clamp((b * f - c * e) / denom, 0f, 1f);
                    else
                        s = 0f;

                    t = (b * s + f) / e;

                    if (t < 0f)
                    {
                        t = 0f;
                        s = Math.Clamp(-c / a, 0f, 1f);
                    }
                    else if (t > 1f)
                    {
                        t = 1f;
                        s = Math.Clamp((b - c) / a, 0f, 1f);
                    }
                }
            }

            return p1 + d1 * s;
        }

        public float DistanceToSquared(Vector3 point)
        {
            Vector3 ab = Point2 - Point1;
            float abLenSq = Vector3.Dot(ab, ab);

            if (abLenSq == 0f)
                return Vector3.DistanceSquared(point, Point1);

            float t = Vector3.Dot(point - Point1, ab) / abLenSq;
            t = Math.Clamp(t, 0f, 1f);

            Vector3 closest = Point1 + ab * t;
            return Vector3.DistanceSquared(point, closest);
        }
    }
}
