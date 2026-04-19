using Frinkahedron.Core;
using Frinkahedron.VeldridImplementation;
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
using Veldrid;

namespace Frinkahedron.WinformsEditor.LevelEditor
{
    public partial class LevelViewerControl : VeldridControl
    {
        private GraphicsService? graphicsService;
        private GraphicsResources? graphicsResources;
        private Scene? scene;
        private GameState? gameState;
        private GameTemplateEditor? gameEditor;
        private LevelTemplateEditor? levelEditor;
        private readonly UserControlInputListener userControlInputListener;
        private LevelViewerBehaviour? behaviour;

        public LevelViewerControl()
        {
            InitializeComponent();
            userControlInputListener = new UserControlInputListener(this);
        }

        public void Initialise(GameTemplateEditor gameEditor, LevelTemplateEditor levelEditor, GraphicsService graphicsService)
        {
            if (this.graphicsService is not null)
            {
                throw new Exception($"Should only call initialise once");
            }
            base.Initialise(graphicsService);
            this.graphicsService = graphicsService;
            this.gameEditor = gameEditor;
            this.levelEditor = levelEditor;
            this.behaviour = new LevelViewerBehaviour(gameEditor, levelEditor);
            this.levelEditor.TemplateChangedCallback = UpdateScene;

            graphicsResources = GraphicsResources.CreateResources(graphicsService.GraphicsDevice, Width, Height, graphicsService.AssetManager, swapchain);

            UpdateScene();

            StartRenderLoop();
        }

        private void UpdateScene()
        {
            float aspectRatio = (float)Width / Height;

            scene = levelEditor.Template.ToScene(gameEditor.Template, scene?.Camera.Position ?? new Vector3(0, 0, -10), scene?.Camera.LookDirection ?? new Vector3(0, 0, 1), aspectRatio);
            scene.AddObject(new GameObject(Vector3.Zero, behaviour));
            gameState = new GameState(0.01f, scene);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // TODO: Also need to update the full screen quad texture size
            if (scene is not null)
            {
                scene.Camera.Projection.AspectRatio = (float)Width / Height;
            }
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

            // TODO: Remove
            context.DrawPrimitiveWireframe(Primitive.Box, System.Numerics.Matrix4x4.Identity);

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
