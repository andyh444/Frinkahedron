using Frinkahedron.Core;
using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using Frinkahedron.Core.Template;
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
using System.Xml.Linq;
using Veldrid;

namespace Frinkahedron.WinformsEditor
{
    public partial class VeldridControl : UserControl
    {
        private GraphicsDevice? graphicsDevice;
        private GraphicsResources? graphicsResources;
        private Scene? scene;
        private InMemoryAssetManager? assetManager;
        private GameState? gameState;
        private GameObjectTemplate gameObjectTemplate;
        private GameObject? currentObj;
        private OrbitalCameraMouseBehaviour behaviour;
        private readonly UserControlInputListener userControlInputListener;

        public VeldridControl()
        {
            InitializeComponent();
            userControlInputListener = new UserControlInputListener(this);
            gameObjectTemplate = new GameObjectTemplate();
            behaviour = new OrbitalCameraMouseBehaviour();
        }

        public GameObjectTemplate GetGameObjectTemplate() => gameObjectTemplate;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (DesignMode)
            {
                return;
            }
            graphicsDevice = CreateGraphicsDevice();
            assetManager = new InMemoryAssetManager();
            assetManager.AddShadersFromFolder("Assets\\Shaders");
            graphicsResources = GraphicsResources.CreateResources(graphicsDevice, Width, Height, assetManager);

            scene = new Scene(new Vector3(), Vector3.UnitZ, (float)Width / Height, []);
            scene.SceneLights.DirectionalLight = new DirectionalLight(Vector3.Normalize(new Vector3(1)), new Vector3(1));
            scene.CollisionsEnabled = false;

            gameState = new GameState(0.001f, scene);
            timer1.Enabled = true;
        }

        public void LoadModel(string fileName)
        {
            Model? model = null;
            //try
            {
                model = ModelLoader.LoadModel(graphicsDevice.ResourceFactory, graphicsDevice, fileName, null);
            }
            //catch (Exception ex)
            {
            //    MessageBox.Show($"Failed to load model: {ex.Message}");
            }
            if (model is not null)
            {
                assetManager.AddModel(Path.GetFileNameWithoutExtension(fileName), model);
            }
        }

        public void GameObjectTemplateUpdated()
        {
            SetCurrentObject(gameObjectTemplate.ToGameObject(new Vector3(), [behaviour]));
        }

        private void SetCurrentObject(GameObject obj)
        {
            if (currentObj is not null)
            {
                scene.RemoveObject(currentObj);
            }
            currentObj = obj;
            scene.AddObject(obj);
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
            userControlInputListener.UpdateInput(gameState.Input);

            gameState.DeltaTime = timer1.Interval * 0.001f;
            scene.Update(gameState);
            VeldridRenderContext context = new VeldridRenderContext();
            scene.Draw(context);

            graphicsResources.CommandList.Begin();
            foreach (var renderPass in graphicsResources.RenderPasses)
            {
                renderPass.RenderScene(graphicsDevice, graphicsResources.CommandList, graphicsResources, scene, context.DrawInstructions);
            }
            graphicsResources.CommandList.End();
            graphicsDevice.SubmitCommands(graphicsResources.CommandList);
            graphicsDevice.SwapBuffers();
        }
    }
}
