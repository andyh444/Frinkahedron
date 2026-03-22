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

        public required DirectionalShadowRenderPass ShadowRenderPass { get; init; }

        public static GraphicsResources CreateResources(GraphicsDevice graphicsDevice)
        {
            ResourceFactory factory = graphicsDevice.ResourceFactory;
            AssetManager assetManager = AssetManager.LoadAssets(factory, graphicsDevice, "Assets");

            // note mainrenderpass needs to be created before shadow render pass otherwise the textures don't get drawn
            MainRenderPass mainRenderPass = MainRenderPass.Create(factory, graphicsDevice, assetManager);
            DirectionalShadowRenderPass directionalShadowRenderPass = DirectionalShadowRenderPass.Create(factory, graphicsDevice, assetManager);
            mainRenderPass.ShadowMapTextureInfo = directionalShadowRenderPass.DepthTexture;

            return new GraphicsResources
            {
                CommandList = factory.CreateCommandList(),
                Primitives = Primitives.Create(graphicsDevice),
                AssetManager = assetManager,
                MainRenderPass = mainRenderPass,
                ShadowRenderPass = directionalShadowRenderPass
            };
        }

        public void Dispose()
        {
            CommandList.Dispose();
            Primitives.Dispose();
            AssetManager.Dispose();
            MainRenderPass.Dispose();
            ShadowRenderPass.Dispose();
        }
    }
}
