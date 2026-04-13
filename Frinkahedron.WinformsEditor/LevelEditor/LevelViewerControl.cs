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
    public partial class LevelViewerControl : UserControl
    {
        private GraphicsService? graphicsService;
        private Swapchain? swapchain;
        private GraphicsResources? graphicsResources;
        private Scene? scene;
        private GameState? gameState;
        private GameTemplateEditor? gameEditor;
        private LevelTemplateEditor? levelEditor;
        private readonly UserControlInputListener userControlInputListener;

        public LevelViewerControl()
        {
            InitializeComponent();
            userControlInputListener = new UserControlInputListener(this);
        }

        public void Initialise(GameTemplateEditor gameEditor, LevelTemplateEditor levelEditor, GraphicsService graphicsService)
        {
            this.graphicsService = graphicsService;
            this.gameEditor = gameEditor;
            this.levelEditor = levelEditor;

            swapchain = graphicsService.CreateSwapchain(this);
            graphicsResources = GraphicsResources.CreateResources(graphicsService.GraphicsDevice, Width, Height, graphicsService.AssetManager, swapchain);

            scene = new Scene(new Vector3(0, 0, -10), new Vector3(0, 0, 1), (float)Width / Height, []);
            gameState = new GameState(0.01f, scene);

            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (scene is null || gameState is null || DesignMode)
            {
                return;
            }
            userControlInputListener.UpdateInput(gameState.Input);

            gameState.DeltaTime = timer1.Interval * 0.001f;
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

            // TODO: remove
            var newLook = Vector3.Transform(scene.Camera.LookDirection, Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0.01f));
            scene.Camera.SetValues(-10 * newLook, newLook);
        }
    }
}
