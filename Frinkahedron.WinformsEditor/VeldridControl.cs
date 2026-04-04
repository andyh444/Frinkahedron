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
        private GameObject? currentObj;
        private Matrix4x4 currentTransform;
        private IShape? currentShape;
        private readonly UserControlInputListener userControlInputListener;

        public VeldridControl()
        {
            InitializeComponent();
            userControlInputListener = new UserControlInputListener(this);
        }

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
            currentTransform = Matrix4x4.Identity;
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
                SetModel(Path.GetFileNameWithoutExtension(fileName), model);
            }
        }

        public void UpdateTransform(Matrix4x4 transform)
        {
            currentTransform = transform;
            if (currentObj is not null)
            {
                GameObject obj = new GameObject(new Vector3(),
                    behaviour: currentObj.Behaviour,
                    colliderShape: currentObj.Collider,
                    rigidBody: null,
                    renderable: new ModelRenderable((currentObj.Renderable as ModelRenderable).ModelID, currentTransform));

                SetObject(obj);
            }
        }

        public void UpdateCollider(IShape shape)
        {
            currentShape = shape;
            if (currentObj is not null)
            {
                GameObject obj = new GameObject(new Vector3(),
                    behaviour: currentObj.Behaviour,
                    colliderShape: currentShape,
                    rigidBody: null,
                    renderable: currentObj.Renderable);

                SetObject(obj);
            }
        }

        public void SetModel(string name, Model model)
        {
            assetManager.AddModel(name, model);
            GameObject obj = new GameObject(new Vector3(),
                behaviour: new OrbitalCameraMouseBehaviour(),
                colliderShape: currentShape,
                rigidBody: null,
                renderable: new ModelRenderable(name, currentTransform));
            SetObject(obj);
        }

        public void SetObject(GameObject obj)
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
            graphicsDevice.SwapBuffers();
            graphicsResources.CommandList.End();
            graphicsDevice.SubmitCommands(graphicsResources.CommandList);
        }
    }
}
