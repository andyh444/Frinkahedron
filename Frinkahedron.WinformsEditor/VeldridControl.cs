using Frinkahedron.Core;
using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using Frinkahedron.TestApp;
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

namespace Frinkahedron.WinformsEditor
{
    public partial class VeldridControl : UserControl
    {
        private GraphicsDevice? graphicsDevice;
        private GraphicsResources? graphicsResources;
        private Scene? scene;

        public VeldridControl()
        {
            InitializeComponent();
            //DoubleBuffered = true;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (DesignMode)
            {
                return;
            }
            graphicsDevice = CreateGraphicsDevice();
            graphicsResources = GraphicsResources.CreateResources(graphicsDevice, Width, Height);
            scene = CreateScene();
            timer1.Enabled = true;
        }

        private GraphicsDevice CreateGraphicsDevice()
        {
            var options = new GraphicsDeviceOptions
            {
                HasMainSwapchain = true,
                SwapchainDepthFormat = PixelFormat.D32_Float_S8_UInt,
                SyncToVerticalBlank = true,
                PreferDepthRangeZeroToOne = true,
                PreferStandardClipSpaceYDirection = true,
            };

            return GraphicsDevice.CreateD3D11(options, Handle, (uint)Width, (uint)Height);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GameState gameState = new GameState(timer1.Interval * 0.001f, scene);
            scene.Update(gameState);

            VeldridRenderContext context = new VeldridRenderContext();
            scene.Draw(context);

            graphicsResources.CommandList.Begin();
            foreach (var renderPass in graphicsResources.RenderPasses)
            {
                renderPass.RenderScene(graphicsDevice, graphicsResources.CommandList, graphicsResources, scene, context.DrawInstructions);
            }
            graphicsDevice.SwapBuffers();
            graphicsResources.CommandList.End();
            graphicsDevice.SubmitCommands(graphicsResources.CommandList);
        }

        private Scene CreateScene()
        {
            SceneBuilder sb = new SceneBuilder();
            sb.AddBigBoxes();
            sb.AddBasicCar();
            sb.AddCrateTower(new Vector3(0, -14, 0));

            var scene = sb.ToScene(new Vector3(0, 0, -2), new Vector3(0, 0, 1), (float)Width / Height);
            scene.SceneLights.PointLights.Add(new PointLight(new Vector3(), new Vector3(1), 100f));
            scene.SceneLights.PointLights.Add(new PointLight(new Vector3(0, 0, -75), new Vector3(1, 0, 0), 200f));
            scene.SceneLights.PointLights.Add(new PointLight(new Vector3(0, 0, 75), new Vector3(0, 1, 0), 300f));
            scene.SceneLights.DirectionalLight = new DirectionalLight(Vector3.Normalize(new Vector3(-0.5f, -1f, -0.5f)), new Vector3(1));

            return scene;
        }
    }
}
