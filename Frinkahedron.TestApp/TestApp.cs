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
            _scene = new Scene(new Vector3(0, 0, -2), new Vector3(0, 0, 1),
                [
                    new GameObject(new Vector3(-1, 0, 0), new CompositeBehaviour([new ContinuousRotationBehaviour(0.1f, 0.4f, 0.2f), new OrbitalCameraMouseBehaviour()])),
                    new GameObject(new Vector3(1, 0, 0), new ContinuousRotationBehaviour(-0.5f, 0.1f, 0.3f))]
                );
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
                    _scene.Update(gameState);
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
