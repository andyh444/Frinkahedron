using Frinkahedron.Core;
using Veldrid;
using Veldrid.SPIRV;
using static Frinkahedron.VeldridImplementation.VeldridRenderContext;

namespace Frinkahedron.VeldridImplementation.RenderPasses
{
    public sealed class WireframeRenderPass : IRenderPass
    {
        public required Shader[] Shaders { get; init; }

        public required Pipeline Pipeline { get; init; }

        public required Framebuffer Framebuffer { get; init; }

        public required UniformBufferInfo ModelMatricesBufferInfo { get; init; }

        public required UniformBufferInfo CameraMatricesBufferInfo { get; init; }

        public static MainRenderPass Create(ResourceFactory factory, GraphicsDevice graphicsDevice, AssetManager assetManager, Framebuffer frameBuffer)
        {
            ShaderDescription vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                assetManager.GetShaderCode("WireframePass.vert"),
                "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                assetManager.GetShaderCode("WireframePass.frag"),
                "main");
            var shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            var modelBufferInfo = UniformBufferInfo.Create<ModelMatrixInfo>(factory, "ModelMatrices", ShaderStages.Vertex);
            var cameraMatrixBufferInfo = UniformBufferInfo.Create<CameraMatrixInfo>(factory, "CameraMatrices", ShaderStages.Vertex);

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

            pipelineDescription.PrimitiveTopology = PrimitiveTopology.LineList;
            //pipelineDescription.ResourceLayouts = System.Array.Empty<ResourceLayout>();

            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { MeshInfo.GetVertexLayoutDescription() },
                shaders: shaders);

            pipelineDescription.Outputs = frameBuffer.OutputDescription;
            pipelineDescription.ResourceLayouts = new[]
            {
                modelBufferInfo.ResourceLayout,
                cameraMatrixBufferInfo.ResourceLayout,
            };
            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            throw new NotImplementedException();
        }

        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene, IReadOnlyList<DrawInstruction> sceneDrawInstructions)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
