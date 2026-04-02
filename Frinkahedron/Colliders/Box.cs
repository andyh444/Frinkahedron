using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using System.Numerics;

namespace Frinkahedron.Core.Colliders
{
    public sealed class Box(Vector3 dimensions) : IShape
    {
        public Vector3 Dimensions { get; } = dimensions;

        public DiagonalMatrix3x3 CalculateFilledInertia(float mass)
        {
            return Inertia.CalculateFilledCubeInertia(Dimensions, mass);
        }

        public float CalculateVolume()
        {
            return Dimensions.X * Dimensions.Y * Dimensions.Z;
        }

        public IReadOnlyList<Vector3> GetCorners()
        {
            Span<Vector3> corners = stackalloc Vector3[8];
            GetCorners(corners);
            List<Vector3> cornersList = [.. corners];
            return cornersList;
        }

        public void GetCorners(Span<Vector3> cornersSpan)
        {
            if (cornersSpan.Length < 8)
            {
                throw new ArgumentException("Expected a span with at least length 8");
            }
            Vector3 hd = Dimensions / 2;

            cornersSpan[0] = new Vector3(hd.X, hd.Y, hd.Z);
            cornersSpan[1] = new Vector3(-hd.X, hd.Y, hd.Z);
            cornersSpan[2] = new Vector3(hd.X, hd.Y, -hd.Z);
            cornersSpan[3] = new Vector3(-hd.X, hd.Y, -hd.Z);

            cornersSpan[4] = new Vector3(hd.X, -hd.Y, hd.Z);
            cornersSpan[5] = new Vector3(-hd.X, -hd.Y, hd.Z);
            cornersSpan[6] = new Vector3(hd.X, -hd.Y, -hd.Z);
            cornersSpan[7] = new Vector3(-hd.X, -hd.Y, -hd.Z);
        }

        public void Draw(IRenderContext renderer, Matrix4x4 position)
        {
            Matrix4x4 scale = Matrix4x4.CreateScale(Dimensions);
            renderer.DrawPrimitiveWireframe(Primitive.Box, scale * position);
        }

        public AxisAlignedBoundingBox CalculateAABB(Position position)
        {
            Span<Vector3> axes = stackalloc Vector3[] {
                Vector3.Transform(Vector3.UnitX, position.Orientation),
                Vector3.Transform(Vector3.UnitY, position.Orientation),
                Vector3.Transform(Vector3.UnitZ, position.Orientation),
            };

            Vector3 halfExtent = Dimensions / 2;
            Project(position.Centre, halfExtent, axes, Vector3.UnitX, out float minX, out float maxX);
            Project(position.Centre, halfExtent, axes, Vector3.UnitY, out float minY, out float maxY);
            Project(position.Centre, halfExtent, axes, Vector3.UnitZ, out float minZ, out float maxZ);

            return new AxisAlignedBoundingBox(
                new Vector3(minX, minY, minZ),
                new Vector3(maxX, maxY, maxZ));
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

        public IReadOnlyList<BoxFace> GetFaces(Position position)
        {
            Span<Vector3> axes = stackalloc Vector3[] {
                Vector3.Transform(Vector3.UnitX, position.Orientation),
                Vector3.Transform(Vector3.UnitY, position.Orientation),
                Vector3.Transform(Vector3.UnitZ, position.Orientation),
            };
            Vector3 centre = position.ToWorld(new Vector3());
            Vector3 halfDimensions = Dimensions / 2;
            return [
                    BoxFace.GetFace(centre, axes, halfDimensions, axes[0]),
                    BoxFace.GetFace(centre, axes, halfDimensions, -axes[0]),
                    BoxFace.GetFace(centre, axes, halfDimensions, axes[1]),
                    BoxFace.GetFace(centre, axes, halfDimensions, -axes[1]),
                    BoxFace.GetFace(centre, axes, halfDimensions, axes[2]),
                    BoxFace.GetFace(centre, axes, halfDimensions, -axes[2]),
                ];
        }

        public bool RayIntersection(Position position, Vector3 rayPosition, Vector3 rayDirection, out Vector3 result)
        {
            result = Vector3.Zero;

            Quaternion invRot = Quaternion.Inverse(position.Orientation);

            // Transform ray into local space
            Vector3 localOrigin = Vector3.Transform(rayPosition - position.Centre, invRot);
            Vector3 localDir = Vector3.Transform(rayDirection, invRot);

            // AABB bounds
            Vector3 min = -Dimensions / 2;
            Vector3 max = Dimensions / 2;

            if (!RayAABB(localOrigin, localDir, min, max, out float t))
            {
                return false;
            }

            // Intersection point in local space
            Vector3 localHit = localOrigin + localDir * t;

            // Transform back to world space
            result = Vector3.Transform(localHit, position.Orientation) + position.Centre;

            return true;
        }

        private bool RayAABB(
            Vector3 origin,
            Vector3 dir,
            Vector3 min,
            Vector3 max,
            out float t)
        {
            float tMin = 0.0f;
            float tMax = float.MaxValue;

            for (int i = 0; i < 3; i++)
            {
                if (Math.Abs(dir[i]) < 1e-6f)
                {
                    // Ray parallel to slab
                    if (origin[i] < min[i] || origin[i] > max[i])
                    {
                        t = 0;
                        return false;
                    }
                }
                else
                {
                    float ood = 1.0f / dir[i];
                    float t1 = (min[i] - origin[i]) * ood;
                    float t2 = (max[i] - origin[i]) * ood;

                    if (t1 > t2)
                    {
                        float tmp = t1;
                        t1 = t2;
                        t2 = tmp;
                    }

                    tMin = Math.Max(tMin, t1);
                    tMax = Math.Min(tMax, t2);

                    if (tMin > tMax)
                    {
                        t = 0;
                        return false;
                    }
                }
            }

            t = tMin;
            return true;
        }
    }

}
