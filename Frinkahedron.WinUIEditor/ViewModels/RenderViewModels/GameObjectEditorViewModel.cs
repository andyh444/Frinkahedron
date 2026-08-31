using Frinkahedron.Core;
using Frinkahedron.Core.Template;
using Frinkahedron.VeldridImplementation;
using Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Frinkahedron.WinUIEditor.ViewModels.RenderViewModels
{
    internal class GameObjectEditorViewModel : RenderViewModelBase
    {
        private Scene? scene;
        private GameState? gameState;
        private IAssetManager? assetManager;
        private GraphicsResources? graphicsResources;
        private Vector2 size;
        private OrbitalCameraMouseBehaviour behaviour;

        public GameObjectTemplateViewModel Model { get; }

        public GameObjectEditorViewModel(GameObjectTemplateViewModel model)
        {
            Model = model;
            Model.PropertyChanged += Model_PropertyChanged;
            Model.RenderableTemplate.PropertyChanged += Model_PropertyChanged;
            Model.RenderableTemplate.TransformTemplate.PropertyChanged += Model_PropertyChanged;

            behaviour = new OrbitalCameraMouseBehaviour();
        }

        private void Model_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SetCurrentObject(Model.Model.ToGameObject(new TransformTemplate(), [behaviour], -1));
        }

        public override void Draw(GraphicsDevice graphicsDevice, Swapchain swapchain)
        {
            if (scene is null || graphicsResources is null || assetManager is null)
            {
                return;
            }
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

        public override async Task Initialise(GraphicsDevice graphicsDevice, Vector2 initialSize, Swapchain swapchain)
        {
            size = initialSize;

            SetCurrentObject(Model.Model.ToGameObject(new TransformTemplate(), [behaviour], -1));

            assetManager = await Task.Run(() => FromFolderAssetManager.LoadAssets(graphicsDevice.ResourceFactory, graphicsDevice, "C:\\Users\\Andy\\source\\repos\\Frinkahedron\\Frinkahedron.TestApp\\Assets")); // TODO Fix
            graphicsResources = GraphicsResources.CreateResources(graphicsDevice, (int)initialSize.X, (int)initialSize.Y, assetManager, swapchain);
        }

        public override void SizeChanged(GraphicsDevice graphicsDevice, Vector2 newSize, Swapchain swapchain)
        {
            if (scene is null || graphicsResources is null || assetManager is null)
            {
                return;
            }
            scene.Camera.SetAspectRatio(newSize.X / newSize.Y);
            graphicsResources.Dispose();
            graphicsResources = GraphicsResources.CreateResources(graphicsDevice, (int)newSize.X, (int)newSize.Y, assetManager, swapchain);
        }

        public override void Update(Action<Input> updateInput)
        {
            if (gameState is null || scene is null)
            {
                return;
            }
            updateInput(gameState.Input);
            scene.Update(gameState);
        }

        private void SetCurrentObject(GameObject obj)
        {
            scene = new Scene(new Vector3(), Vector3.UnitZ, size.X / size.Y, []);
            scene.SceneLights.PointLights.Add(new PointLight(new Vector3(), new Vector3(1), 100f));
            scene.SceneLights.PointLights.Add(new PointLight(new Vector3(0, 0, -75), new Vector3(1, 0, 0), 200f));
            scene.SceneLights.PointLights.Add(new PointLight(new Vector3(0, 0, 75), new Vector3(0, 1, 0), 300f));
            scene.SceneLights.DirectionalLight = new DirectionalLight(Vector3.Normalize(new Vector3(-0.5f, -1f, -0.5f)), new Vector3(1));

            scene.CollisionsEnabled = false;
            scene.AddObject(obj);

            gameState = new GameState(0.001f, scene);

            // TODO: This can be called multiple times in a single frame which is problematic as the objects don't get removed/added til the end of the frame
            /*if (currentObj is not null)
            {
                scene.RemoveObject(currentObj);
            }
            currentObj = obj;
            scene.AddObject(obj);*/
        }
    }
}
