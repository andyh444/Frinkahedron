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
            var manifold = CollisionPairTester.Test(
                testCase.CollisionObjectA.Position,
                testCase.CollisionObjectA.Collider,
                testCase.CollisionObjectB.Position,
                testCase.CollisionObjectB.Collider);

            Assert.That(manifold.Points.Length > 0, Is.EqualTo(testCase.CollisionExpected));
            if (testCase.CollisionExpected)
            {
                Assert.That(manifold.Normal, Is.EqualTo(testCase.ExpectedNormal));
                //Assert.That(manifold.Penetration, Is.EqualTo(testCase.ExpectedPenetration).Within(1e-3));
            }
        }

        private static IEnumerable<TestCase> GetTestCases()
        {
            foreach (var testCase in GetForwardTestCases())
            {
                yield return testCase;
                yield return new TestCase(
                    testCase.CollisionObjectB,
                    testCase.CollisionObjectA,
                    testCase.CollisionExpected,
                    -testCase.ExpectedNormal,
                    testCase.ExpectedPenetration,
                    testCase.ToString() + "-Reversed");
            }
        }

        private static IEnumerable<TestCase> GetForwardTestCases()
        {
            var sphere1 = CreateSphere(new Vector3(0, 0, 0), 1.01f);
            var sphere2 = CreateSphere(new Vector3(2, 0, 0), 1.01f);
            yield return new TestCase(sphere1, sphere2, true, new Vector3(-1, 0, 0), 0.02f, "Colliding spheres");

            var sphere3 = CreateSphere(new Vector3(3, 0, 0), 1.01f);
            yield return new TestCase(sphere1, sphere3, false, default, default, "Non-Colliding spheres");

            var aab = CreateAxisAlignedBox(new Vector3(0, -2, 0), new Vector3(20, 2, 20));
            yield return new TestCase(sphere1, aab, true, new Vector3(0, 1, 0), 0.01f, "Colliding sphere-aab");

            var box1 = CreateAxisAlignedBox(new Vector3(), new Vector3(1, 1, 2));
            var box2 = CreateAxisAlignedBox(new Vector3(1f, 0, 0), new Vector3(1, 1, 1));
            box2.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 4);

            yield return new TestCase(box1, box2, true, new Vector3(-1, 0, 0), 0.293f, "Colliding boxes");

            // real world problematic case
            var box3 = CreateAxisAlignedBox(new Vector3(-8.036688f, -13.5663185f, 0), new Vector3(2.4534235f, 1.1443835f, 2.5965152f));
            box3.Position.Orientation = new Quaternion(0.10432941f, -0.6247331f, 0.7655363f, 0.113040484f);

            var box4 = CreateAxisAlignedBox(new Vector3(0, -20, 0), new Vector3(100, 10, 100));
            yield return new TestCase(box3, box4, true, new Vector3(0, 1, 0), 0.293f, "Colliding boxes 2");
        }

        private static TestObject CreateSphere(Vector3 centre, float radius)
        {
            Position position = new Position(centre, Quaternion.Identity);
            Sphere collider = new Sphere(radius);
            return new TestObject(position, collider);
        }

        private static TestObject CreateAxisAlignedBox(Vector3 centre, Vector3 dimensions)
        {
            Position position = new Position(centre, Quaternion.Identity);
            Box collider = new Box(dimensions);
            return new TestObject(position, collider);
        }

        public record TestObject(Position Position, IShape Collider);

        public class TestCase(TestObject collisionObjectA, TestObject collisionObjectB, bool collisionExpected, Vector3 expectedNormal, float expectedPenetration, string testName)
        {
            public TestObject CollisionObjectA { get; } = collisionObjectA;

            public TestObject CollisionObjectB { get; } = collisionObjectB;

            public bool CollisionExpected { get; } = collisionExpected;

            public Vector3 ExpectedNormal { get; } = expectedNormal;

            public float ExpectedPenetration { get; } = expectedPenetration;

            public override string ToString() => testName;
        }
    }
}
