namespace Frinkahedron.Core.Colliders
{
    public struct BoxSphereTester : ICollisionPairTester<Box, Sphere>
    {
        public static CollisionManifold Test(Collidable<Box> shapeA, Collidable<Sphere> shapeB)
        {
            return SphereBoxTester.Test(shapeB, shapeA).Invert();
        }
    }
}
