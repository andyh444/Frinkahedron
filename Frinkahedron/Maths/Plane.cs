using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Maths
{
    public readonly struct Plane(Vector3 point, Vector3 normal)
    {
        public Vector3 Point { get; } = point;

        public Vector3 Normal { get; } = Vector3.Normalize(normal);

        public float DistanceTo(Vector3 point)
        {
            return Vector3.Dot(Point, Normal) - Vector3.Dot(Normal, point);
        }

        public Vector3 ProjectOntoPlane(Vector3 point)
        {
            return point + DistanceTo(point) * Normal;
        }

        public int IntersectCirclePlane(
            Vector3 circleCenter,
            Vector3 circleNormal,
            float radius,
            out Vector3 p0,
            out Vector3 p1)
        {
            const float EPS = 1e-6f;

            p0 = default;
            p1 = default;

            circleNormal = Vector3.Normalize(circleNormal);

            // Step 1: Compute line of intersection between planes
            Vector3 lineDir = Vector3.Cross(circleNormal, Normal);
            float denom = lineDir.LengthSquared();

            // If planes are parallel
            if (denom < EPS)
            {
                float dist = Vector3.Dot(Normal, circleCenter - Point);

                // Circle lies in plane → infinite intersections
                if (MathF.Abs(dist) < EPS)
                    return -1;

                // No intersection
                return 0;
            }

            // Find a point on the intersection line
            float d1 = -Vector3.Dot(circleNormal, circleCenter);
            float d2 = -Vector3.Dot(Normal, Point);

            Vector3 pointOnLine = Vector3.Cross(
                (Normal * d1 - circleNormal * d2),
                lineDir) / denom;

            // Step 2: Intersect line with circle
            // Line: L(t) = pointOnLine + t * lineDir

            Vector3 m = pointOnLine - circleCenter;

            float a = Vector3.Dot(lineDir, lineDir);
            float b = 2f * Vector3.Dot(m, lineDir);
            float c = Vector3.Dot(m, m) - radius * radius;

            float discriminant = b * b - 4f * a * c;

            if (discriminant < -EPS)
                return 0;

            if (MathF.Abs(discriminant) < EPS)
            {
                float t = -b / (2f * a);
                p0 = pointOnLine + t * lineDir;
                return 1;
            }

            float sqrtD = MathF.Sqrt(discriminant);

            float t0 = (-b - sqrtD) / (2f * a);
            float t1 = (-b + sqrtD) / (2f * a);

            p0 = pointOnLine + t0 * lineDir;
            p1 = pointOnLine + t1 * lineDir;

            return 2;
        }

        public Vector3 ClosestPointOnCircle(
            Vector3 circleCenter,
            Vector3 circleNormal,
            float radius)
        {
            // Ensure normals are normalized
            circleNormal = Vector3.Normalize(circleNormal);
            float planeD = -Vector3.Dot(Point, Normal);

            // Build orthonormal basis (U, V) for the circle plane
            Vector3 U = Vector3.Normalize(
                Math.Abs(circleNormal.X) > 0.9f
                    ? Vector3.Cross(circleNormal, Vector3.UnitY)
                    : Vector3.Cross(circleNormal, Vector3.UnitX)
            );
            Vector3 V = Vector3.Cross(circleNormal, U);

            // Precompute coefficients
            float A = Vector3.Dot(Normal, circleCenter) + planeD;
            float a = Vector3.Dot(Normal, U);
            float b = Vector3.Dot(Normal, V);

            float amplitude = radius * MathF.Sqrt(a * a + b * b);

            float t;

            // Check if circle intersects plane (can make distance zero)
            if (amplitude > 1e-6f && MathF.Abs(A) <= amplitude)
            {
                // Solve A + r*(a cos t + b sin t) = 0
                float phi = MathF.Atan2(b, a);
                float cosTerm = -A / amplitude;
                float delta = MathF.Acos(Math.Clamp(cosTerm, -1f, 1f));

                // Two solutions; pick either (both lie on plane)
                t = phi + delta;
            }
            else
            {
                // Minimize absolute distance → check extrema
                float t0 = MathF.Atan2(b, a);
                float t1 = t0 + MathF.PI;

                float f0 = A + radius * (a * MathF.Cos(t0) + b * MathF.Sin(t0));
                float f1 = A + radius * (a * MathF.Cos(t1) + b * MathF.Sin(t1));

                t = MathF.Abs(f0) < MathF.Abs(f1) ? t0 : t1;
            }

            // Compute closest point
            return circleCenter + radius * (MathF.Cos(t) * U + MathF.Sin(t) * V);
        }

        public float CirclePlanePenetration(
        Vector3 circleCenter,
        Vector3 circleNormal,
        float radius,
        out Vector3 penetrationPoint)
        {
            // Normalize normals
            circleNormal = Vector3.Normalize(circleNormal);

            // Build orthonormal basis (U, V)
            Vector3 U = Vector3.Normalize(
                Math.Abs(circleNormal.X) > 0.9f
                    ? Vector3.Cross(circleNormal, Vector3.UnitY)
                    : Vector3.Cross(circleNormal, Vector3.UnitX)
            );
            Vector3 V = Vector3.Cross(circleNormal, U);

            // Coefficients
            float planeD = -Vector3.Dot(Normal, Point);
            float A = Vector3.Dot(Normal, circleCenter) + planeD;
            float a = Vector3.Dot(Normal, U);
            float b = Vector3.Dot(Normal, V);

            float len = MathF.Sqrt(a * a + b * b);

            // Handle degenerate case: circle plane parallel to plane normal projection
            float t;
            if (len < 1e-6f)
            {
                // All points have same distance → pick arbitrary point on circle
                t = 0f;
            }
            else
            {
                float phi = MathF.Atan2(b, a);
                t = phi + MathF.PI; // direction of minimum
            }

            // Compute penetration point
            penetrationPoint = circleCenter +
                               radius * (MathF.Cos(t) * U + MathF.Sin(t) * V);

            // Compute minimum distance
            float amplitude = radius * len;
            float minDist = A - amplitude;

            // Return penetration depth
            return MathF.Max(0f, -minDist);
        }

        public bool RayPlaneIntersection(Vector3 rayPos, Vector3 rayDir, out Vector3 intersection)
        {
            // Calculate the denominator
            float denominator = Vector3.Dot(Normal, rayDir);

            // If the denominator is zero, the ray is parallel to the plane
            if (Math.Abs(denominator) < float.Epsilon)
            {
                intersection = Vector3.Zero;
                return false;
            }

            // Calculate the distance from the ray origin to the plane
            float t = DistanceTo(rayPos) / denominator;

            // If t is negative, the intersection is behind the ray origin
            if (t < 0)
            {
                intersection = Vector3.Zero;
                return false;
            }

            // Calculate the intersection point
            intersection = rayPos + t * rayDir;
            return true;
        }
    }
}
