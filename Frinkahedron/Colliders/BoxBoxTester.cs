using Frinkahedron.Core.Physics;
using System.Numerics;

namespace Frinkahedron.Core.Colliders
{
    public struct BoxBoxTester : ICollisionPairTester<Box, Box>
    {
        public static CollisionManifold Test(Collidable<Box> shapeA, Collidable<Box> shapeB)
        {
            if (shapeB.Position.Orientation.IsIdentity)
            {
                return BoxAABBCollision(shapeA.Shape, shapeA.Position, shapeB.Shape, shapeB.Position.Centre);
            }

            else if (shapeA.Position.Orientation.IsIdentity)
            {
                return BoxAABBCollision(shapeB.Shape, shapeB.Position, shapeA.Shape, shapeA.Position.Centre).Invert();
            }
            return CollisionManifold.NoCollision();
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
    }
}
