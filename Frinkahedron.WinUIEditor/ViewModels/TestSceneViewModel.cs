using Frinkahedron.Core;
using Frinkahedron.Core.Template;
using Frinkahedron.TestApp;
using Frinkahedron.VeldridImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Veldrid;
using Windows.Security.Cryptography.Certificates;

namespace Frinkahedron.WinUIEditor.ViewModels
{
    internal class TestSceneViewModel : RenderViewModel
    {
        private record State(Scene Scene, GameState GameState, IAssetManager AssetManager, GraphicsResources GraphicsResources)
        {
            public State WithNewResources(GraphicsResources resources) => new State(Scene, GameState, AssetManager, resources);
        }

        private State? state;

        public TestSceneViewModel()
        {
        }

        public override void Initialise(GraphicsDevice graphicsDevice, Vector2 initialSize, Swapchain swapchain)
        {
            if (state is not null)
            {
                throw new InvalidOperationException($"Should only call initialise once");
            }
            var scene = CreateScene(initialSize.X / initialSize.Y);
            var gameState = new GameState(0.01f, scene);
            var assetManager = FromFolderAssetManager.LoadAssets(graphicsDevice.ResourceFactory, graphicsDevice, "C:\\Users\\Andy\\source\\repos\\Frinkahedron\\Frinkahedron.TestApp\\Assets"); // TODO Fix
            var graphicsResources = GraphicsResources.CreateResources(graphicsDevice, (int)initialSize.X, (int)initialSize.Y, assetManager, swapchain);

            state = new State(scene, gameState, assetManager, graphicsResources);
        }

        public override void SizeChanged(GraphicsDevice graphicsDevice, Vector2 newSize, Swapchain swapchain)
        {
            if (state is null)
            {
                return;
            }
            state.Scene.Camera.SetAspectRatio(newSize.X / newSize.Y);
            state.GraphicsResources.Dispose();
            state = state.WithNewResources(GraphicsResources.CreateResources(graphicsDevice, (int)newSize.X, (int)newSize.Y, state.AssetManager, swapchain));
        }

        public override void Update(Action<Input> updateInput)
        {
            if (state is null)
            {
                return;
            }
            updateInput(state.GameState.Input);
            state.Scene.Update(state.GameState);
        }

        public override void Draw(GraphicsDevice graphicsDevice, Swapchain swapchain)
        {
            if (state is null)
            {
                return;
            }
            state.GraphicsResources.CommandList.Begin();

            VeldridRenderContext context = new VeldridRenderContext();
            state.Scene.Draw(context);
            foreach (var renderPass in state.GraphicsResources.RenderPasses)
            {
                renderPass.RenderScene(graphicsDevice, state.GraphicsResources.CommandList, state.GraphicsResources, state.Scene, context.DrawInstructions);
            }

            state.GraphicsResources.CommandList.End();
            graphicsDevice.SubmitCommands(state.GraphicsResources.CommandList);
            graphicsDevice.SwapBuffers(swapchain);
        }

        private Scene CreateScene(float aspectRatio)
        {
            if (File.Exists($@"C:\tmp\tempgame.json"))
            {
                using var fs = File.OpenRead($@"C:\tmp\tempgame.json");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true, };
                options.Converters.Add(new Vector3Converter());
                var template = JsonSerializer.Deserialize<GameTemplate>(fs, options);

                return template.Levels[0].ToScene(template, new Vector3(0, 0, -2), new Vector3(0, 0, 1), aspectRatio);
            }
            else
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
        }
    }
}
