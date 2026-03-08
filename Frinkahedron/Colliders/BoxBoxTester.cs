using Frinkahedron.Core.Physics;
using System.Numerics;

namespace Frinkahedron.Core.Colliders
{
    public struct BoxBoxTester : ICollisionPairTester<Box, Box>
    {
        public static CollisionManifold Test(Collidable<Box> shapeA, Collidable<Box> shapeB)
        {
            /*if (shapeB.Position.Orientation.IsIdentity)
            {
                return BoxAABBCollision(shapeA.Shape, shapeA.Position, shapeB.Shape, shapeB.Position.Centre);
            }

            else if (shapeA.Position.Orientation.IsIdentity)
            {
                return BoxAABBCollision(shapeB.Shape, shapeB.Position, shapeA.Shape, shapeA.Position.Centre).Invert();
            }
            return CollisionManifold.NoCollision();*/
            return BoxBoxCollision(shapeA.Shape, shapeA.Position, shapeB.Shape, shapeB.Position);
        }

        public static CollisionManifold BoxAABBCollision(Box boxCollider, Position boxPosition, Box aabbCollider, Vector3 aabbPosition)
        {
            float aabbTop = aabbCollider.Dimensions.Y / 2 + aabbPosition.Y;
            Matrix4x4 transform = boxPosition.ToMatrix();

            List<Vector3> points = new List<Vector3>();
            float penetration = 0;
            foreach (var corner in boxCollider.GetCorners())
            {
                Vector3 tc = Vector3.Transform(corner, transform);
                if (tc.Y < aabbTop)
                {
                    points.Add(tc);
                    penetration = Math.Max(penetration, aabbTop - tc.Y);
                }
            }
            return new CollisionManifold(points.ToArray(), Vector3.UnitY, penetration);
        }

        public static CollisionManifold BoxBoxCollision(Box boxA, Position positionA, Box boxB, Position positionB)
        {
            var transformA = positionA.ToMatrix();
            var transformB = positionB.ToMatrix();

            _ = Matrix4x4.Invert(transformA, out var inverseA);

            var transformBToA = transformB * inverseA;

            Span<Vector3> axesA = [
                Vector3.UnitX,
                Vector3.UnitY,
                Vector3.UnitZ,
                ];

            Span<Vector3> axesB = [
                Vector3.TransformNormal(Vector3.UnitX, transformBToA),
                Vector3.TransformNormal(Vector3.UnitY, transformBToA),
                Vector3.TransformNormal(Vector3.UnitZ, transformBToA),
                ];

            Span<Vector3> crossAxes = stackalloc Vector3[9];
            int index = 0;
            foreach (var axisA in axesA)
            {
                foreach (var axisB in axesB)
                {
                    var cross = Vector3.Cross(axisA, axisB);
                    var lengthSq = cross.LengthSquared();
                    if (lengthSq > 1e-6f)
                    {
                        crossAxes[index++] = cross / MathF.Sqrt(lengthSq);
                    }
                }
            }

            Span<Vector3> cornersA = stackalloc Vector3[8];
            boxA.GetCorners(cornersA);
            
            Span<Vector3> cornersB = stackalloc Vector3[8];
            boxB.GetCorners(cornersB);
            for (int i = 0; i < cornersB.Length; i++)
            {
                cornersB[i] = Vector3.Transform(cornersB[i], transformBToA);
            }

            Vector3 bestNormal = default;
            float bestPenetration = float.MaxValue;

            if (!TestAxes(cornersA, cornersB, axesA, ref bestNormal, ref bestPenetration))
            {
                return CollisionManifold.NoCollision();
            }

            if (!TestAxes(cornersA, cornersB, axesB, ref bestNormal, ref bestPenetration))
            {
                return CollisionManifold.NoCollision();
            }

            if (!TestAxes(cornersA, cornersB, crossAxes[..index], ref bestNormal, ref bestPenetration))
            {
                return CollisionManifold.NoCollision();
            }

            // rotate normal back to world space
            bestNormal = Vector3.Transform(bestNormal, positionA.Orientation);

            // normal needs to point from B to A
            Vector3 centerA = positionA.Centre;
            Vector3 centerB = positionB.Centre;
            if (Vector3.Dot(centerA - centerB, bestNormal) < 0f)
            {
                bestNormal = -bestNormal;
            }

            /*Vector3[] contactPoints = CalculateContactPoints(boxA.Dimensions, cornersB)
                .Select(x => Vector3.Transform(x, transformA))
                .ToArray();*/

            axesA[0] = Vector3.TransformNormal(Vector3.UnitX, transformA);
            axesA[1] = Vector3.TransformNormal(Vector3.UnitY, transformA);
            axesA[2] = Vector3.TransformNormal(Vector3.UnitZ, transformA);

            axesB[0] = Vector3.TransformNormal(Vector3.UnitX, transformB);
            axesB[1] = Vector3.TransformNormal(Vector3.UnitY, transformB);
            axesB[2] = Vector3.TransformNormal(Vector3.UnitZ, transformB);

            var contactPoints = CalculateContactPoints2(boxA, positionA, axesA, boxB, positionB, axesB, bestNormal, bestPenetration);

            var contactPoint = new Vector3();
            foreach (var p in contactPoints)
            {
                contactPoint += p;
            }
            contactPoint /= contactPoints.Count;

            return new CollisionManifold([contactPoint], bestNormal, 30 * bestPenetration);
        }

        private static List<Vector3> CalculateContactPoints(Vector3 boxADimensions, Span<Vector3> cornersB)
        {
            // simple case - one of B's corners is inside A
            Vector3 halfA = boxADimensions / 2;
            List<Vector3> contactPoints = new List<Vector3>();
            foreach (var corner in cornersB)
            {
                if (MathF.Abs(corner.X) < halfA.X
                    && MathF.Abs(corner.Y) < halfA.Y
                    && MathF.Abs(corner.Z) < halfA.Z)
                {
                    contactPoints.Add(corner);
                    //break;
                }
            }
            return contactPoints;

            // need to consider other cases e.g. edge-face intersection, A corners inside B
        }

        private static List<Vector3> CalculateContactPoints2(
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
            if (contacts.Count <= 4)
            {
                return contacts.Select(x => x.Item1).ToList();
            }
            return PickFourPoints(contacts);
        }

        private static List<Vector3> PickFourPoints(List<(Vector3 Position, float Depth)> candidates)
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

            return new List<Vector3>
            {
                candidates[deepest].Position,
                candidates[farthest].Position,
                candidates[third].Position,
                candidates[fourth].Position
            };
        }

        private static List<Vector3> ClipFaces(Face reference, Face incident)
        {
            var polygon = new List<Vector3>(incident.GetVertices());

            var t1 = reference.Tangent1;
            var t2 = reference.Tangent2;

            var e1 = reference.HalfExtent1;
            var e2 = reference.HalfExtent2;

            // 4 edge planes of the reference face
            var planes = new (Vector3 point, Vector3 normal)[]
            {
                (reference.Centre + t1 * e1,  t1),
                (reference.Centre - t1 * e1, -t1),
                (reference.Centre + t2 * e2,  t2),
                (reference.Centre - t2 * e2, -t2),
            };

            foreach (var (point, normal) in planes)
            {
                polygon = ClipPolygon(polygon, point, normal);

                if (polygon.Count == 0)
                    break;
            }

            return polygon;
        }

        static List<Vector3> ClipPolygon(List<Vector3> polygon, Vector3 planePoint, Vector3 planeNormal)
        {
            var result = new List<Vector3>();

            for (int i = 0; i < polygon.Count; i++)
            {
                var a = polygon[i];
                var b = polygon[(i + 1) % polygon.Count];

                float da = Vector3.Dot(a - planePoint, planeNormal);
                float db = Vector3.Dot(b - planePoint, planeNormal);

                bool aInside = da <= 0;
                bool bInside = db <= 0;

                if (aInside && bInside)
                {
                    result.Add(b);
                }
                else if (aInside && !bInside)
                {
                    float t = da / (da - db);
                    result.Add(a + t * (b - a));
                }
                else if (!aInside && bInside)
                {
                    float t = da / (da - db);
                    result.Add(a + t * (b - a));
                    result.Add(b);
                }
            }

            return result;
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

            public Vector3[] GetVertices()
            {
                var t1 = Tangent1 * HalfExtent1;
                var t2 = Tangent2 * HalfExtent2;

                return new[]
                {
                    Centre - t1 - t2,
                    Centre + t1 - t2,
                    Centre + t1 + t2,
                    Centre - t1 + t2
                };
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
