using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Veldrid;
using Frinkahedron.Core;
using Vulkan.Xlib;
using System.Diagnostics;
using Vulkan;
using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using Frinkahedron.VeldridImplementation;

namespace Frinkahedron.TestApp
{

    internal sealed class TestApp
    {
        private Sdl2Window _window;
        private GraphicsDevice _graphicsDevice;
        private GraphicsResources _graphicsResources;
        private Scene _scene;
        private Task _warmupTask;

        public TestApp()
        {
            _warmupTask = Task.Run(() => Warmup());

            WindowCreateInfo windowCI = new WindowCreateInfo()
            {
                X = 100,
                Y = 100,
                WindowWidth = 1600,
                WindowHeight = 900,
                WindowTitle = "Frinkahedron Test App"
            };
            _window = VeldridStartup.CreateWindow(ref windowCI);
            
            GraphicsDeviceOptions options = new GraphicsDeviceOptions
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true,
                SwapchainDepthFormat = PixelFormat.D32_Float_S8_UInt,
                SyncToVerticalBlank = true
            };
            _graphicsDevice = VeldridStartup.CreateGraphicsDevice(_window, options);
            _scene = CreateScene();
            _graphicsResources = GraphicsResources.CreateResources(_graphicsDevice, _window.Width, _window.Height);

            _warmupTask.Wait();
        }

        private void Warmup()
        {
            Scene scene = CreateScene();
            GameState gameState = new GameState(0.01f, scene);
            for (int i = 0; i < 100; i++)
            {
                scene.Update(gameState);
            }
        }

        private GameObject CreateBigBox(Vector3 position, Quaternion orientation, bool first = false)
        {
            Box box = new Box(new Vector3(100, 10, 100));
            var obj = new GameObject(position,
                !first ? null : new CompositeBehaviour([/*new OrbitalCameraMouseBehaviour(),*/
                    new ImpulseOnClickBehaviour()]),
                box,
                new Core.Physics.RigidBody {
                    Mass = float.PositiveInfinity,
                    InverseInertia = new DiagonalMatrix3x3(),
                    Gravity = false,
                    Material = new PhysicsMaterial(0, 0.8f)
                },
                new ModelRenderable("crate", Matrix4x4.CreateScale(1f / 8f) * Matrix4x4.CreateScale(box.Dimensions)));
            
            obj.Position.Orientation = orientation;
            return obj;
        }

        private Scene CreateScene()
        {
            List<GameObject> gameObjects = new List<GameObject>();

            Random r = Random.Shared;

            gameObjects.Add(CreateBigBox(new Vector3(0, -20, 0), Quaternion.Identity, true));
            gameObjects.Add(CreateBigBox(new Vector3(0, 10, 93), Quaternion.CreateFromYawPitchRoll(0, -MathF.PI / 5, 0)));
            gameObjects.Add(CreateBigBox(new Vector3(0, 10, -93), Quaternion.CreateFromYawPitchRoll(0, MathF.PI / 5, 0)));
            gameObjects.Add(CreateBigBox(new Vector3(-93, 10, 0), Quaternion.CreateFromYawPitchRoll(0, 0, -MathF.PI / 5)));
            gameObjects.Add(CreateBigBox(new Vector3(93, 10, 0), Quaternion.CreateFromYawPitchRoll(0, 0, MathF.PI / 5)));

            Cylinder cyl = new Cylinder(1, 2.5f);
            float mass = 1 * cyl.CalculateVolume();
            GameObject obj = new GameObject(new Vector3(0, -13, 0),
                new OrbitalCameraMouseBehaviour(),
                cyl,
                new RigidBody {
                    Mass = mass,
                    InverseInertia = cyl.CalculateFilledInertia(mass).GetInverse(),
                    Gravity = true,
                    Material = new PhysicsMaterial(0.0f, 0.8f) },
                new ModelRenderable("tincan", Matrix4x4.CreateRotationX(-MathF.PI / 2) * Matrix4x4.CreateScale(1 / 0.053f, 1 / 0.158f, 1 / 0.053f) * Matrix4x4.CreateScale(cyl.Radius, cyl.Height, cyl.Radius)));
            gameObjects.Add(obj);

            /*for (int k = -1; k <= 2; k++)
            {
                for (int j = -4; j < 4; j++)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        Box box = new Box(new Vector3(1, 2, 1));
                        float mass = 1 * box.CalculateVolume();
                        GameObject obj = new GameObject(new Vector3(k * 1.01f, -14 + i * 2, j * 1.01f),
                            null,
                            box,
                            new RigidBody { Mass = mass, InverseInertia = box.CalculateFilledInertia(mass).GetInverse(), Gravity = true, Material = new PhysicsMaterial(0.0f, 0.8f) });
                        gameObjects.Add(obj);
                    }
                }
            }*/

            /*Sphere sph = new Sphere(4);
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
                });
            sphObj.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI);
            gameObjects.Add(sphObj);*/

            /*bool firstSphere = true;
            for (int i = 0; i < 10; i++)
            {
                IShape collider;
                float density;
                var rand = r.NextSingle();
                Behaviour? behaviour = null;
                //if (rand > 0.7f)
                //{
                //    float radius = r.NextSingle(1.5f, 2.5f);
                //    collider = new Sphere(radius);
                //    density = 10f;
                //    if (firstSphere)
                //    {
                //        //behaviour = new CompositeBehaviour([new SphereControlBehaviour(), new OrbitalCameraMouseBehaviour { distance = 15f}]);
                //        firstSphere = false;
                //        density = 50f;
                //    }
                //}
                //else if (rand > 0.45f)
                //{
                //    float radius = r.NextSingle(1.5f, 2.5f);
                //    float length = r.NextSingle(2.5f, 5.5f);
                //    density = 1f;
                //    collider = new Capsule(length, radius);
                //}
                //else
                //{
                //    Vector3 dimensions = new Vector3(r.NextSingle(1f, 3f), r.NextSingle(1f, 3f), r.NextSingle(1f, 9f));
                //    collider = new Box(dimensions);
                //    density = 1f;
                //}


                collider = new Cylinder(r.NextSingle(1f, 3f), r.NextSingle(4f, 10f));
                density = 1;

                float volume = collider.CalculateVolume();
                float mass = density * volume;

                var inertia = collider.CalculateFilledInertia(mass);

                gameObjects.Add(new GameObject(
                    new Vector3(0, r.NextSingle(20f, 35f), r.NextSingle(-130f, 130f)),
                    behaviour,
                    collider,
                    new Core.Physics.RigidBody
                    {
                        Mass = mass,
                        InverseInertia = inertia.GetInverse(),
                        Gravity = true,
                        Velocity = r.NextSingle(0f, 20f) * new Vector3(r.NextSingle(-1f, 1f), r.NextSingle(-1f, 1f), r.NextSingle(-0f, 0f)),
                        AngularVelocity = r.NextSingle(0f, 6f) * new Vector3(r.NextSingle(-1f, 1f), r.NextSingle(-1f, 1f), r.NextSingle(-1f, 1f)),
                        Material = new PhysicsMaterial(0.1f, 0.8f)
                    }));

                gameObjects.Last().Position.Orientation = Quaternion.CreateFromYawPitchRoll(r.NextSingle(0, 0.15f * MathF.PI), r.NextSingle(0, 0.15f * MathF.PI), r.NextSingle(0, 0.15f * MathF.PI));
            }*/




            //gameObjects.Add(new GameObject(new Vector3(-1, 0, 0), new CompositeBehaviour([new ContinuousRotationBehaviour(0.1f, 0.4f, 0.2f), new OrbitalCameraMouseBehaviour()]), new BoxCollider(new Vector3(1, 1.25f, 1.5f))));
            //gameObjects.Add(new GameObject(new Vector3(1, 0, 0), new ContinuousRotationBehaviour(-0.5f, 0.1f, 0.3f), new SphereCollider(0.5f)));

            var scene = new Scene(new Vector3(0, 0, -2), new Vector3(0, 0, 1), gameObjects);
            scene.SceneLights.PointLights.Add(new PointLight(new Vector3(), new Vector3(1), 100f));
            scene.SceneLights.PointLights.Add(new PointLight(new Vector3(0, 0, -75), new Vector3(1, 0, 0), 200f));
            scene.SceneLights.PointLights.Add(new PointLight(new Vector3(0, 0, 75), new Vector3(0, 1, 0), 300f));

            scene.SceneLights.DirectionalLight = new DirectionalLight(Vector3.Normalize(new Vector3(-0.5f, -1f, -0.5f)), new Vector3(1));

            return scene;
        }

        public void Run()
        {
            try
            {
                var gameState = new GameState(0.01f, _scene);
                Stopwatch sw = Stopwatch.StartNew();
                while (_window.Exists)
                {
                    var inputSnapshot = _window.PumpEvents();
                    UpdateInput(gameState.Input, inputSnapshot);
                    if (gameState.Input.IsKeyPressed(Core.Key.R))
                    {
                        _scene = CreateScene();
                        gameState = new GameState(0.01f, _scene);
                    }
                    else
                    {
                        gameState.DeltaTime /= 20;
                        for (int i = 0; i < 20; i++)
                        {
                            _scene.Update(gameState);
                            gameState.Input.Clear();
                        }
                        var start = Stopwatch.GetTimestamp();
                        Draw();
                        Console.WriteLine($"Draw took {Stopwatch.GetElapsedTime(start).TotalMilliseconds:#0.000} ms");

                        //gameState.Input.Clear();

                        sw.Stop();
                        gameState.DeltaTime = (float)sw.Elapsed.TotalSeconds;
                        sw.Restart();
                    }
                }
            }
            finally
            {
                _graphicsDevice.Dispose();
                _graphicsResources.Dispose();
            }
        }

        private void UpdateInput(Input input, InputSnapshot snapshot)
        {
            foreach (var keyEvent in snapshot.KeyEvents)
            {
                if (keyEvent.Down)
                {
                    input.NewKeyDown(keyEvent.Key.ToCoreKey());
                }
                else
                {
                    input.NewKeyUp(keyEvent.Key.ToCoreKey());
                }
            }

            foreach (var mouseEvent in snapshot.MouseEvents)
            {
                Core.MouseButton mouseButton = mouseEvent.MouseButton switch
                {
                    Veldrid.MouseButton.Left => Core.MouseButton.Left,
                    Veldrid.MouseButton.Middle => Core.MouseButton.Middle,
                    Veldrid.MouseButton.Right => Core.MouseButton.Right,
                    _ => Core.MouseButton.None
                };
                if (mouseEvent.Down)
                {
                    input.NewMouseButtonDown(mouseButton);
                }
                else
                {
                    input.NewMouseButtonUp(mouseButton);
                }
            }
            input.SetScrollDelta((int)snapshot.WheelDelta);
            input.SetMousePosition(snapshot.MousePosition, new Vector2(_window.Width, _window.Height));
        }

        private void Draw()
        {
            VeldridRenderContext context = new VeldridRenderContext();
            _scene.Draw(context);

            _graphicsResources.CommandList.Begin();
            foreach (var renderPass in _graphicsResources.RenderPasses)
            {
                renderPass.RenderScene(_graphicsDevice, _graphicsResources.CommandList, _graphicsResources, _scene, context.DrawInstructions);
            }
            _graphicsDevice.SwapBuffers();
            _graphicsResources.CommandList.End();
            _graphicsDevice.SubmitCommands(_graphicsResources.CommandList);
        }
    }
}
