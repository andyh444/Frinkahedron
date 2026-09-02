using Frinkahedron.Core;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;
using Veldrid.SPIRV;
using Vulkan;
using static Frinkahedron.VeldridImplementation.VeldridRenderContext;

namespace Frinkahedron.VeldridImplementation.RenderPasses
{
    public class FullScreenQuadRenderPass : IRenderPass
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PostProcessSettings
        {
            public int enableFXAA;
            public int enableGaussian;
            public int gaussianRadius;
            public float gaussianSigma;

            public int enableMean;
            public int meanRadius;
            public int enablePixelation;
            public int pixelationRadius;

            public int enableSepia;
            public float sepiaStrength;

            public int enableGreyscale;
            // padding to align to 16 bytes for std140 compatibility
            public int _pad0;
            //public int _pad1;
            //public int _pad2;
        }

        private struct QuadVertex(Vector2 position, Vector2 uv)
        {
            public Vector2 Position = position;
            public Vector2 UV = uv;
        }

        public required Shader[] Shaders { get; init; }

        public required Pipeline Pipeline { get; init; }

        public required DeviceBuffer VertexBuffer { get; init; }

        public required DeviceBuffer IndexBuffer { get; init; }

        public List<(TextureInfo texture, PostProcessSettings settings)> Textures { get; } = new List<(TextureInfo texture, PostProcessSettings settings)>();

        public Swapchain? Swapchain { get; set; }

        public required UniformBufferInfo PostProcessSettingsBufferInfo { get; init; }

        public static FullScreenQuadRenderPass Create(ResourceFactory factory, GraphicsDevice graphicsDevice, IAssetManager assetManager, Swapchain? swapchain)
        {
            ShaderDescription vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                assetManager.GetShaderCode("ScreenQuadPass.vert"),
                "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                assetManager.GetShaderCode("ScreenQuadPass.frag"),
                "main");
            var shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            QuadVertex[] vertices = new[]
            {
                new QuadVertex(new Vector2(-1, 1), new Vector2(0, 0)),
                new QuadVertex(new Vector2(1, 1), new Vector2(1, 0)),
                new QuadVertex(new Vector2(-1, -1), new Vector2(0, 1)),
                new QuadVertex(new Vector2(1, -1), new Vector2(1, 1))
            };
            ushort[] indices = [0, 1, 2, 3];

            var vertexBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.VertexBuffer));
            var indexBuffer = factory.CreateBuffer(new BufferDescription(8, BufferUsage.IndexBuffer));
            graphicsDevice.UpdateBuffer(vertexBuffer, 0, vertices);
            graphicsDevice.UpdateBuffer(indexBuffer, 0, indices);

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleAdditiveBlend;

            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: false,
                depthWriteEnabled: false,
                comparisonKind: ComparisonKind.LessEqual);

            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.None,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: false,
                scissorTestEnabled: false);

            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;

            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { new VertexLayoutDescription(
                    new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                    new VertexElementDescription("TexCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2)) },
                shaders: shaders);

            pipelineDescription.Outputs = swapchain?.Framebuffer.OutputDescription ?? graphicsDevice.SwapchainFramebuffer.OutputDescription;
            // Create the post-process uniform buffer info (will be set as set=1)
            var postProcessInfo = UniformBufferInfo.Create<PostProcessSettings>(factory, "PostProcessSettings", ShaderStages.Fragment);

            // Resource layouts: set=0 is texture layout (texture + sampler), set=1 is the post-process uniform buffer
            pipelineDescription.ResourceLayouts = new[]
            {
                TextureInfo.GetResourceLayout(factory),
                postProcessInfo.ResourceLayout
            };

            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            return new FullScreenQuadRenderPass()
            {
                IndexBuffer = indexBuffer,
                VertexBuffer = vertexBuffer,
                Pipeline = pipeline,
                Shaders = shaders,
                Swapchain = swapchain,
                PostProcessSettingsBufferInfo = postProcessInfo
            };
        }

        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene, IReadOnlyList<DrawInstruction> sceneDrawInstructions)
        {
            commandList.SetFramebuffer(Swapchain?.Framebuffer ?? graphicsDevice.SwapchainFramebuffer);
            commandList.ClearColorTarget(0, RgbaFloat.Black);
            commandList.SetPipeline(Pipeline);
            commandList.SetVertexBuffer(0, VertexBuffer);
            commandList.SetIndexBuffer(IndexBuffer, IndexFormat.UInt16);
            foreach ((var texture, var settings) in Textures)
            {
                // set 0: texture resource set (contains texture + sampler)
                commandList.SetGraphicsResourceSet(0, texture.ResourceSet);
                // set 1: post-process settings uniform buffer
                commandList.SetGraphicsResourceSet(1, PostProcessSettingsBufferInfo.ResourceSet);
                PostProcessSettings thisSettings = settings;
                commandList.UpdateBuffer(PostProcessSettingsBufferInfo.DeviceBuffer, 0, ref thisSettings);
                commandList.DrawIndexed(4, 1, 0, 0, 0);
            }
        }

        public void Dispose()
        {
            IndexBuffer.Dispose();
            VertexBuffer.Dispose();
            Pipeline.Dispose();
            PostProcessSettingsBufferInfo.Dispose();
            foreach (var shader in Shaders)
            {
                shader.Dispose();
            }
        }
    }
}
