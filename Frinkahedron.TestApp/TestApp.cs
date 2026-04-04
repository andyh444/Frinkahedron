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
        private IAssetManager _assetManager;

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
                SyncToVerticalBlank = true
            };
            _graphicsDevice = VeldridStartup.CreateGraphicsDevice(_window, options);
            _scene = CreateScene((float)_window.Width / _window.Height);
            _assetManager = FromFolderAssetManager.LoadAssets(_graphicsDevice.ResourceFactory, _graphicsDevice, "Assets");
            _graphicsResources = GraphicsResources.CreateResources(_graphicsDevice, _window.Width, _window.Height, _assetManager);

            _warmupTask.Wait();
        }

        private void Warmup()
        {
            Scene scene = CreateScene(1600f / 900f);
            GameState gameState = new GameState(0.01f, scene);
            for (int i = 0; i < 100; i++)
            {
                scene.Update(gameState);
            }
        }

        private Scene CreateScene(float aspectRatio)
        {
            SceneBuilder sb = new SceneBuilder();
            sb.AddBigBoxes();
            sb.AddBasicCar();
            sb.AddCrateTower(new Vector3(0, -14, 0));

            var scene = sb.ToScene(new Vector3(0, 0, -2), new Vector3(0, 0, 1), aspectRatio);
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
                        _scene = CreateScene((float)_window.Width / _window.Height);
                        gameState = new GameState(0.01f, _scene);
                    }
                    else
                    {
                        _scene.Update(gameState);
                        Draw();

                        sw.Stop();
                        gameState.DeltaTime = MathF.Min((float)sw.Elapsed.TotalSeconds, 0.1f);
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
