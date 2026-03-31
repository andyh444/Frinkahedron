using System.Text;
using Veldrid;
using Frinkahedron.Core;
using System.Runtime.CompilerServices;
using Veldrid.SPIRV;
using Frinkahedron.VeldridImplementation.RenderPasses;

namespace Frinkahedron.VeldridImplementation
{

    public sealed class GraphicsResources : IDisposable
    {
        public required CommandList CommandList { get; init; }
        
        public required Primitives Primitives { get; init; }

        public required AssetManager AssetManager { get; init; }

        public required MainRenderPass MainRenderPass { get; init; }

        public required DirectionalShadowRenderPass ShadowRenderPass { get; init; }

        public required FullScreenQuadRenderPass QuadRenderPass { get; init; }

        public static GraphicsResources CreateResources(GraphicsDevice graphicsDevice, int screenWidth, int screenHeight)
        {
            ResourceFactory factory = graphicsDevice.ResourceFactory;
            AssetManager assetManager = AssetManager.LoadAssets(factory, graphicsDevice, "Assets");

            // note mainrenderpass needs to be created before shadow render pass otherwise the textures don't get drawn
            TextureDescription colourDescription = TextureDescription.Texture2D(
                (uint)screenWidth,
                (uint)screenHeight,
                1,
                1,
                PixelFormat.R32_G32_B32_A32_Float,
                TextureUsage.RenderTarget | TextureUsage.Sampled);
            var colourTexture = TextureInfo.Create(factory, graphicsDevice, colourDescription);

            TextureDescription depthDescription = TextureDescription.Texture2D(
                colourTexture.Texture.Width,
                colourTexture.Texture.Height,
                1,
                1,
                PixelFormat.D32_Float_S8_UInt,
                TextureUsage.DepthStencil | TextureUsage.Sampled);
            var depthTexture = TextureInfo.Create(factory, graphicsDevice, depthDescription);

            var mainFrameBuffer = factory.CreateFramebuffer(
                new FramebufferDescription(
                    colorTargets: [new FramebufferAttachmentDescription(colourTexture.Texture, 0)],
                    depthTarget: new FramebufferAttachmentDescription(depthTexture.Texture, 0)
                    ));

            MainRenderPass mainRenderPass = MainRenderPass.Create(factory, graphicsDevice, assetManager, mainFrameBuffer);
            DirectionalShadowRenderPass directionalShadowRenderPass = DirectionalShadowRenderPass.Create(factory, graphicsDevice, assetManager);
            mainRenderPass.ShadowMapTextureInfo = directionalShadowRenderPass.DepthTexture;

            FullScreenQuadRenderPass quadRenderPass = FullScreenQuadRenderPass.Create(factory, graphicsDevice, assetManager);
            quadRenderPass.FullScreenTexture = colourTexture;

            return new GraphicsResources
            {
                CommandList = factory.CreateCommandList(),
                Primitives = Primitives.Create(graphicsDevice),
                AssetManager = assetManager,
                MainRenderPass = mainRenderPass,
                ShadowRenderPass = directionalShadowRenderPass,
                QuadRenderPass = quadRenderPass
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
