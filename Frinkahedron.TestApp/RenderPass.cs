using Frinkahedron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        public required ResourceLayout MatricesResourceLayout { get; init; }
        public required ResourceSet MatricesResourceSet { get; init; }
        public required DeviceBuffer PointLightsBuffer { get; init; }
        public required ResourceLayout PointLightsResourceLayout { get; init; }
        public required ResourceSet PointLightsResourceSet { get; init; }

        public static MainRenderPass Create(ResourceFactory factory, GraphicsDevice graphicsDevice, AssetManager assetManager)
        {
            ShaderDescription vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                assetManager.GetShaderCode("MainRenderPass.vert"),
                "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                assetManager.GetShaderCode("MainRenderPass.frag"),
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

            var pointLightsResourceLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("PointLights", ResourceKind.UniformBuffer, ShaderStages.Fragment)));
            var pointLightsUniformBuffer = factory.CreateBuffer(new BufferDescription(
                (uint)Unsafe.SizeOf<PointLightsInfo>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            var pointLightsResourceSet = factory.CreateResourceSet(new ResourceSetDescription(pointLightsResourceLayout, pointLightsUniformBuffer));

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
            pipelineDescription.ResourceLayouts = new[] { resourceLayout, TextureInfo.GetResourceLayout(factory), pointLightsResourceLayout };
            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            return new MainRenderPass
            {
                Shaders = shaders,
                Pipeline = pipeline,
                MatricesBuffer = uniformBuffer,
                MatricesResourceLayout = resourceLayout,
                MatricesResourceSet = resourceSet,
                PointLightsBuffer = pointLightsUniformBuffer,
                PointLightsResourceLayout = pointLightsResourceLayout,
                PointLightsResourceSet = pointLightsResourceSet,
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
            MatricesResourceLayout.Dispose();
            MatricesResourceSet.Dispose();
        }

        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene)
        {
            commandList.Begin();
            commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
            commandList.ClearColorTarget(0, RgbaFloat.Black);
            commandList.ClearDepthStencil(1f);
            commandList.SetPipeline(Pipeline);
            commandList.SetGraphicsResourceSet(0, MatricesResourceSet);
            commandList.SetGraphicsResourceSet(2, PointLightsResourceSet);

            PointLightsInfo pointLightInfo = new PointLightsInfo
            {
                NumActiveLights = 1,
                PointLights0 = new PointLightInfo { Colour = new Vector3(1, 0, 0), Position = new Vector3(0, 0, -75), Range = 200f },
                PointLights1 = new PointLightInfo { Colour = new Vector3(1, 1, 1), Position = new Vector3(0, 0, 0), Range = 100f },
                PointLights2 = new PointLightInfo { Colour = new Vector3(0, 1, 0), Position = new Vector3(0, 0, 75), Range = 300f }
            };

            commandList.UpdateBuffer(PointLightsBuffer, 0, ref pointLightInfo);

            VeldridRenderer renderer = new VeldridRenderer(graphicsResources.Primitives, MatricesBuffer, commandList, graphicsResources.AssetManager, scene.Camera);
            scene.Draw(renderer);

            commandList.End();

            graphicsDevice.SubmitCommands(commandList);
        }
    }
}
