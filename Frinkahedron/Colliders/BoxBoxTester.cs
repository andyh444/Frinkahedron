using Frinkahedron.Core.Physics;
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

            // Pick the reference face from whichever box has a face most aligned
            // with the collision normal. This avoids degenerate clipping when the
            // normal is nearly parallel to all faces of the first box.
            float bestAlignA = MaxAbsDot(axesA, bestNormal);
            float bestAlignB = MaxAbsDot(axesB, bestNormal);

            Vector3[] contactPoints;
            if (bestAlignA >= bestAlignB)
            {
                contactPoints = CalculateContactPoints(boxA, positionA, axesA, boxB, positionB, axesB, bestNormal, bestPenetration);
            }
            else
            {
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
            const float DEPTH_THRESHOLD = 1e-3f;

            BoxFace faceA = BoxFace.GetFace(positionA.Centre, axesA, boxA.Dimensions / 2, -normal);
            BoxFace faceB = BoxFace.GetFace(positionB.Centre, axesB, boxB.Dimensions / 2, normal);

            var intersection = ClipFaces(faceA, faceB);

            Span<float> depths = stackalloc float[intersection.Count];
            int contactCount = 0;

            for (int i = 0; i < intersection.Count; i++)
            {
                Vector3 p = intersection[i];
                float depth = Vector3.Dot(faceA.Centre - p, normal);
                depths[i] = depth;

                if (depth <= DEPTH_THRESHOLD)
                {
                    contactCount++;
                }
            }
            if (contactCount <= 4)
            {
                Vector3[] contacts = new Vector3[contactCount];
                int index = 0;
                for (int i = 0; i < intersection.Count; i++)
                {
                    if (depths[i] <= DEPTH_THRESHOLD)
                    {
                        contacts[index++] = intersection[i];
                    }
                }
                return contacts;
            }
            return PickFourPoints(intersection, depths);
        }

        private static Vector3[] PickFourPoints(List<Vector3> candidates, Span<float> depths)
        {
            const float DEPTH_THRESHOLD = 1e-3f;

            int deepest = 0;
            float maxDepth = depths[0];

            for (int i = 1; i < candidates.Count; i++)
            {
                if (depths[i] > maxDepth)
                {
                    deepest = i;
                    maxDepth = depths[i];
                }
            }

            int farthest = deepest;
            float maxDist = 0f;

            for (int i = 0; i < candidates.Count; i++)
            {
                if (depths[i] < DEPTH_THRESHOLD)
                {
                    continue;
                }

                float d = Vector3.DistanceSquared(
                    candidates[i],
                    candidates[deepest]);

                if (d > maxDist)
                {
                    maxDist = d;
                    farthest = i;
                }
            }

            int third = deepest;
            float maxLineDist = 0f;

            Vector3 a = candidates[deepest];
            Vector3 b = candidates[farthest];

            for (int i = 0; i < candidates.Count; i++)
            {
                if (depths[i] < DEPTH_THRESHOLD)
                {
                    continue;
                }
                Vector3 p = candidates[i];

                float dist = Vector3.Cross(p - a, b - a).LengthSquared();

                if (dist > maxLineDist)
                {
                    maxLineDist = dist;
                    third = i;
                }
            }

            int fourth = deepest;
            float maxPlaneDist = 0f;

            Vector3 c = candidates[third];

            Vector3 normal = Vector3.Cross(b - a, c - a);

            for (int i = 0; i < candidates.Count; i++)
            {
                if (depths[i] < DEPTH_THRESHOLD)
                {
                    continue;
                }
                Vector3 p = candidates[i];

                float dist = MathF.Abs(Vector3.Dot(p - a, normal));

                if (dist > maxPlaneDist)
                {
                    maxPlaneDist = dist;
                    fourth = i;
                }
            }

            return
            [
                candidates[deepest],
                candidates[farthest],
                candidates[third],
                candidates[fourth]
            ];
        }

        private static List<Vector3> ClipFaces(BoxFace reference, BoxFace incident)
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

        
        private static bool TestAxes(Vector3 centreA, Vector3 halfExtentA, Span<Vector3> axesA, Vector3 centreB, Vector3 halfExtentB, Span<Vector3> axesB, Span<Vector3> testAxes, ref Vector3 bestNormal, ref float bestPenetration)
        {
            foreach (var axis in testAxes)
            {
                Box.Project(centreA, halfExtentA, axesA, axis, out float minA, out float maxA);
                Box.Project(centreB, halfExtentB, axesB, axis, out float minB, out float maxB);

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

        private static float MaxAbsDot(Span<Vector3> axes, Vector3 direction)
        {
            float best = 0f;
            foreach (var axis in axes)
            {
                float d = MathF.Abs(Vector3.Dot(axis, direction));
                if (d > best)
                {
                    best = d;
                }
            }
            return best;
        }
    }
}
