using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Physics;
using System.Numerics;

namespace Frinkahedron.Core.Tests
{
    public class Collider_Test
    {
        [TestCaseSource(nameof(GetTestCases))]
        public void CheckForCollisions_Test(TestCase testCase)
        {
            var manifold = testCase.CollisionObjectA.Collider.CheckForCollisions(
                testCase.CollisionObjectA.Position,
                testCase.CollisionObjectB.Collider,
                testCase.CollisionObjectB.Position);

            Assert.That(manifold.Points.Length > 0, Is.EqualTo(testCase.CollisionExpected));
            if (testCase.CollisionExpected)
            {
                Assert.That(manifold.Normal, Is.EqualTo(testCase.ExpectedNormal));
            }

            // now check the same but with the objects swapped. The result should be the same but with the normal pointing the other way
            var reverseManifold = testCase.CollisionObjectB.Collider.CheckForCollisions(
                testCase.CollisionObjectB.Position,
                testCase.CollisionObjectA.Collider,
                testCase.CollisionObjectA.Position);

            Assert.That(reverseManifold.Points.Length > 0, Is.EqualTo(testCase.CollisionExpected));
            if (testCase.CollisionExpected)
            {
                Assert.That(reverseManifold.Normal, Is.EqualTo(-testCase.ExpectedNormal));
            }
        }

        private static IEnumerable<TestCase> GetTestCases()
        {
            var sphere1 = CreateSphere(new Vector3(0, 0, 0), 1.01f);
            var sphere2 = CreateSphere(new Vector3(2, 0, 0), 1.01f);
            yield return new TestCase(sphere1, sphere2, true, new Vector3(-1, 0, 0), "Colliding spheres");

            var sphere3 = CreateSphere(new Vector3(3, 0, 0), 1.01f);
            yield return new TestCase(sphere1, sphere3, false, default, "Non-Colliding spheres");
        }

        private static TestObject CreateSphere(Vector3 centre, float radius)
        {
            Position position = new Position(centre, Quaternion.Identity);
            SphereCollider collider = new SphereCollider(radius);
            return new TestObject(position, collider);
        }

        private static TestObject CreateAxisAlignedBox(Vector3 centre, Vector3 dimensions)
        {
            Position position = new Position(centre, Quaternion.Identity);
            BoxCollider collider = new BoxCollider(dimensions);
            return new TestObject(position, collider);
        }

        public record TestObject(Position Position, ICollider Collider);

        public class TestCase(TestObject collisionObjectA, TestObject collisionObjectB, bool collisionExpected, Vector3 expectedNormal, string testName)
        {
            public TestObject CollisionObjectA { get; } = collisionObjectA;

            public TestObject CollisionObjectB { get; } = collisionObjectB;

            public bool CollisionExpected { get; } = collisionExpected;

            public Vector3 ExpectedNormal { get; } = expectedNormal;

            public override string ToString() => testName;
        }
    }
}
