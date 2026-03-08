using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using System.Numerics;

namespace Frinkahedron.Core.Colliders
{
    public sealed class Box(Vector3 dimensions) : IShape
    {
        public Vector3 Dimensions { get; } = dimensions;

        public Matrix3x3 CalculateFilledInertia(float mass)
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

        /*public CollisionManifold CheckForCollisions(Position position, ICollider other, Position otherPosition)
        {
            if (other is SphereCollider sphereCollider)
            {
                if (position.Orientation.IsIdentity)
                {
                    return Collisions.AABBSphereCollision(this, position.Centre, sphereCollider, otherPosition);
                }
            }
            if (other is BoxCollider boxCollider)
            {
                if (otherPosition.Orientation.IsIdentity)
                {
                    return Collisions.BoxAABBCollision(this, position, boxCollider, otherPosition.Centre);
                }
                else if (position.Orientation.IsIdentity)
                {
                    return Collisions.BoxAABBCollision(boxCollider, otherPosition, this, position.Centre).Invert();
                }
                else
                {
                    //return Collisions.BoxBoxCollision(this, position, boxCollider, otherPosition);
                }
            }
            return CollisionManifold.NoCollision();
        }*/

        public void Draw(IRenderer renderer, Matrix4x4 position)
        {
            Matrix4x4 scale = Matrix4x4.CreateScale(Dimensions);
            renderer.DrawCuboid(scale * position);
        }

        
    }
}
