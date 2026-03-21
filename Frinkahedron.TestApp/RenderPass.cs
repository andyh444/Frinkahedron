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
        public required UniformBufferInfo MatricesBufferInfo { get; init; }
        public required LightingBufferInfo LightsBufferInfo{ get; init; }
        public required UniformBufferInfo CameraBufferInfo { get; init; }

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
            var matricesBufferInfo = UniformBufferInfo.Create<MatrixUniforms>(factory, "Matrices", ShaderStages.Vertex);
            var lightsBufferInfo = LightingBufferInfo.Create(factory, "PointLights", ShaderStages.Fragment);
            var cameraBufferInfo = UniformBufferInfo.Create<CameraInfo>(factory, "Camera", ShaderStages.Fragment);

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
            pipelineDescription.ResourceLayouts = new[]
            {
                matricesBufferInfo.ResourceLayout,
                TextureInfo.GetResourceLayout(factory),
                lightsBufferInfo.ResourceLayout,
                cameraBufferInfo.ResourceLayout,
            };
            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            return new MainRenderPass
            {
                Shaders = shaders,
                Pipeline = pipeline,
                MatricesBufferInfo = matricesBufferInfo,
                LightsBufferInfo = lightsBufferInfo,
                CameraBufferInfo = cameraBufferInfo,
            };
        }

        public void Dispose()
        {
            foreach (var shader in Shaders)
            {
                shader.Dispose();
            }
            Pipeline.Dispose();
            MatricesBufferInfo.Dispose();
            LightsBufferInfo.Dispose();
        }

        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene)
        {
            commandList.Begin();
            commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
            commandList.ClearColorTarget(0, RgbaFloat.Black);
            commandList.ClearDepthStencil(1f);
            commandList.SetPipeline(Pipeline);
            commandList.SetGraphicsResourceSet(0, MatricesBufferInfo.ResourceSet);
            commandList.SetGraphicsResourceSet(2, LightsBufferInfo.ResourceSet);
            commandList.SetGraphicsResourceSet(3, CameraBufferInfo.ResourceSet);

            // TODO: light info should come from scene
            PointLightsInfo pointLightInfo = new PointLightsInfo
            {
                NumActiveLights = 0,
                PointLights0 = new PointLightInfo { Colour = new Vector3(1, 1, 1), Position = new Vector3(0, 0, 0), Range = 100f },
                PointLights1 = new PointLightInfo { Colour = new Vector3(1, 0, 0), Position = new Vector3(0, 0, -75), Range = 200f },
                PointLights2 = new PointLightInfo { Colour = new Vector3(0, 1, 0), Position = new Vector3(0, 0, 75), Range = 300f }
            };

            CameraInfo cameraInfo = new CameraInfo
            {
                WorldPosition = scene.Camera.Position,
                LookDirection = scene.Camera.LookDirection,
            };

            DirectionalLightInfo directionalLight = new DirectionalLightInfo()
            {
                Enabled = 1,
                Colour = new Vector3(1, 1, 1),
                Direction = Vector3.Normalize(new Vector3(-1, -1, 0))
            };
            commandList.UpdateBuffer(LightsBufferInfo.PointLightsBuffer, 0, ref pointLightInfo);
            commandList.UpdateBuffer(LightsBufferInfo.DirectionalLightBuffer, 0, ref directionalLight);
            commandList.UpdateBuffer(CameraBufferInfo.DeviceBuffer, 0, ref cameraInfo);

            VeldridRenderer renderer = new VeldridRenderer(graphicsResources.Primitives, MatricesBufferInfo.DeviceBuffer, commandList, graphicsResources.AssetManager, scene.Camera);
            scene.Draw(renderer);

            commandList.End();

            graphicsDevice.SubmitCommands(commandList);
        }
    }
}
