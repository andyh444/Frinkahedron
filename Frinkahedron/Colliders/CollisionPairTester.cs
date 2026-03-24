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
                case (Sphere sphA, Capsule capB):
                    return SphereCapsuleTester.Test(Collidable(positionA, sphA), Collidable(positionB, capB));
                case (Sphere sphA, Cylinder cylB):
                    return CollisionManifold.NoCollision(); // TODO

                case (Box boxA, Sphere sphB):
                    return BoxSphereTester.Test(Collidable(positionA, boxA), Collidable(positionB, sphB));
                case (Box boxA, Box boxB):
                    return BoxBoxTester.Test(Collidable(positionA, boxA), Collidable(positionB, boxB));
                case (Box boxA, Capsule capB):
                    return BoxCapsuleTester.Test(Collidable(positionA, boxA), Collidable(positionB, capB));
                case (Box boxA, Cylinder cylB):
                    return BoxCylinderTester.Test(Collidable(positionA, boxA), Collidable(positionB, cylB));

                case (Capsule capA, Sphere sphB):
                    return CapsuleSphereTester.Test(Collidable(positionA, capA), Collidable(positionB, sphB));
                case (Capsule capA, Box boxB):
                    return CapsuleBoxTester.Test(Collidable(positionA, capA), Collidable(positionB, boxB));
                case (Capsule capA, Capsule capB):
                    return CapsuleCapsuleTester.Test(Collidable(positionA, capA), Collidable(positionB, capB));
                case (Capsule capA, Cylinder cylB):
                    return CollisionManifold.NoCollision(); // TODO

                case (Cylinder cylA, Sphere sphB):
                    return CollisionManifold.NoCollision(); // TODO
                case (Cylinder cylA, Box boxB):
                    return CylinderBoxTester.Test(Collidable(positionA, cylA), Collidable(positionB, boxB));
                case (Cylinder cylA, Capsule capB):
                    return CollisionManifold.NoCollision(); // TODO
                case (Cylinder cylA, Cylinder cylB):
                    return CollisionManifold.NoCollision(); // TODO

            }

            return CollisionManifold.NoCollision();
        }

        private static Collidable<T> Collidable<T>(Position position, T shape) where T : IShape
        {
            return new Collidable<T>(position, shape);
        }
    }
}
