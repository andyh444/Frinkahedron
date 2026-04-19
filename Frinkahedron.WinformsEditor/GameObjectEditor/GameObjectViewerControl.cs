using Frinkahedron.Core;
using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using Frinkahedron.Core.Template;
using Frinkahedron.TestApp;
using Frinkahedron.VeldridImplementation;
using Frinkahedron.WinformsEditor.GameObjectEditor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Veldrid;

namespace Frinkahedron.WinformsEditor.GameObjectEditor
{
    public partial class GameObjectViewerControl : VeldridControl
    {
        private GraphicsService? graphicsService;
        private GraphicsResources? graphicsResources;
        private Scene? scene;
        private GameState? gameState;
        private GameObject? currentObj;
        private OrbitalCameraMouseBehaviour behaviour;
        private GameObjectTemplateEditor? objectEditor;
        private GameTemplateEditor? gameEditor;
        private readonly UserControlInputListener userControlInputListener;

        public GameObjectViewerControl()
        {
            InitializeComponent();
            userControlInputListener = new UserControlInputListener(this);
            behaviour = new OrbitalCameraMouseBehaviour();
        }

        public void Initialise(GameTemplateEditor gameEditor, GameObjectTemplateEditor objectEditor, GraphicsService graphicsService)
        {
            if (this.objectEditor is not null)
            {
                throw new Exception($"Should only initialise once");
            }
            base.Initialise(graphicsService);
            this.graphicsService = graphicsService;
            this.gameEditor = gameEditor;
            this.objectEditor = objectEditor;
            objectEditor.TemplateChangedCallback = GameObjectTemplateUpdated;
            objectEditor.LoadModelFunc = GetModel;

            graphicsResources = GraphicsResources.CreateResources(graphicsService.GraphicsDevice, Width, Height, graphicsService.AssetManager, swapchain);

            StartRenderLoop();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (scene is not null)
            {
                scene.Camera.Projection.AspectRatio = (float)Width / Height;
            }
        }

        public ModelInfo? GetModel(string modelID)
        {
            if (graphicsService.AssetManager.HasModel(modelID))
            {
                return new ModelInfo(graphicsService.AssetManager.GetModel(modelID), modelID);
            }
            return graphicsService.LoadModel(gameEditor.Template.Models.Single(x => x.ModelID == modelID).ModelPath);
        }

        private void GameObjectTemplateUpdated()
        {
            var template = objectEditor?.Template;
            if (template is null)
            {
                return;
            }
            if (template.Renderable is ModelEntitiesRenderableTemplate mert)
            {
                // ensures it's loaded
                _ = GetModel(mert.ModelID);
            }

            SetCurrentObject(template.ToGameObject(new TransformTemplate(), [behaviour]));
        }

        private void SetCurrentObject(GameObject obj)
        {
            scene = new Scene(new Vector3(), Vector3.UnitZ, (float)Width / Height, []);
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

        protected override void Render(Swapchain swapChain, TimeSpan interval)
        {
            if (scene is null || gameState is null || DesignMode)
            {
                return;
            }
            userControlInputListener.UpdateInput(gameState.Input);

            gameState.DeltaTime = (float)interval.TotalSeconds;
            scene.Update(gameState);
            VeldridRenderContext context = new VeldridRenderContext();
            scene.Draw(context);

            graphicsResources.CommandList.Begin();
            foreach (var renderPass in graphicsResources.RenderPasses)
            {
                renderPass.RenderScene(graphicsService.GraphicsDevice, graphicsResources.CommandList, graphicsResources, scene, context.DrawInstructions);
            }
            graphicsResources.CommandList.End();
            graphicsService.GraphicsDevice.SubmitCommands(graphicsResources.CommandList);
            graphicsService.GraphicsDevice.SwapBuffers(swapchain);
        }
    }
}
