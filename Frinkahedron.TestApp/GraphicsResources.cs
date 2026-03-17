using System.Text;
using Veldrid;
using Frinkahedron.Core;
using System.Runtime.CompilerServices;
using Veldrid.SPIRV;

namespace Frinkahedron.TestApp
{

    internal sealed class GraphicsResources : IDisposable
    {
        public required CommandList CommandList { get; init; }
        
        public required Primitives Primitives { get; init; }

        public required AssetManager AssetManager { get; init; }

        public required MainRenderPass MainRenderPass { get; init; }

        public static GraphicsResources CreateResources(GraphicsDevice graphicsDevice)
        {
            ResourceFactory factory = graphicsDevice.ResourceFactory;
            AssetManager assetManager = AssetManager.LoadAssets(factory, graphicsDevice, "Assets");
            return new GraphicsResources
            {
                CommandList = factory.CreateCommandList(),
                Primitives = Primitives.Create(graphicsDevice),
                AssetManager = assetManager,
                MainRenderPass = MainRenderPass.Create(factory, graphicsDevice, assetManager)
            };
        }

        public void Dispose()
        {
            CommandList.Dispose();
            Primitives.Dispose();
            MainRenderPass.Dispose();
        }
    }
}
