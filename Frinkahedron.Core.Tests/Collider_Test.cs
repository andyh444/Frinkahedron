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

            Assert.That(manifold.CollisionFound, Is.EqualTo(testCase.CollisionExpected));
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
            foreach (var tc in GetSphereSphereTestCases())
                yield return tc;

            foreach (var tc in GetSphereBoxTestCases())
                yield return tc;

            foreach (var tc in GetSphereCapsuleTestCases())
                yield return tc;

            foreach (var tc in GetBoxBoxTestCases())
                yield return tc;

            foreach (var tc in GetBoxCapsuleTestCases())
                yield return tc;

            foreach (var tc in GetCapsuleCapsuleTestCases())
                yield return tc;
        }

        // ===== Sphere-Sphere =====

        private static IEnumerable<TestCase> GetSphereSphereTestCases()
        {
            // Overlapping along X
            var s1 = CreateSphere(new Vector3(0, 0, 0), 1.01f);
            var s2 = CreateSphere(new Vector3(2, 0, 0), 1.01f);
            yield return new TestCase(s1, s2, true, new Vector3(-1, 0, 0), 0.02f, "Sphere-Sphere: colliding X");

            // Separated along X
            var s3 = CreateSphere(new Vector3(3, 0, 0), 1.01f);
            yield return new TestCase(s1, s3, false, default, default, "Sphere-Sphere: separated X");

            // Overlapping along Y
            var s4 = CreateSphere(new Vector3(0, 0, 0), 1.0f);
            var s5 = CreateSphere(new Vector3(0, 1.5f, 0), 1.0f);
            yield return new TestCase(s4, s5, true, new Vector3(0, -1, 0), 0.5f, "Sphere-Sphere: colliding Y");

            // Overlapping along Z
            var s6 = CreateSphere(new Vector3(0, 0, 0), 1.0f);
            var s7 = CreateSphere(new Vector3(0, 0, 1.5f), 1.0f);
            yield return new TestCase(s6, s7, true, new Vector3(0, 0, -1), 0.5f, "Sphere-Sphere: colliding Z");

            // Separated along Y
            var s8 = CreateSphere(new Vector3(0, 0, 0), 1.0f);
            var s9 = CreateSphere(new Vector3(0, 3, 0), 1.0f);
            yield return new TestCase(s8, s9, false, default, default, "Sphere-Sphere: separated Y");

            // Separated along Z
            var s10 = CreateSphere(new Vector3(0, 0, 0), 1.0f);
            var s11 = CreateSphere(new Vector3(0, 0, 3), 1.0f);
            yield return new TestCase(s10, s11, false, default, default, "Sphere-Sphere: separated Z");

            // Identical positions (fully overlapping)
            var s12 = CreateSphere(new Vector3(0, 0, 0), 1.0f);
            var s13 = CreateSphere(new Vector3(0, 0, 0), 1.0f);
            yield return new TestCase(s12, s13, true, default, 2.0f, "Sphere-Sphere: identical position") { SkipNormalCheck = true };

            // Barely touching (just overlapping)
            var s14 = CreateSphere(new Vector3(0, 0, 0), 1.0f);
            var s15 = CreateSphere(new Vector3(1.99f, 0, 0), 1.0f);
            yield return new TestCase(s14, s15, true, new Vector3(-1, 0, 0), 0.01f, "Sphere-Sphere: barely touching");

            // Barely separated
            var s16 = CreateSphere(new Vector3(0, 0, 0), 1.0f);
            var s17 = CreateSphere(new Vector3(2.01f, 0, 0), 1.0f);
            yield return new TestCase(s16, s17, false, default, default, "Sphere-Sphere: barely separated");

            // Different radii - overlapping
            var s18 = CreateSphere(new Vector3(0, 0, 0), 2.0f);
            var s19 = CreateSphere(new Vector3(2.5f, 0, 0), 1.0f);
            yield return new TestCase(s18, s19, true, new Vector3(-1, 0, 0), 0.5f, "Sphere-Sphere: different radii overlap");

            // Different radii - separated
            var s20 = CreateSphere(new Vector3(0, 0, 0), 2.0f);
            var s21 = CreateSphere(new Vector3(4, 0, 0), 1.0f);
            yield return new TestCase(s20, s21, false, default, default, "Sphere-Sphere: different radii separated");

            // Diagonal overlap
            var s22 = CreateSphere(new Vector3(0, 0, 0), 1.0f);
            float d = 1.0f; // distance along each axis so total distance = sqrt(3) ≈ 1.732
            var s23 = CreateSphere(new Vector3(d, d, d), 1.0f);
            float dist = MathF.Sqrt(3);
            Vector3 normal = -Vector3.Normalize(new Vector3(d, d, d));
            yield return new TestCase(s22, s23, true, normal, 2.0f - dist, "Sphere-Sphere: diagonal overlap");

            // Small sphere inside large sphere
            var s24 = CreateSphere(new Vector3(0, 0, 0), 5.0f);
            var s25 = CreateSphere(new Vector3(1, 0, 0), 0.5f);
            yield return new TestCase(s24, s25, true, new Vector3(-1, 0, 0), 4.5f, "Sphere-Sphere: small inside large") { SkipNormalCheck = true };
        }

        // ===== Sphere-Box =====

        private static IEnumerable<TestCase> GetSphereBoxTestCases()
        {
            // Sphere above box face (Y axis)
            var sph1 = CreateSphere(new Vector3(0, 0, 0), 1.01f);
            var box1 = CreateAxisAlignedBox(new Vector3(0, -2, 0), new Vector3(20, 2, 20));
            yield return new TestCase(sph1, box1, true, new Vector3(0, 1, 0), 0.01f, "Sphere-Box: sphere on box face Y");

            // Sphere separated from box along X
            var sph2 = CreateSphere(new Vector3(5, 0, 0), 1.0f);
            var box2 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            yield return new TestCase(sph2, box2, false, default, default, "Sphere-Box: separated X");

            // Sphere overlapping box face along X
            var sph3 = CreateSphere(new Vector3(1.8f, 0, 0), 1.0f);
            var box3 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            yield return new TestCase(sph3, box3, true, new Vector3(1, 0, 0), 0.2f, "Sphere-Box: overlap face X");

            // Sphere overlapping box face along Z
            var sph4 = CreateSphere(new Vector3(0, 0, 1.8f), 1.0f);
            var box4 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            yield return new TestCase(sph4, box4, true, new Vector3(0, 0, 1), 0.2f, "Sphere-Box: overlap face Z");

            // Sphere separated from box along Y
            var sph5 = CreateSphere(new Vector3(0, 5, 0), 1.0f);
            var box5 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            yield return new TestCase(sph5, box5, false, default, default, "Sphere-Box: separated Y");

            // Sphere separated from box along Z
            var sph6 = CreateSphere(new Vector3(0, 0, 5), 1.0f);
            var box6 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            yield return new TestCase(sph6, box6, false, default, default, "Sphere-Box: separated Z");

            // Sphere centre inside box
            var sph7 = CreateSphere(new Vector3(0, 0, 0), 0.5f);
            var box7 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(4, 4, 4));
            yield return new TestCase(sph7, box7, true, default, default, "Sphere-Box: sphere inside box") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Sphere near box edge (diagonal approach in XY plane)
            var sph8 = CreateSphere(new Vector3(1.5f, 1.5f, 0), 0.8f);
            var box8 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            // Closest point on box to sphere centre is corner (1,1,0). Distance = sqrt(0.5^2+0.5^2) ≈ 0.707. Radius 0.8 > 0.707 → overlap
            yield return new TestCase(sph8, box8, true, default, default, "Sphere-Box: near edge XY") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Sphere near box corner (diagonal approach in XYZ)
            var sph9 = CreateSphere(new Vector3(1.5f, 1.5f, 1.5f), 1.0f);
            var box9 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            // Closest point on box to sphere centre is corner (1,1,1). Distance = sqrt(0.5^2+0.5^2+0.5^2) ≈ 0.866. Radius 1.0 > 0.866 → overlap
            yield return new TestCase(sph9, box9, true, default, default, "Sphere-Box: near corner XYZ") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Sphere near box corner but separated
            var sph10 = CreateSphere(new Vector3(2, 2, 2), 0.5f);
            var box10 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            // Closest point on box to sphere centre is corner (1,1,1). Distance = sqrt(1+1+1) ≈ 1.732. Radius 0.5 → separated
            yield return new TestCase(sph10, box10, false, default, default, "Sphere-Box: separated corner");

            // Sphere vs rotated box - overlapping
            var sph11 = CreateSphere(new Vector3(1.5f, 0, 0), 0.5f);
            var box11 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            box11.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 4);
            // Rotated box extends further along X (half-diagonal ≈ 1.414). Sphere at 1.5 with r=0.5, so overlapping
            yield return new TestCase(sph11, box11, true, default, default, "Sphere-Box: rotated box overlap") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Sphere vs rotated box - separated
            var sph12 = CreateSphere(new Vector3(3, 0, 0), 0.5f);
            var box12 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            box12.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 4);
            yield return new TestCase(sph12, box12, false, default, default, "Sphere-Box: rotated box separated");

            // Large sphere engulfing small box
            var sph13 = CreateSphere(new Vector3(0, 0, 0), 10.0f);
            var box13 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            yield return new TestCase(sph13, box13, true, default, default, "Sphere-Box: large sphere engulfs box") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Sphere barely touching box face
            var sph14 = CreateSphere(new Vector3(1.99f, 0, 0), 1.0f);
            var box14 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            yield return new TestCase(sph14, box14, true, new Vector3(1, 0, 0), 0.01f, "Sphere-Box: barely touching face X");

            // Sphere barely separated from box face
            var sph15 = CreateSphere(new Vector3(2.01f, 0, 0), 1.0f);
            var box15 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            yield return new TestCase(sph15, box15, false, default, default, "Sphere-Box: barely separated face X");

            // Sphere on flat floor box
            var sph16 = CreateSphere(new Vector3(0, 0.9f, 0), 1.0f);
            var box16 = CreateAxisAlignedBox(new Vector3(0, -5, 0), new Vector3(100, 10, 100));
            yield return new TestCase(sph16, box16, true, new Vector3(0, 1, 0), 0.1f, "Sphere-Box: sphere on floor");
        }

        // ===== Sphere-Capsule =====

        private static IEnumerable<TestCase> GetSphereCapsuleTestCases()
        {
            // Sphere overlapping capsule side (perpendicular to capsule axis)
            var sph1 = CreateSphere(new Vector3(2, 0, 0), 1.0f);
            var cap1 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            // Closest point on capsule axis to sphere centre (2,0,0) is (0,0,0). Distance = 2. Radii sum = 1.5 → separated
            yield return new TestCase(sph1, cap1, false, default, default, "Sphere-Capsule: separated side");

            // Sphere overlapping capsule side
            var sph2 = CreateSphere(new Vector3(1.2f, 0, 0), 1.0f);
            var cap2 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            // Distance from axis = 1.2. Radii sum = 1.5 → overlap 0.3
            yield return new TestCase(sph2, cap2, true, new Vector3(1, 0, 0), 0.3f, "Sphere-Capsule: overlap side");

            // Sphere at capsule top endpoint
            var sph3 = CreateSphere(new Vector3(0, 2.3f, 0), 1.0f);
            var cap3 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            // Top endpoint at (0,1,0). Distance = 1.3. Radii sum = 1.5 → overlap 0.2
            yield return new TestCase(sph3, cap3, true, new Vector3(0, 1, 0), 0.2f, "Sphere-Capsule: overlap top endpoint");

            // Sphere at capsule bottom endpoint
            var sph4 = CreateSphere(new Vector3(0, -2.3f, 0), 1.0f);
            var cap4 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            yield return new TestCase(sph4, cap4, true, new Vector3(0, -1, 0), 0.2f, "Sphere-Capsule: overlap bottom endpoint");

            // Sphere separated from capsule top
            var sph5 = CreateSphere(new Vector3(0, 3, 0), 1.0f);
            var cap5 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            yield return new TestCase(sph5, cap5, false, default, default, "Sphere-Capsule: separated top");

            // Sphere separated from capsule bottom
            var sph6 = CreateSphere(new Vector3(0, -3, 0), 1.0f);
            var cap6 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            yield return new TestCase(sph6, cap6, false, default, default, "Sphere-Capsule: separated bottom");

            // Sphere overlapping capsule along Z side
            var sph7 = CreateSphere(new Vector3(0, 0, 1.2f), 1.0f);
            var cap7 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            yield return new TestCase(sph7, cap7, true, new Vector3(0, 0, 1), 0.3f, "Sphere-Capsule: overlap side Z");

            // Sphere at same centre as capsule
            var sph8 = CreateSphere(new Vector3(0, 0, 0), 1.0f);
            var cap8 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            yield return new TestCase(sph8, cap8, true, default, default, "Sphere-Capsule: coincident centres") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Sphere vs rotated capsule (capsule tilted 90 deg around Z, so axis becomes horizontal along X)
            var sph9 = CreateSphere(new Vector3(2, 0, 0), 1.0f);
            var cap9 = CreateCapsule(new Vector3(0, 0, 0), 4.0f, 0.5f);
            cap9.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 2);
            // Capsule axis now along X, endpoints at (-2,0,0) and (2,0,0). Sphere at (2,0,0) → distance from endpoint = 0. Radii sum = 1.5 → overlap
            yield return new TestCase(sph9, cap9, true, default, default, "Sphere-Capsule: rotated capsule overlap") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Sphere separated from rotated capsule
            var sph10 = CreateSphere(new Vector3(5, 0, 0), 1.0f);
            var cap10 = CreateCapsule(new Vector3(0, 0, 0), 4.0f, 0.5f);
            cap10.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 2);
            yield return new TestCase(sph10, cap10, false, default, default, "Sphere-Capsule: rotated capsule separated");

            // Small sphere inside large capsule
            var sph11 = CreateSphere(new Vector3(0, 0, 0), 0.1f);
            var cap11 = CreateCapsule(new Vector3(0, 0, 0), 4.0f, 3.0f);
            yield return new TestCase(sph11, cap11, true, default, default, "Sphere-Capsule: small sphere inside large capsule") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Sphere near capsule diagonal approach
            var sph12 = CreateSphere(new Vector3(1.0f, 1.5f, 0), 1.0f);
            var cap12 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            // Top endpoint at (0,1,0). Distance from (1,1.5,0) to (0,1,0) = sqrt(1+0.25) ≈ 1.118. Radii sum = 1.5 → overlap
            yield return new TestCase(sph12, cap12, true, default, default, "Sphere-Capsule: diagonal near endpoint") { SkipNormalCheck = true, SkipPenetrationCheck = true };
        }

        // ===== Box-Capsule =====

        private static IEnumerable<TestCase> GetBoxCapsuleTestCases()
        {
            // Capsule standing on box (vertical capsule above box face)
            var box1 = CreateAxisAlignedBox(new Vector3(0, -5, 0), new Vector3(20, 10, 20));
            var cap1 = CreateCapsule(new Vector3(0, 1.2f, 0), 2.0f, 0.5f);
            // Capsule bottom endpoint at (0, 0.2, 0), bottom hemisphere at y = 0.2 - 0.5 = -0.3. Box top at y = 0. Overlap
            yield return new TestCase(cap1, box1, true, new Vector3(0, 1, 0), 0.3f, "Box-Capsule: capsule standing on floor") { SkipPenetrationCheck = true };

            // Capsule above box - separated
            var box2 = CreateAxisAlignedBox(new Vector3(0, -5, 0), new Vector3(20, 10, 20));
            var cap2 = CreateCapsule(new Vector3(0, 2, 0), 2.0f, 0.5f);
            // Capsule bottom at y=1, minus radius 0.5 → y=0.5. Box top at y=0. Separated
            yield return new TestCase(cap2, box2, false, default, default, "Box-Capsule: capsule above floor separated");

            // Capsule horizontal alongside box face X
            var box3 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var cap3 = CreateCapsule(new Vector3(3, 0, 0), 2.0f, 0.5f);
            cap3.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 2);
            // Capsule axis now along X, endpoints at (2,0,0) and (4,0,0). Box face at x=1. Closest axis point is (2,0,0), distance from box = 1. Radius = 0.5 → separated
            yield return new TestCase(box3, cap3, false, default, default, "Box-Capsule: horizontal capsule separated X");

            // Capsule horizontal overlapping box face X
            var box4 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var cap4 = CreateCapsule(new Vector3(1.3f, 0, 0), 2.0f, 0.5f);
            cap4.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 2);
            yield return new TestCase(box4, cap4, true, default, default, "Box-Capsule: horizontal capsule overlap X") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Vertical capsule along box edge
            var box5 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var cap5 = CreateCapsule(new Vector3(1.2f, 0, 1.2f), 4.0f, 0.5f);
            // Box corner at (1,y,1). Distance from (1.2,0,1.2) to edge = sqrt(0.04+0.04) ≈ 0.283. Radius 0.5 → overlap
            yield return new TestCase(box5, cap5, true, default, default, "Box-Capsule: capsule near box edge") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Vertical capsule far from box
            var box6 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var cap6 = CreateCapsule(new Vector3(5, 0, 0), 2.0f, 0.5f);
            yield return new TestCase(box6, cap6, false, default, default, "Box-Capsule: capsule far from box");

            // Capsule inside box
            var box7 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(10, 10, 10));
            var cap7 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            yield return new TestCase(box7, cap7, true, default, default, "Box-Capsule: capsule inside box") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Capsule endpoint touching box face along Y from below
            var box8 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(4, 2, 4));
            var cap8 = CreateCapsule(new Vector3(0, -2.3f, 0), 2.0f, 0.5f);
            // Top endpoint at (0,-1.3,0), plus radius 0.5 → reaches y=-0.8. Box bottom at y=-1. Separated? top endpoint y=-1.3+0.5=-0.8, box bottom=-1. -0.8 > -1 → overlap
            yield return new TestCase(box8, cap8, true, default, default, "Box-Capsule: capsule endpoint near bottom face") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Capsule diagonal to box - separated
            var box9 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var cap9 = CreateCapsule(new Vector3(3, 3, 3), 2.0f, 0.5f);
            yield return new TestCase(box9, cap9, false, default, default, "Box-Capsule: diagonal separated");

            // Rotated capsule (45 deg around Z) overlapping box
            var box10 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            var cap10 = CreateCapsule(new Vector3(1.5f, 0, 0), 3.0f, 0.5f);
            cap10.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 4);
            yield return new TestCase(box10, cap10, true, default, default, "Box-Capsule: rotated capsule 45deg overlap") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Rotated box vs vertical capsule
            var box11 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(2, 2, 2));
            box11.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 4);
            var cap11 = CreateCapsule(new Vector3(1.8f, 0, 0), 2.0f, 0.5f);
            // Rotated box extends to sqrt(2) ≈ 1.414 along X. Capsule at 1.8 - 0.5 = 1.3 → overlap
            yield return new TestCase(box11, cap11, true, default, default, "Box-Capsule: rotated box vs capsule") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Thin box (wall) vs capsule going through it
            var box12 = CreateAxisAlignedBox(new Vector3(0, 0, 0), new Vector3(0.1f, 10, 10));
            var cap12 = CreateCapsule(new Vector3(0, 0, 0), 4.0f, 0.5f);
            cap12.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 2);
            yield return new TestCase(box12, cap12, true, default, default, "Box-Capsule: capsule through thin wall") { SkipNormalCheck = true, SkipPenetrationCheck = true };
        }

        // ===== Capsule-Capsule =====

        private static IEnumerable<TestCase> GetCapsuleCapsuleTestCases()
        {
            // Parallel vertical capsules - overlapping
            var cap1 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            var cap2 = CreateCapsule(new Vector3(0.8f, 0, 0), 2.0f, 0.5f);
            yield return new TestCase(cap1, cap2, true, new Vector3(-1, 0, 0), 0.2f, "Capsule-Capsule: parallel overlap X");

            // Parallel vertical capsules - separated
            var cap3 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            var cap4 = CreateCapsule(new Vector3(2, 0, 0), 2.0f, 0.5f);
            yield return new TestCase(cap3, cap4, false, default, default, "Capsule-Capsule: parallel separated X");

            // Parallel vertical capsules - separated along Z
            var cap5 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            var cap6 = CreateCapsule(new Vector3(0, 0, 2), 2.0f, 0.5f);
            yield return new TestCase(cap5, cap6, false, default, default, "Capsule-Capsule: parallel separated Z");

            // End-to-end vertical capsules - overlapping
            var cap7 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            var cap8 = CreateCapsule(new Vector3(0, 2.8f, 0), 2.0f, 0.5f);
            // Cap7 top at y=1 + r=0.5 → 1.5. Cap8 bottom at y=2.8-1 - r=0.5 → 1.3. Overlap along Y? Endpoints: cap7 top=(0,1,0), cap8 bottom=(0,1.8,0). Distance=0.8. Radii sum=1.0 → overlap 0.2
            yield return new TestCase(cap7, cap8, true, new Vector3(0, -1, 0), 0.2f, "Capsule-Capsule: end-to-end overlap Y");

            // End-to-end vertical capsules - separated
            var cap9 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            var cap10 = CreateCapsule(new Vector3(0, 4, 0), 2.0f, 0.5f);
            // Cap9 top endpoint=(0,1,0), cap10 bottom=(0,3,0). Distance=2. Radii sum=1.0 → separated
            yield return new TestCase(cap9, cap10, false, default, default, "Capsule-Capsule: end-to-end separated Y");

            // Perpendicular capsules - overlapping (T-shape)
            var cap11 = CreateCapsule(new Vector3(0, 0, 0), 4.0f, 0.5f);
            var cap12 = CreateCapsule(new Vector3(0.8f, 0, 0), 4.0f, 0.5f);
            cap12.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 2);
            // The rotated capsule's axis passes through the vertical capsule's axis, closest distance ≈ 0
            yield return new TestCase(cap11, cap12, true, default, default, "Capsule-Capsule: perpendicular overlap") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Perpendicular capsules - separated
            var cap13 = CreateCapsule(new Vector3(0, 0, 0), 4.0f, 0.5f);
            var cap14 = CreateCapsule(new Vector3(4, 0, 0), 4.0f, 0.5f);
            cap14.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 2);
            // Rotated axis endpoints at (4-2,0,0)=(2,0,0) and (4+2,0,0)=(6,0,0). Nearest point to A's axis is (2,0,0), distance=2. Radii sum=1.0 → separated
            yield return new TestCase(cap13, cap14, false, default, default, "Capsule-Capsule: perpendicular separated");

            // Coincident capsules (same position and size)
            var cap15 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            var cap16 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            yield return new TestCase(cap15, cap16, true, default, 1.0f, "Capsule-Capsule: coincident") { SkipNormalCheck = true };

            // Small capsule inside large capsule
            var cap17 = CreateCapsule(new Vector3(0, 0, 0), 1.0f, 0.2f);
            var cap18 = CreateCapsule(new Vector3(0, 0, 0), 6.0f, 3.0f);
            yield return new TestCase(cap17, cap18, true, default, default, "Capsule-Capsule: small inside large") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Crossed capsules at angle - overlapping
            var cap19 = CreateCapsule(new Vector3(0, 0, 0), 4.0f, 0.5f);
            var cap20 = CreateCapsule(new Vector3(0, 0, 0), 4.0f, 0.5f);
            cap20.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2);
            // One vertical (Y axis), one along Z axis. They cross at origin. Distance=0. Radii sum=1.0 → overlap
            yield return new TestCase(cap19, cap20, true, default, 1.0f, "Capsule-Capsule: crossed at origin") { SkipNormalCheck = true };

            // Different radii - overlapping
            var cap21 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 1.0f);
            var cap22 = CreateCapsule(new Vector3(1.8f, 0, 0), 2.0f, 0.5f);
            // Distance between axes = 1.8. Radii sum = 1.5 → 1.8 > 1.5, separated
            yield return new TestCase(cap21, cap22, false, default, default, "Capsule-Capsule: different radii separated");

            // Different radii - overlapping
            var cap23 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 1.0f);
            var cap24 = CreateCapsule(new Vector3(1.2f, 0, 0), 2.0f, 0.5f);
            // Distance between axes = 1.2. Radii sum = 1.5 → overlap 0.3
            yield return new TestCase(cap23, cap24, true, new Vector3(-1, 0, 0), 0.3f, "Capsule-Capsule: different radii overlap");

            // Diagonal approach capsules - barely overlapping endpoints
            var cap25 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            var cap26 = CreateCapsule(new Vector3(0.5f, 2.6f, 0), 2.0f, 0.5f);
            // Cap25 top=(0,1,0), cap26 bottom=(0.5,1.6,0). Distance = sqrt(0.25+0.36) ≈ 0.781. Radii sum = 1.0 → overlap
            yield return new TestCase(cap25, cap26, true, default, default, "Capsule-Capsule: diagonal endpoint overlap") { SkipNormalCheck = true, SkipPenetrationCheck = true };

            // Diagonal approach capsules - separated
            var cap27 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.3f);
            var cap28 = CreateCapsule(new Vector3(3, 3, 0), 2.0f, 0.3f);
            yield return new TestCase(cap27, cap28, false, default, default, "Capsule-Capsule: diagonal separated");

            // Rotated capsules forming an X shape - overlapping
            var cap29 = CreateCapsule(new Vector3(0, 0, 0), 4.0f, 0.5f);
            cap29.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 4);
            var cap30 = CreateCapsule(new Vector3(0, 0, 0), 4.0f, 0.5f);
            cap30.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -MathF.PI / 4);
            yield return new TestCase(cap29, cap30, true, default, 1.0f, "Capsule-Capsule: X shape overlap") { SkipNormalCheck = true };

            // Parallel horizontal capsules - overlapping along Z
            var cap31 = CreateCapsule(new Vector3(0, 0, 0), 4.0f, 0.5f);
            cap31.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2);
            var cap32 = CreateCapsule(new Vector3(0, 0.8f, 0), 4.0f, 0.5f);
            cap32.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2);
            // Both along Z. Distance between axes = 0.8. Radii sum = 1.0 → overlap 0.2
            yield return new TestCase(cap31, cap32, true, new Vector3(0, -1, 0), 0.2f, "Capsule-Capsule: parallel horizontal overlap Y");

            // Capsule barely touching
            var cap33 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            var cap34 = CreateCapsule(new Vector3(0.99f, 0, 0), 2.0f, 0.5f);
            yield return new TestCase(cap33, cap34, true, new Vector3(-1, 0, 0), 0.01f, "Capsule-Capsule: barely touching");

            // Capsule barely separated
            var cap35 = CreateCapsule(new Vector3(0, 0, 0), 2.0f, 0.5f);
            var cap36 = CreateCapsule(new Vector3(1.01f, 0, 0), 2.0f, 0.5f);
            yield return new TestCase(cap35, cap36, false, default, default, "Capsule-Capsule: barely separated");
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

        private static TestObject CreateCapsule(Vector3 centre, float pointToPointLength, float radius)
        {
            Position position = new Position(centre, Quaternion.Identity);
            Capsule collider = new Capsule(pointToPointLength, radius);
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
