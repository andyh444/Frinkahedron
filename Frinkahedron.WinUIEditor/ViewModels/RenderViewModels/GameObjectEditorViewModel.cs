using Frinkahedron.Core;
using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Physics;
using Frinkahedron.Core.Template;
using Frinkahedron.VeldridImplementation;
using Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels;
using Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Windows.Devices.Radios;

namespace Frinkahedron.WinUIEditor.ViewModels.RenderViewModels
{
    internal class GameObjectEditorViewModel : RenderViewModelBase
    {
        private class DimensionsGizmoBehaviour : Behaviour
        {
            private bool mouseOver;
            private bool mouseDragged;

            public GameObjectTemplateViewModel? ViewModel { get; set; }

            public GameObject? EditableObject { get; set; }

            public override void Update(GameObject self, GameState gameState)
            {
                base.Update(self, gameState);
                mouseOver = false;

                if (EditableObject is null || ViewModel is null)
                {
                    return;
                }

                if (ViewModel.Shape is null || !ViewModel.Shape.GetGizmos().Any())
                {
                    return;
                }

                var gizmo = ViewModel.Shape.GetGizmos().First();
                mouseOver = gizmo.IsMouseOver(EditableObject, gameState);

                if (gameState.Input.IsMouseButtonDown(MouseButton.Left))
                {
                    if (mouseOver)
                    {
                        mouseDragged = true;
                    }
                }
                else if (mouseDragged)
                {
                    mouseDragged = false;
                }

                if (mouseDragged)
                {
                    gizmo.OnDragged(EditableObject, gameState);
                }
            }

            public override void Draw(GameObject self, IRenderContext renderer)
            {
                base.Draw(self, renderer);

                if (EditableObject is null || ViewModel is null)
                {
                    return;
                }

                if (ViewModel.Shape is null)
                {
                    return;
                }

                foreach (var gizmo in ViewModel.Shape.GetGizmos())
                {
                    gizmo.Draw(mouseOver, mouseDragged, EditableObject, renderer);
                }
            }
        }


        private Scene? scene;
        private GameState? gameState;
        private IAssetManager? assetManager;
        private GraphicsResources? graphicsResources;
        private Vector2 size;
        private OrbitalCameraMouseBehaviour camBehaviour;
        private DimensionsGizmoBehaviour gizmoBehaviour;
        private object renderLock;
        private object updateLock;

        public GameObjectTemplateViewModel Model { get; }

        public GameObjectEditorViewModel(GameObjectTemplateViewModel model)
        {
            Model = model;
            Model.ObjectChanged += Model_PropertyChanged;

            camBehaviour = new OrbitalCameraMouseBehaviour();
            gizmoBehaviour = new DimensionsGizmoBehaviour();

            renderLock = new object();
            updateLock = new object();
        }

        private void Model_PropertyChanged()
        {
            SetCurrentObject(Model.Model.ToGameObject(new TransformTemplate(), [camBehaviour], -1));
        }

        public override void Draw(GraphicsDevice graphicsDevice, Swapchain swapchain)
        {
            if (scene is null || graphicsResources is null || assetManager is null)
            {
                return;
            }
            lock (renderLock)
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
        }

        public override async Task Initialise(GraphicsDevice graphicsDevice, Vector2 initialSize, Swapchain swapchain)
        {
            size = initialSize;

            SetCurrentObject(Model.Model.ToGameObject(new TransformTemplate(), [camBehaviour], -1));

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
            lock (updateLock)
            {
                updateInput(gameState.Input);
                scene.Update(gameState);
            }
        }

        private void SetCurrentObject(GameObject obj)
        {
            lock (renderLock)
            {
                lock (updateLock)
                {
                    scene = new Scene(new Vector3(), Vector3.UnitZ, size.X / size.Y, []);
                    scene.SceneLights.PointLights.Add(new PointLight(new Vector3(), new Vector3(1), 100f));
                    scene.SceneLights.PointLights.Add(new PointLight(new Vector3(0, 0, -75), new Vector3(1, 0, 0), 200f));
                    scene.SceneLights.PointLights.Add(new PointLight(new Vector3(0, 0, 75), new Vector3(0, 1, 0), 300f));
                    scene.SceneLights.DirectionalLight = new DirectionalLight(Vector3.Normalize(new Vector3(-0.5f, -1f, -0.5f)), new Vector3(1));

                    scene.CollisionsEnabled = false;
                    scene.AddObject(obj);

                    gizmoBehaviour.ViewModel = Model;
                    gizmoBehaviour.EditableObject = obj;
                    scene.AddObject(new GameObject(new Vector3(), gizmoBehaviour));

                    gameState = new GameState(0.01f, scene, gameState?.Input ?? new Input());
                }
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
}
