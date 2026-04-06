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
                if (!testCase.SkipNormalCheck)
                {
                    Assert.That(manifold.Normal.X, Is.EqualTo(testCase.ExpectedNormal.X).Within(1e-4), "Normal.X");
                    Assert.That(manifold.Normal.Y, Is.EqualTo(testCase.ExpectedNormal.Y).Within(1e-4), "Normal.Y");
                    Assert.That(manifold.Normal.Z, Is.EqualTo(testCase.ExpectedNormal.Z).Within(1e-4), "Normal.Z");
                }
                if (!testCase.SkipPenetrationCheck)
                {
                    Assert.That(manifold.Penetration, Is.EqualTo(testCase.ExpectedPenetration).Within(1e-2));
                }
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
                    testCase.ToString() + "-Reversed")
                {
                    SkipNormalCheck = testCase.SkipNormalCheck,
                    SkipPenetrationCheck = testCase.SkipPenetrationCheck
                };
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

            foreach (var tc in GetBoxBoxTestCases())
                yield return tc;
        }

        private static IEnumerable<TestCase> GetBoxBoxTestCases()
        {
            // --- Existing cases ---

            var box1 = CreateAxisAlignedBox(new Vector3(), new Vector3(1, 1, 2));
            var box2 = CreateAxisAlignedBox(new Vector3(1f, 0, 0), new Vector3(1, 1, 1));
            box2.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 4);
            yield return new TestCase(box1, box2, true, new Vector3(-1, 0, 0), 0.2071f, "Box-Box: rotated overlap");

            // real world problematic case
            var box3 = CreateAxisAlignedBox(new Vector3(-8.036688f, -13.5663185f, 0), new Vector3(2.4534235f, 1.1443835f, 2.5965152f));
            box3.Position.Orientation = new Quaternion(0.10432941f, -0.6247331f, 0.7655363f, 0.113040484f);
            var box4 = CreateAxisAlignedBox(new Vector3(0, -20, 0), new Vector3(100, 10, 100));
            yield return new TestCase(box3, box4, true, new Vector3(0, 1, 0), 0.002f, "Box-Box: real world case");

            // --- Axis-aligned face-face overlap on each axis ---

            // Overlap along X axis (face-face contact on X)
            var aafX_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var aafX_B = CreateAxisAlignedBox(new Vector3(1.5f, 0, 0), new Vector3(2, 2, 2));
            yield return new TestCase(aafX_A, aafX_B, true, new Vector3(-1, 0, 0), 0.5f, "Box-Box: AA face overlap X");

            // Overlap along Y axis (face-face contact on Y)
            var aafY_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var aafY_B = CreateAxisAlignedBox(new Vector3(0, 1.5f, 0), new Vector3(2, 2, 2));
            yield return new TestCase(aafY_A, aafY_B, true, new Vector3(0, -1, 0), 0.5f, "Box-Box: AA face overlap Y");

            // Overlap along Z axis (face-face contact on Z)
            var aafZ_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var aafZ_B = CreateAxisAlignedBox(new Vector3(0, 0, 1.5f), new Vector3(2, 2, 2));
            yield return new TestCase(aafZ_A, aafZ_B, true, new Vector3(0, 0, -1), 0.5f, "Box-Box: AA face overlap Z");

            // --- Axis-aligned separation on each axis ---

            // Separated along X
            var sepX_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var sepX_B = CreateAxisAlignedBox(new Vector3(3, 0, 0), new Vector3(2, 2, 2));
            yield return new TestCase(sepX_A, sepX_B, false, default, default, "Box-Box: AA separated X");

            // Separated along Y
            var sepY_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var sepY_B = CreateAxisAlignedBox(new Vector3(0, 3, 0), new Vector3(2, 2, 2));
            yield return new TestCase(sepY_A, sepY_B, false, default, default, "Box-Box: AA separated Y");

            // Separated along Z
            var sepZ_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var sepZ_B = CreateAxisAlignedBox(new Vector3(0, 0, 3), new Vector3(2, 2, 2));
            yield return new TestCase(sepZ_A, sepZ_B, false, default, default, "Box-Box: AA separated Z");

            // --- Identical boxes (fully overlapping) ---

            var ident_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var ident_B = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            yield return new TestCase(ident_A, ident_B, true, default, 2.0f, "Box-Box: identical boxes") { SkipNormalCheck = true };

            // --- Small box fully inside large box ---

            var inner = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            var outer = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(10, 10, 10));
            yield return new TestCase(inner, outer, true, default, 1.0f, "Box-Box: small inside large") { SkipNormalCheck = true };

            // --- Asymmetric dimensions: different sizes on each axis ---

            var asym_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(4, 1, 1));
            var asym_B = CreateAxisAlignedBox(new Vector3(1.5f, 0, 0), new Vector3(1, 4, 1));
            yield return new TestCase(asym_A, asym_B, true, new Vector3(0, 0, -1), 1.0f, "Box-Box: asymmetric overlap") { SkipNormalCheck = true };

            // --- Rotated box separated from axis-aligned box ---

            var rotSep_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var rotSep_B = CreateAxisAlignedBox(new Vector3(3, 0, 0), new Vector3(2, 2, 2));
            rotSep_B.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 4);
            yield return new TestCase(rotSep_A, rotSep_B, false, default, default, "Box-Box: rotated separated");

            // --- 45-degree rotation causing overlap that wouldn't exist axis-aligned ---

            var rot45_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var rot45_B = CreateAxisAlignedBox(new Vector3(2.2f, 0, 0), new Vector3(2, 2, 2));
            rot45_B.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 4);
            // Rotated box extends further along X, creating overlap
            yield return new TestCase(rot45_A, rot45_B, true, default, default, "Box-Box: 45deg Y rotation overlap") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // --- Edge-edge contact: both boxes rotated differently ---

            var ee_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            ee_A.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 4);
            var ee_B = CreateAxisAlignedBox(new Vector3(2.0f, 0, 0), new Vector3(2, 2, 2));
            ee_B.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 4);
            yield return new TestCase(ee_A, ee_B, true, default, default, "Box-Box: edge-edge both rotated") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // --- Rotation around X axis ---

            var rotX_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var rotX_B = CreateAxisAlignedBox(new Vector3(0, 1.8f, 0), new Vector3(2, 2, 2));
            rotX_B.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 4);
            yield return new TestCase(rotX_A, rotX_B, true, default, default, "Box-Box: rotation around X overlap") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // --- Rotation around Z axis ---

            var rotZ_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var rotZ_B = CreateAxisAlignedBox(new Vector3(1.8f, 0, 0), new Vector3(2, 2, 2));
            rotZ_B.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 6); // 30 degrees
            yield return new TestCase(rotZ_A, rotZ_B, true, default, default, "Box-Box: 30deg Z rotation overlap") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // --- Barely separated on rotated axis ---

            float sqrt2 = MathF.Sqrt(2);
            var barSep_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var barSep_B = CreateAxisAlignedBox(new Vector3(1 + sqrt2 + 0.1f, 0, 0), new Vector3(2, 2, 2));
            barSep_B.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 4);
            yield return new TestCase(barSep_A, barSep_B, false, default, default, "Box-Box: barely separated rotated");

            // --- Barely overlapping on rotated axis ---

            var barOvl_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var barOvl_B = CreateAxisAlignedBox(new Vector3(1 + sqrt2 - 0.1f, 0, 0), new Vector3(2, 2, 2));
            barOvl_B.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 4);
            yield return new TestCase(barOvl_A, barOvl_B, true, default, default, "Box-Box: barely overlapping rotated") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // --- Large flat box vs small cube (floor scenario) ---

            var floor = CreateAxisAlignedBox(new Vector3(0, -5.5f, 0), new Vector3(100, 1, 100));
            var cube = CreateAxisAlignedBox(new Vector3(0, -4.8f, 0), new Vector3(1, 1, 1));
            yield return new TestCase(cube, floor, true, new Vector3(0, 1, 0), 0.3f, "Box-Box: cube on floor");

            // --- Cube resting exactly on floor surface (touching) ---

            var floorTouch = CreateAxisAlignedBox(new Vector3(0, -1, 0), new Vector3(100, 2, 100));
            var cubeTouch = CreateAxisAlignedBox(new Vector3(0, 0.6f, 0), new Vector3(1, 1, 1));
            yield return new TestCase(cubeTouch, floorTouch, false, default, default, "Box-Box: cube above floor (no overlap)");

            // --- Diagonal separation: separated along combined diagonal direction ---

            var diag_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var diag_B = CreateAxisAlignedBox(new Vector3(3, 3, 3), new Vector3(2, 2, 2));
            yield return new TestCase(diag_A, diag_B, false, default, default, "Box-Box: diagonal separation");

            // --- Diagonal overlap: overlapping along combined diagonal direction ---

            var diagO_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var diagO_B = CreateAxisAlignedBox(new Vector3(1.5f, 1.5f, 1.5f), new Vector3(2, 2, 2));
            yield return new TestCase(diagO_A, diagO_B, true, default, 0.5f, "Box-Box: diagonal overlap") { SkipNormalCheck = true };

            // --- Both boxes rotated 90 degrees (effectively axis-aligned again) ---

            var rot90_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 4, 2));
            rot90_A.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2);
            var rot90_B = CreateAxisAlignedBox(new Vector3(1.5f, 0, 0), new Vector3(2, 4, 2));
            rot90_B.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2);
            yield return new TestCase(rot90_A, rot90_B, true, new Vector3(-1, 0, 0), 0.5f, "Box-Box: both rotated 90deg same axis") { SkipNormalCheck = true };

            // --- Very thin box (like a wall) vs cube ---

            var wall = CreateAxisAlignedBox(new Vector3(0.9f, 0, 0), new Vector3(0.1f, 10, 10));
            var wallCube = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            yield return new TestCase(wallCube, wall, true, new Vector3(-1, 0, 0), 0.05f, "Box-Box: thin wall overlap") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // --- Perpendicular boxes overlapping at cross ---

            var cross_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(10, 1, 1));
            var cross_B = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(1, 1, 10));
            yield return new TestCase(cross_A, cross_B, true, default, 1.0f, "Box-Box: cross overlap") { SkipNormalCheck = true };

            // --- 90-degree rotation around Z making tall box wide ---

            var tall_A = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(1, 4, 1));
            var tall_B = CreateAxisAlignedBox(new Vector3(2, 0, 0), new Vector3(1, 4, 1));
            tall_B.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 2);
            // After rotation: B is now 4 wide, 1 tall. Half-width = 2, centre at x=2, extends from 0 to 4 on X.
            // A half-width = 0.5, extends from -0.5 to 0.5 on X. Overlap = 0.5
            yield return new TestCase(tall_A, tall_B, true, new Vector3(-1, 0, 0), 0.5f, "Box-Box: 90deg Z rotation tall becomes wide") { SkipNormalCheck = true };
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

            public bool SkipNormalCheck { get; set; }

            public bool SkipPenetrationCheck { get; set; }

            public override string ToString() => testName;
        }
    }
}
