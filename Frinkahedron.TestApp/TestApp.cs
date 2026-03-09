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

namespace Frinkahedron.TestApp
{

    internal sealed class TestApp
    {
        private Sdl2Window _window;
        private GraphicsDevice _graphicsDevice;
        private GraphicsResources _graphicsResources;
        private Scene _scene;

        public TestApp()
        {
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

            List<GameObject> gameObjects = new List<GameObject>();
            
            Random r = Random.Shared;

            gameObjects.Add(new GameObject(new Vector3(0, -20, 0),
                new OrbitalCameraMouseBehaviour(),
                new Box(new Vector3(100, 10, 100)),
                new Core.Physics.RigidBody { Mass = float.PositiveInfinity, Inertia = Inertia.CalculateInfiniteInertia(), Gravity = false }));

            gameObjects.Add(new GameObject(new Vector3(0, 10, 93),
                null,
                new Box(new Vector3(100, 10, 100)),
                new Core.Physics.RigidBody { Mass = float.PositiveInfinity, Inertia = Inertia.CalculateInfiniteInertia(), Gravity = false }));

            gameObjects.Last().Position.Orientation = Quaternion.CreateFromYawPitchRoll(0, -MathF.PI / 5, 0);

            gameObjects.Add(new GameObject(new Vector3(0, 10, -93),
                null,
                new Box(new Vector3(100, 10, 100)),
                new Core.Physics.RigidBody { Mass = float.PositiveInfinity, Inertia = Inertia.CalculateInfiniteInertia(), Gravity = false }));

            gameObjects.Last().Position.Orientation = Quaternion.CreateFromYawPitchRoll(0, MathF.PI / 5, 0);


            for (int i = 0; i < 250; i++)
            {
                IShape collider;
                float density;
                if (r.NextSingle() < 0.75f)
                {
                    float radius = r.NextSingle(1.5f, 2.5f);
                    collider = new Sphere(radius);
                    density = 10f;
                }
                else
                {
                    Vector3 dimensions = new Vector3(r.NextSingle(1f, 3f), r.NextSingle(1f, 3f), r.NextSingle(1f, 9f));
                    collider = new Box(dimensions);
                    density = 1f;
                }
                float volume = collider.CalculateVolume();
                float mass = density * volume;

                var inertia = collider.CalculateFilledInertia(mass);

                gameObjects.Add(new GameObject(
                    new Vector3(0, r.NextSingle(5f, 20f), r.NextSingle(-150f, 150f)),
                    null,
                    collider,
                    new Core.Physics.RigidBody
                    {
                        Mass = mass,
                        Inertia = inertia,
                        Gravity = true,
                        Velocity = r.NextSingle(0f, 20f) * new Vector3(r.NextSingle(-1f, 1f), r.NextSingle(-1f, 1f), r.NextSingle(-0f, 0f))
                    }));

                gameObjects.Last().Position.Orientation = Quaternion.CreateFromYawPitchRoll(r.NextSingle(0, MathF.PI), r.NextSingle(0, MathF.PI), r.NextSingle(0, MathF.PI));
            }

            


            //gameObjects.Add(new GameObject(new Vector3(-1, 0, 0), new CompositeBehaviour([new ContinuousRotationBehaviour(0.1f, 0.4f, 0.2f), new OrbitalCameraMouseBehaviour()]), new BoxCollider(new Vector3(1, 1.25f, 1.5f))));
            //gameObjects.Add(new GameObject(new Vector3(1, 0, 0), new ContinuousRotationBehaviour(-0.5f, 0.1f, 0.3f), new SphereCollider(0.5f)));

            _scene = new Scene(new Vector3(0, 0, -2), new Vector3(0, 0, 1), gameObjects);
            _graphicsResources = GraphicsResources.CreateResources(_graphicsDevice);
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
                    gameState.DeltaTime /= 20;
                    for (int i = 0; i < 20; i++)
                    {
                        _scene.Update(gameState);
                    }
                    Draw();
                    gameState.Input.Clear();

                    sw.Stop();
                    gameState.DeltaTime = (float)sw.Elapsed.TotalSeconds;
                    sw.Restart();
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
            input.SetMousePosition(snapshot.MousePosition);
        }

        private void Draw()
        {
            _graphicsResources.CommandList.Begin();
            _graphicsResources.CommandList.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);
            _graphicsResources.CommandList.ClearColorTarget(0, RgbaFloat.Black);
            _graphicsResources.CommandList.ClearDepthStencil(1f);
            _graphicsResources.CommandList.SetPipeline(_graphicsResources.Pipeline);
            _graphicsResources.CommandList.SetGraphicsResourceSet(0, _graphicsResources.ResourceSet);

            VeldridRenderer renderer = new VeldridRenderer(_graphicsResources, _scene.Camera);
            _scene.Draw(renderer);

            _graphicsResources.CommandList.End();

            _graphicsDevice.SubmitCommands(_graphicsResources.CommandList);
            _graphicsDevice.SwapBuffers();
        }
    }
}
