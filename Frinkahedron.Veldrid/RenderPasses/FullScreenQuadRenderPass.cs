using Frinkahedron.Core;
using System.Numerics;
using Veldrid;
using Veldrid.SPIRV;
using Vulkan;
using static Frinkahedron.VeldridImplementation.VeldridRenderContext;

namespace Frinkahedron.VeldridImplementation.RenderPasses
{
    public class FullScreenQuadRenderPass : IRenderPass
    {
        private struct QuadVertex(Vector2 position, Vector2 uv)
        {
            public Vector2 Position = position;
            public Vector2 UV = uv;
        }

        public required Shader[] Shaders { get; init; }

        public required Pipeline Pipeline { get; init; }

        public required DeviceBuffer VertexBuffer { get; init; }

        public required DeviceBuffer IndexBuffer { get; init; }

        public TextureInfo? FullScreenTexture { get; set; }

        public Swapchain? Swapchain { get; set; }

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
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.LessEqual);

            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.None,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);

            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;

            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { new VertexLayoutDescription(
                    new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                    new VertexElementDescription("TexCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2)) },
                shaders: shaders);

            pipelineDescription.Outputs = swapchain?.Framebuffer.OutputDescription ?? graphicsDevice.SwapchainFramebuffer.OutputDescription;
            pipelineDescription.ResourceLayouts = new[]
            {
                TextureInfo.GetResourceLayout(factory)
            };

            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            return new FullScreenQuadRenderPass()
            {
                IndexBuffer = indexBuffer,
                VertexBuffer = vertexBuffer,
                Pipeline = pipeline,
                Shaders = shaders,
                Swapchain = swapchain
            };
        }

        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene, IReadOnlyList<DrawInstruction> sceneDrawInstructions)
        {
            commandList.SetFramebuffer(Swapchain.Framebuffer ?? graphicsDevice.SwapchainFramebuffer);
            commandList.ClearColorTarget(0, RgbaFloat.Black);
            commandList.SetPipeline(Pipeline);
            commandList.SetVertexBuffer(0, VertexBuffer);
            commandList.SetIndexBuffer(IndexBuffer, IndexFormat.UInt16);
            commandList.SetGraphicsResourceSet(0, FullScreenTexture.ResourceSet);
            commandList.DrawIndexed(4, 1, 0, 0, 0);
        }

        public void Dispose()
        {
            IndexBuffer.Dispose();
            VertexBuffer.Dispose();
            Pipeline.Dispose();
            foreach (var shader in Shaders)
            {
                shader.Dispose();
            }
        }
    }
}
