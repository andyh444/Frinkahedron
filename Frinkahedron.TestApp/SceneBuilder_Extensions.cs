using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using Frinkahedron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Frinkahedron.Core.Colliders;

namespace Frinkahedron.TestApp
{
    internal static class SceneBuilder_Extensions
    {
        public static void AddBowlingBall(this SceneBuilder sceneBuilder)
        {
            Sphere sph = new Sphere(4);
            float sphMass = 10 * sph.CalculateVolume();
            GameObject sphObj = new GameObject(new Vector3(-60, 0, 0),
                new SphereControlBehaviour(),
                //new CompositeBehaviour([new SphereControlBehaviour(), new OrbitalCameraMouseBehaviour()]),
                sph,
                new RigidBody
                {
                    Mass = sphMass,
                    InverseInertia = sph.CalculateFilledInertia(sphMass).GetInverse(),
                    Gravity = true,
                    Velocity = new Vector3(3, 0, 0),
                    AngularVelocity = new Vector3(0.5f, 1f, 1.5f),
                },
                new ModelRenderable("bowlingball", Matrix4x4.Identity)); // TODO: Check if scale is correct
            sphObj.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI);
            sceneBuilder.AddObject(sphObj);
        }

        public static void AddCrateTower(this SceneBuilder sceneBuilder, Vector3 centrePoint)
        {
            for (int k = -1; k <= 1; k++)
            {
                for (int j = -4; j < 4; j++)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Box box = new Box(new Vector3(1, 2, 1));
                        float mass = 1 * box.CalculateVolume();
                        GameObject obj = new GameObject(centrePoint + new Vector3(k * 1.01f, i * 2, j * 1.01f),
                            null,
                            box,
                            new RigidBody { Mass = mass, InverseInertia = box.CalculateFilledInertia(mass).GetInverse(), Gravity = true, Material = new PhysicsMaterial(0.0f, 0.8f) },
                            new ModelRenderable("crate", Matrix4x4.CreateScale(box.Dimensions / 8f)));
                        sceneBuilder.AddObject(obj);
                    }
                }
            }
        }

        public static void AddBasicCar(this SceneBuilder sceneBuilder)
        {
            Box carBox = new Box(new Vector3(2.7f, 1.8f, 6.7f));
            float carMass = 1 * carBox.CalculateVolume();

            var transform = Matrix4x4.CreateRotationX(-MathF.PI / 2) * Matrix4x4.CreateScale(0.01f) * Matrix4x4.CreateTranslation(0, -1, 0.2f);

            GameObject carObj = new GameObject(new Vector3(-30, -13, 0),
                new CompositeBehaviour([new CarCameraFollowBehaviour(), new CarBehaviour()]),
                carBox,
                new RigidBody
                {
                    Mass = carMass,
                    InverseInertia = carBox.CalculateFilledInertia(carMass).GetInverse(),
                    Gravity = true,
                    Material = new PhysicsMaterial(0.2f, 0.8f)
                },
                new CompositeRenderable([
                    new ModelEntityRenderable("car", 0, transform),
                    new ModelEntityRenderable("car", 1, transform),
                    ]));
            sceneBuilder.AddObject(carObj);
        }

        public static void AddBigBoxes(this SceneBuilder sceneBuilder)
        {
            sceneBuilder.AddObject(CreateBigBox(new Vector3(0, -20, 0), Quaternion.Identity, true));
            sceneBuilder.AddObject(CreateBigBox(new Vector3(0, 10, 93), Quaternion.CreateFromYawPitchRoll(0, -MathF.PI / 5, 0)));
            sceneBuilder.AddObject(CreateBigBox(new Vector3(0, 10, -93), Quaternion.CreateFromYawPitchRoll(0, MathF.PI / 5, 0)));
            sceneBuilder.AddObject(CreateBigBox(new Vector3(-93, 10, 0), Quaternion.CreateFromYawPitchRoll(0, 0, -MathF.PI / 5)));
            sceneBuilder.AddObject(CreateBigBox(new Vector3(93, 10, 0), Quaternion.CreateFromYawPitchRoll(0, 0, MathF.PI / 5)));
        }

        private static GameObject CreateBigBox(Vector3 position, Quaternion orientation, bool first = false)
        {
            Box box = new Box(new Vector3(100, 10, 100));
            var obj = new GameObject(position,
                !first ? null : new CompositeBehaviour([/*new OrbitalCameraMouseBehaviour(),*/
                    new ImpulseOnClickBehaviour()]),
                box,
                new Core.Physics.RigidBody
                {
                    Mass = float.PositiveInfinity,
                    InverseInertia = new DiagonalMatrix3x3(),
                    Gravity = false,
                    Material = new PhysicsMaterial(0, 0.8f)
                },
                new ModelRenderable("crate", Matrix4x4.CreateScale(1f / 8f) * Matrix4x4.CreateScale(box.Dimensions)));

            obj.Position.Orientation = orientation;
            return obj;
        }

    }
}
