using System.Text;
using Veldrid;
using Frinkahedron.Core;
using System.Runtime.CompilerServices;
using Veldrid.SPIRV;
using Frinkahedron.VeldridImplementation.RenderPasses;
using System.Numerics;

namespace Frinkahedron.VeldridImplementation
{

    public sealed class GraphicsResources : IDisposable
    {
        public required CommandList CommandList { get; init; }
        
        public required Primitives Primitives { get; init; }

        public required IAssetManager AssetManager { get; init; }

        public required MainRenderPass MainRenderPass { get; init; }

        public required DirectionalShadowRenderPass ShadowRenderPass { get; init; }

        public required FullScreenQuadRenderPass QuadRenderPass { get; init; }

        public required WireframeRenderPass WireframeRenderPass { get; init; }

        public required ObjectHighlightRenderPass ObjectHighlightRenderPass { get; init; }

        public IEnumerable<IRenderPass> RenderPasses => [ShadowRenderPass, MainRenderPass, WireframeRenderPass, ObjectHighlightRenderPass, QuadRenderPass];

        public static GraphicsResources CreateResources(GraphicsDevice graphicsDevice, int screenWidth, int screenHeight, IAssetManager assetManager, Swapchain? swapchain = null)
        {
            // TODO Check if this is still true
            // note mainrenderpass needs to be created before shadow render pass otherwise the textures don't get drawn

            ResourceFactory factory = graphicsDevice.ResourceFactory;
            var colourTexture = TextureInfo.Create(factory, graphicsDevice, CreateColourTargetDescription(screenWidth, screenHeight));
            var depthTexture = TextureInfo.Create(factory, graphicsDevice, CreateDepthTargetDescription(screenWidth, screenHeight));
            Framebuffer mainFrameBuffer = CreateFrameBuffer(factory, colourTexture, depthTexture);



            MainRenderPass mainRenderPass = MainRenderPass.Create(factory, graphicsDevice, assetManager, mainFrameBuffer);
            DirectionalShadowRenderPass directionalShadowRenderPass = DirectionalShadowRenderPass.Create(factory, graphicsDevice, assetManager);
            mainRenderPass.ShadowMapTextureInfo = directionalShadowRenderPass.DepthTexture;

            WireframeRenderPass wireframeRenderPass = WireframeRenderPass.Create(factory, graphicsDevice, assetManager, mainFrameBuffer);

            var highlightColourTexture = TextureInfo.Create(factory, graphicsDevice, CreateColourTargetDescription(screenWidth, screenHeight));
            var highlightDepthTexture = TextureInfo.Create(factory, graphicsDevice, CreateDepthTargetDescription(screenWidth, screenHeight));
            Framebuffer highlightFrameBuffer = CreateFrameBuffer(factory, highlightColourTexture, highlightDepthTexture);


            ObjectHighlightRenderPass highlightRenderPass = ObjectHighlightRenderPass.Create(factory, graphicsDevice, assetManager, highlightFrameBuffer);

            FullScreenQuadRenderPass quadRenderPass = FullScreenQuadRenderPass.Create(factory, graphicsDevice, assetManager, swapchain);
            quadRenderPass.Textures.Add((colourTexture, Vector4.One));
            quadRenderPass.Textures.Add((highlightColourTexture, Vector4.One));


            return new GraphicsResources
            {
                CommandList = factory.CreateCommandList(),
                Primitives = Primitives.Create(graphicsDevice),
                AssetManager = assetManager,
                MainRenderPass = mainRenderPass,
                ShadowRenderPass = directionalShadowRenderPass,
                WireframeRenderPass = wireframeRenderPass,
                ObjectHighlightRenderPass = highlightRenderPass,
                QuadRenderPass = quadRenderPass
            };
        }

        private static Framebuffer CreateFrameBuffer(ResourceFactory factory, TextureInfo colourTexture, TextureInfo depthTexture)
        {
            return factory.CreateFramebuffer(
                new FramebufferDescription(
                    colorTargets: [new FramebufferAttachmentDescription(colourTexture.Texture, 0)],
                    depthTarget: new FramebufferAttachmentDescription(depthTexture.Texture, 0)
                    ));
        }

        private static TextureDescription CreateDepthTargetDescription(int screenWidth, int screenHeight)
        {
            return TextureDescription.Texture2D(
                            (uint)screenWidth,
                            (uint)screenHeight,
                            1,
                            1,
                            PixelFormat.D32_Float_S8_UInt,
                            TextureUsage.DepthStencil | TextureUsage.Sampled);
        }

        private static TextureDescription CreateColourTargetDescription(int screenWidth, int screenHeight)
        {
            return TextureDescription.Texture2D(
                (uint)screenWidth,
                (uint)screenHeight,
                1,
                1,
                PixelFormat.R32_G32_B32_A32_Float,
                TextureUsage.RenderTarget | TextureUsage.Sampled);
        }

        public void Dispose()
        {
            CommandList.Dispose();
            Primitives.Dispose();
            MainRenderPass.Dispose();
            ShadowRenderPass.Dispose();
        }
    }
}
