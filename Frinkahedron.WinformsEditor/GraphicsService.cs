using Frinkahedron.VeldridImplementation;
using Frinkahedron.WinformsEditor.GameObjectEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Frinkahedron.WinformsEditor
{
    public sealed class GraphicsService
    {
        public GraphicsDevice GraphicsDevice { get; }

        public InMemoryAssetManager AssetManager { get; }

        public GraphicsService()
        {
            var options = new GraphicsDeviceOptions
            {
                HasMainSwapchain = false,
                SyncToVerticalBlank = true,
                PreferDepthRangeZeroToOne = true,
                PreferStandardClipSpaceYDirection = true,
            };
            GraphicsDevice = GraphicsDevice.CreateD3D11(options);

            AssetManager = new InMemoryAssetManager();
            AssetManager.AddShadersFromFolder("Assets\\Shaders");
        }

        public Swapchain CreateSwapchain(Control control)
        {
            var swapChainSource = SwapchainSource.CreateWin32(control.Handle, IntPtr.Zero);
            var swapChainDescription = new SwapchainDescription
            {
                Source = swapChainSource,
                Width = (uint)control.Width,
                Height = (uint)control.Height,
                SyncToVerticalBlank = true,
            };
            return GraphicsDevice.ResourceFactory.CreateSwapchain(swapChainDescription);
        }

        public ModelInfo? LoadModel(string fileName)
        {
            Model? model = null;
            //try
            {
                model = ModelLoader.LoadModel(GraphicsDevice.ResourceFactory, GraphicsDevice, fileName, null);
            }
            //catch (Exception ex)
            {
                //    MessageBox.Show($"Failed to load model: {ex.Message}");
            }
            if (model is not null)
            {
                string modelID = new DirectoryInfo(Path.GetDirectoryName(fileName)!).Name;
                AssetManager.AddModel(modelID, model);

                return new ModelInfo(model, modelID);
            }
            return null;
        }
    }
}
