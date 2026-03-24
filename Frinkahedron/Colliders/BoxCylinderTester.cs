namespace Frinkahedron.Core.Colliders
{
    public struct BoxCylinderTester : ICollisionPairTester<Box, Cylinder>
    {
        public static CollisionManifold Test(Collidable<Box> shapeA, Collidable<Cylinder> shapeB)
        {
            return CylinderBoxTester.Test(shapeB, shapeA).Invert();
        }
    }
}
