using Frinkahedron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.SPIRV;

namespace Frinkahedron.TestApp
{
    internal interface IRenderPass : IDisposable
    {
        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene);
    }

    internal class MainRenderPass : IRenderPass
    {
        public required Shader[] Shaders { get; init; }
        public required Pipeline Pipeline { get; init; }
        public required DeviceBuffer MatricesBuffer { get; init; }
        public required ResourceLayout ResourceLayout { get; init; }
        public required ResourceSet ResourceSet { get; init; }

        public static MainRenderPass Create(ResourceFactory factory, GraphicsDevice graphicsDevice)
        {
            ShaderDescription vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                Encoding.UTF8.GetBytes(Frinkahedron.TestApp.Shaders.PositionNormalUvVertexShader),
                "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                Encoding.UTF8.GetBytes(Frinkahedron.TestApp.Shaders.TextureNormalFragmentShader),
                "main");
            var shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            // Create uniform buffer
            var uniformBuffer = factory.CreateBuffer(new BufferDescription(
                (uint)Unsafe.SizeOf<MatrixUniforms>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            // Create resource layout for the uniform buffer
            var resourceLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Matrices", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            // Create resource set for the uniform buffer
            var resourceSet = factory.CreateResourceSet(new ResourceSetDescription(resourceLayout, uniformBuffer));

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.LessEqual);

            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);

            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleList;
            //pipelineDescription.ResourceLayouts = System.Array.Empty<ResourceLayout>();

            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { MeshInfo.GetVertexLayoutDescription() },
                shaders: shaders);

            pipelineDescription.Outputs = graphicsDevice.SwapchainFramebuffer.OutputDescription;
            pipelineDescription.ResourceLayouts = new[] { resourceLayout, TextureInfo.GetResourceLayout(factory) };
            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            return new MainRenderPass
            {
                Shaders = shaders,
                Pipeline = pipeline,
                MatricesBuffer = uniformBuffer,
                ResourceLayout = resourceLayout,
                ResourceSet = resourceSet
            };
        }

        public void Dispose()
        {
            foreach (var shader in Shaders)
            {
                shader.Dispose();
            }
            Pipeline.Dispose();
            MatricesBuffer.Dispose();
            ResourceLayout.Dispose();
            ResourceSet.Dispose();
        }

        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene)
        {
            commandList.Begin();
            commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
            commandList.ClearColorTarget(0, RgbaFloat.Black);
            commandList.ClearDepthStencil(1f);
            commandList.SetPipeline(Pipeline);
            commandList.SetGraphicsResourceSet(0, ResourceSet);

            VeldridRenderer renderer = new VeldridRenderer(graphicsResources.Primitives, MatricesBuffer, commandList, graphicsResources.AssetManager, scene.Camera);
            scene.Draw(renderer);

            commandList.End();

            graphicsDevice.SubmitCommands(commandList);
        }
    }
}
