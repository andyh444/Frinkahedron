namespace Frinkahedron.Core.Colliders
{
    public interface ICollisionPairTester<TShapeA, TShapeB> where TShapeA : IShape where TShapeB : IShape
    {
        /// <summary>
        /// Tests the two shapes for collision
        /// </summary>
        /// <returns>a manifold containing all contact points (if any), and a normal pointing from B to A</returns>
        public static abstract CollisionManifold Test(Collidable<TShapeA> shapeA, Collidable<TShapeB> shapeB);
    }
}
