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
    public partial class GameObjectViewerControl : UserControl
    {
        private GraphicsDevice? graphicsDevice;
        private GraphicsResources? graphicsResources;
        private Scene? scene;
        private InMemoryAssetManager? assetManager;
        private GameState? gameState;
        private GameObject? currentObj;
        private OrbitalCameraMouseBehaviour behaviour;
        private GameObjectTemplateEditor? editor;
        private readonly UserControlInputListener userControlInputListener;

        public GameObjectViewerControl()
        {
            InitializeComponent();
            userControlInputListener = new UserControlInputListener(this);
            behaviour = new OrbitalCameraMouseBehaviour();
        }

        public void Initialise(GameObjectTemplateEditor editor)
        {
            this.editor = editor;
            editor.TemplateChangedCallback = GameObjectTemplateUpdated;
            editor.LoadModelFunc = LoadModel;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (DesignMode)
            {
                return;
            }
            try
            {
                graphicsDevice = CreateGraphicsDevice();
                assetManager = new InMemoryAssetManager();
                assetManager.AddShadersFromFolder("Assets\\Shaders");
                graphicsResources = GraphicsResources.CreateResources(graphicsDevice, Width, Height, assetManager);

                timer1.Enabled = true;
            }
            catch (Exception ex)
            {
                // do nothing - do this to stop the designer having a tantrum
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            graphicsDevice?.ResizeMainWindow((uint)Width, (uint)Height);
            if (scene is not null)
            {
                scene.Camera.Projection.AspectRatio = (float)Width / Height;
            }
        }

        public ModelInfo? LoadModel(string fileName)
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
                string modelID = new DirectoryInfo(Path.GetDirectoryName(fileName)!).Name;
                assetManager.AddModel(modelID, model);

                return new ModelInfo(model, modelID);
            }
            return null;
        }

        private void GameObjectTemplateUpdated()
        {
            SetCurrentObject(editor.Template.ToGameObject(new Vector3(), [behaviour]));
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
            if (scene is null || gameState is null || DesignMode)
            {
                return;
            }
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
