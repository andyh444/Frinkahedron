using Frinkahedron.Core.Physics;

namespace Frinkahedron.Core.Colliders
{
    public static class CollisionPairTester
    {
        public static CollisionManifold Test(Position positionA, IShape shapeA, Position positionB, IShape shapeB)
        {
            switch ((shapeA, shapeB))
            {
                case (Sphere sphA, Sphere sphB):
                    return SphereSphereTester.Test(Collidable(positionA, sphA), Collidable(positionB, sphB));

                case (Sphere sphA, Box boxB):
                    return SphereBoxTester.Test(Collidable(positionA, sphA), Collidable(positionB, boxB));

                case (Box boxA, Sphere sphB):
                    return BoxSphereTester.Test(Collidable(positionA, boxA), Collidable(positionB, sphB));

                case (Box boxA, Box boxB):
                    return BoxBoxTester.Test(Collidable(positionA, boxA), Collidable(positionB, boxB));
            }

            return CollisionManifold.NoCollision();
        }

        private static Collidable<T> Collidable<T>(Position position, T shape) where T : IShape
        {
            return new Collidable<T>(position, shape);
        }
    }
}
