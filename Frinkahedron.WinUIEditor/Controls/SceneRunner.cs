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

namespace Frinkahedron.WinUIEditor.Controls
{
    internal class SceneRunner
    {
        private Scene scene;
        private GameState gameState;
        private IAssetManager assetManager;
        private GraphicsResources graphicsResources;

        public SceneRunner(GraphicsDevice graphicsDevice, Vector2 initialSize, Swapchain swapchain)
        {
            scene = CreateScene(initialSize.X / initialSize.Y);
            gameState = new GameState(0.01f, scene);

            assetManager = FromFolderAssetManager.LoadAssets(graphicsDevice.ResourceFactory, graphicsDevice, "C:\\Users\\Andy\\source\\repos\\Frinkahedron\\Frinkahedron.TestApp\\Assets"); // TODO Fix
            graphicsResources = GraphicsResources.CreateResources(graphicsDevice, (int)initialSize.X, (int)initialSize.Y, assetManager, swapchain);
        }

        public void SizeChanged(GraphicsDevice graphicsDevice, Vector2 newSize, Swapchain swapchain)
        {
            scene.Camera.SetAspectRatio(newSize.X / newSize.Y);
            graphicsResources.Dispose();
            graphicsResources = GraphicsResources.CreateResources(graphicsDevice, (int)newSize.X, (int)newSize.Y, assetManager, swapchain);
        }

        public void Update(Action<Input> updateInput)
        {
            updateInput(gameState.Input);
            scene.Update(gameState);
        }

        public void Draw(GraphicsDevice graphicsDevice, Swapchain swapchain)
        {
            graphicsResources.CommandList.Begin();

            VeldridRenderContext context = new VeldridRenderContext();
            scene.Draw(context);
            foreach (var renderPass in graphicsResources.RenderPasses)
            {
                renderPass.RenderScene(graphicsDevice, graphicsResources.CommandList, graphicsResources, scene, context.DrawInstructions);
            }

            graphicsResources.CommandList.End();
            graphicsDevice.SubmitCommands(graphicsResources.CommandList);
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
