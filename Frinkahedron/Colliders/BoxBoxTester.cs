using Frinkahedron.Core.Physics;
using System.Diagnostics;
using System.Numerics;

namespace Frinkahedron.Core.Colliders
{
    public struct BoxBoxTester : ICollisionPairTester<Box, Box>
    {
        public static CollisionManifold Test(Collidable<Box> shapeA, Collidable<Box> shapeB)
        {
            return BoxBoxCollision(shapeA.Shape, shapeA.Position, shapeB.Shape, shapeB.Position);
        }

        private static CollisionManifold BoxBoxCollision(
            Box boxA,
            Position positionA,
            Box boxB,
            Position positionB)
        {
            Span<Vector3> axesA = stackalloc Vector3[] {
                Vector3.Transform(Vector3.UnitX, positionA.Orientation),
                Vector3.Transform(Vector3.UnitY, positionA.Orientation),
                Vector3.Transform(Vector3.UnitZ, positionA.Orientation),
            };

            Span<Vector3> axesB = stackalloc Vector3[] {
                Vector3.Transform(Vector3.UnitX, positionB.Orientation),
                Vector3.Transform(Vector3.UnitY, positionB.Orientation),
                Vector3.Transform(Vector3.UnitZ, positionB.Orientation),
            };

            Vector3 bestNormal = default;
            float bestPenetration = float.MaxValue;

            if (!TestAxes(positionA.Centre, boxA.Dimensions / 2, axesA, positionB.Centre, boxB.Dimensions / 2, axesB, axesA, ref bestNormal, ref bestPenetration))
            {
                return CollisionManifold.NoCollision();
            }

            //if (!TestAxes(cornersA, cornersB, axesB, ref bestNormal, ref bestPenetration))
            if (!TestAxes(positionA.Centre, boxA.Dimensions / 2, axesA, positionB.Centre, boxB.Dimensions / 2, axesB, axesB, ref bestNormal, ref bestPenetration))
            {
                return CollisionManifold.NoCollision();
            }
            
            Span<Vector3> crossAxes = stackalloc Vector3[9];
            int index = 0;
            foreach (var axisA in axesA)
            {
                foreach (var axisB in axesB)
                {
                    var cross = Vector3.Cross(axisA, axisB);
                    var lengthSq = cross.LengthSquared();
                    if (lengthSq > 1e-9f)
                    {
                        crossAxes[index++] = Vector3.Normalize(cross);
                    }
                }
            }

            if (!TestAxes(positionA.Centre, boxA.Dimensions / 2, axesA, positionB.Centre, boxB.Dimensions / 2, axesB, crossAxes[..index], ref bestNormal, ref bestPenetration))
            //if (!TestAxes(cornersA, cornersB, crossAxes[..index], ref bestNormal, ref bestPenetration))
            {
                return CollisionManifold.NoCollision();
            }
            // normal needs to point from B to A
            Vector3 centerA = positionA.Centre;
            Vector3 centerB = positionB.Centre;
            if (Vector3.Dot(centerA - centerB, bestNormal) < 0f)
            {
                bestNormal = -bestNormal;
            }

            var contactPoints = CalculateContactPoints(boxA, positionA, axesA, boxB, positionB, axesB, bestNormal, bestPenetration);
            if (contactPoints.Length == 0)
            {
                // when it fails, doing the same thing in reverse often seems to just do the trick
                // TODO: Get rid of the need for this
                contactPoints = CalculateContactPoints(boxB, positionB, axesB, boxA, positionA, axesA, -bestNormal, bestPenetration);
            }
            return new CollisionManifold(contactPoints, bestNormal, bestPenetration);
        }

        private static Vector3[] CalculateContactPoints(
            Box boxA,
            Position positionA,
            Span<Vector3> axesA,
            Box boxB,
            Position positionB,
            Span<Vector3> axesB,
            Vector3 normal,
            float penetration)
        {
            Face faceA = Face.GetFace(positionA.Centre, axesA, boxA.Dimensions / 2, -normal); // note, this was originally +normal and faceB was -normal
            Face faceB = Face.GetFace(positionB.Centre, axesB, boxB.Dimensions / 2, normal);

            var intersection = ClipFaces(faceA, faceB);

            var contacts = new List<(Vector3, float)>();
            foreach (var p in intersection)
            {
                float depth = Vector3.Dot(faceA.Centre - p, normal);

                if (depth <= 1e-3f) // this was swapped when the normals were the other way around
                {
                    contacts.Add((p, depth));
                }
            }
            if (contacts.Count == 0)
            {
                //Debugger.Break();
            }
            if (contacts.Count <= 4)
            {
                return contacts.Select(x => x.Item1).ToArray();
            }
            return PickFourPoints(contacts);
        }

        private static Vector3[] PickFourPoints(List<(Vector3 Position, float Depth)> candidates)
        {
            int deepest = 0;
            float maxDepth = candidates[0].Depth;

            for (int i = 1; i < candidates.Count; i++)
            {
                if (candidates[i].Depth > maxDepth)
                {
                    deepest = i;
                    maxDepth = candidates[i].Depth;
                }
            }

            int farthest = deepest;
            float maxDist = 0f;

            for (int i = 0; i < candidates.Count; i++)
            {
                float d = Vector3.DistanceSquared(
                    candidates[i].Position,
                    candidates[deepest].Position);

                if (d > maxDist)
                {
                    maxDist = d;
                    farthest = i;
                }
            }

            int third = deepest;
            float maxLineDist = 0f;

            Vector3 a = candidates[deepest].Position;
            Vector3 b = candidates[farthest].Position;

            for (int i = 0; i < candidates.Count; i++)
            {
                Vector3 p = candidates[i].Position;

                float dist = Vector3.Cross(p - a, b - a).LengthSquared();

                if (dist > maxLineDist)
                {
                    maxLineDist = dist;
                    third = i;
                }
            }

            int fourth = deepest;
            float maxPlaneDist = 0f;

            Vector3 c = candidates[third].Position;

            Vector3 normal = Vector3.Cross(b - a, c - a);

            for (int i = 0; i < candidates.Count; i++)
            {
                Vector3 p = candidates[i].Position;

                float dist = MathF.Abs(Vector3.Dot(p - a, normal));

                if (dist > maxPlaneDist)
                {
                    maxPlaneDist = dist;
                    fourth = i;
                }
            }

            return
            [
                candidates[deepest].Position,
                candidates[farthest].Position,
                candidates[third].Position,
                candidates[fourth].Position
            ];
        }

        private static List<Vector3> ClipFaces(Face reference, Face incident)
        {
            var polygon = incident.GetVertices();
            List<Vector3> outputBuffer = new List<Vector3>(8);

            var t1 = reference.Tangent1;
            var t2 = reference.Tangent2;

            var e1 = reference.HalfExtent1;
            var e2 = reference.HalfExtent2;

            // 4 edge planes of the reference face
            Span<(Vector3 point, Vector3 normal)> planes = stackalloc (Vector3 point, Vector3 normal)[]
            {
                (reference.Centre + t1 * e1,  t1),
                (reference.Centre - t1 * e1, -t1),
                (reference.Centre + t2 * e2,  t2),
                (reference.Centre - t2 * e2, -t2),
            };

            foreach (var (point, normal) in planes)
            {
                ClipPolygon(polygon, outputBuffer, point, normal);
                (polygon, outputBuffer) = (outputBuffer, polygon);
                if (polygon.Count == 0)
                    break;
            }

            return polygon;
        }

        private static void ClipPolygon(List<Vector3> input, List<Vector3> output, Vector3 planePoint, Vector3 planeNormal)
        {
            output.Clear();
            for (int i = 0; i < input.Count; i++)
            {
                var a = input[i];
                var b = input[(i + 1) % input.Count];

                float da = Vector3.Dot(a - planePoint, planeNormal);
                float db = Vector3.Dot(b - planePoint, planeNormal);

                bool aInside = da <= 0;
                bool bInside = db <= 0;

                if (aInside && bInside)
                {
                    output.Add(b);
                }
                else if (aInside && !bInside)
                {
                    float t = da / (da - db);
                    output.Add(a + t * (b - a));
                }
                else if (!aInside && bInside)
                {
                    float t = da / (da - db);
                    output.Add(a + t * (b - a));
                    output.Add(b);
                }
            }
        }

        private readonly record struct Face(Vector3 Centre, Vector3 Tangent1, Vector3 Tangent2, float HalfExtent1, float HalfExtent2)
        {
            public static Face GetFace(
                Vector3 boxCenter,
                Span<Vector3> axes,
                Vector3 halfDimensions,
                Vector3 normal)
            {
                Vector3 axisX = axes[0];
                Vector3 axisY = axes[1];
                Vector3 axisZ = axes[2];

                float dx = MathF.Abs(Vector3.Dot(normal, axisX));
                float dy = MathF.Abs(Vector3.Dot(normal, axisY));
                float dz = MathF.Abs(Vector3.Dot(normal, axisZ));

                if (dx > dy && dx > dz)
                {
                    var faceNormal = Vector3.Dot(normal, axisX) > 0 ? axisX : -axisX;

                    return new Face(
                        boxCenter + faceNormal * halfDimensions.X,
                        axisY,
                        axisZ,
                        halfDimensions.Y,
                        halfDimensions.Z);
                }

                if (dy > dz)
                {
                    var faceNormal = Vector3.Dot(normal, axisY) > 0 ? axisY : -axisY;

                    return new Face(
                        boxCenter + faceNormal * halfDimensions.Y,
                        axisX,
                        axisZ,
                        halfDimensions.X,
                        halfDimensions.Z);
                }

                var faceNormalZ = Vector3.Dot(normal, axisZ) > 0 ? axisZ : -axisZ;

                return new Face(
                    boxCenter + faceNormalZ * halfDimensions.Z,
                    axisX,
                    axisY,
                    halfDimensions.X,
                    halfDimensions.Y);
            }

            public List<Vector3> GetVertices()
            {
                var t1 = Tangent1 * HalfExtent1;
                var t2 = Tangent2 * HalfExtent2;

                return [
                    Centre - t1 - t2,
                    Centre + t1 - t2,
                    Centre + t1 + t2,
                    Centre - t1 + t2
                ];
            }
        }

        private static bool TestAxes(Span<Vector3> polyA, Span<Vector3> polyB, Span<Vector3> axes, ref Vector3 bestNormal, ref float bestPenetration)
        {
            foreach (var axis in axes)
            {
                Project(polyA, axis, out float minA, out float maxA);
                Project(polyB, axis, out float minB, out float maxB);

                if (maxA < minB
                    || maxB < minA)
                {
                    return false;
                }

                float overlap = MathF.Min(maxA, maxB) - MathF.Max(minA, minB);
                if (overlap < bestPenetration)
                {
                    bestPenetration = overlap;
                    bestNormal = axis;
                }
            }
            return true;
        }

        private static bool TestAxes(Vector3 centreA, Vector3 halfExtentA, Span<Vector3> axesA, Vector3 centreB, Vector3 halfExtentB, Span<Vector3> axesB, Span<Vector3> testAxes, ref Vector3 bestNormal, ref float bestPenetration)
        {
            foreach (var axis in testAxes)
            {
                Project(centreA, halfExtentA, axesA, axis, out float minA, out float maxA);
                Project(centreB, halfExtentB, axesB, axis, out float minB, out float maxB);

                if (maxA < minB
                    || maxB < minA)
                {
                    return false;
                }

                float overlap = MathF.Min(maxA, maxB) - MathF.Max(minA, minB);
                if (overlap < bestPenetration)
                {
                    bestPenetration = overlap;
                    bestNormal = axis;
                }
            }
            return true;
        }

        public static void Project(
            Vector3 position,
            Vector3 halfExtent,
            Span<Vector3> boxWorldAxes,
            Vector3 axis,
            out float min,
            out float max)
        {
            // OBB local axes in world space
            Vector3 u0 = boxWorldAxes[0];
            Vector3 u1 = boxWorldAxes[1];
            Vector3 u2 = boxWorldAxes[2];

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

        private static void Project(
            Span<Vector3> poly,
            Vector3 axis,
            out float min,
            out float max)
        {
            min = Vector3.Dot(poly[0], axis);
            max = min;

            for (int i = 1; i < poly.Length; i++)
            {
                float p = Vector3.Dot(poly[i], axis);
                if (p < min)
                {
                    min = p;
                }
                if (p > max)
                {
                    max = p;
                }
            }
        }
    }
}
