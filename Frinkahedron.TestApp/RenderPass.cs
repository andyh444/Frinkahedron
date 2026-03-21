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

    internal sealed class DirectionalShadowRenderPass : IRenderPass
    {
        public required Shader[] Shaders { get; init; }
        public required Pipeline Pipeline { get; init; }
        public required UniformBufferInfo MatricesBufferInfo { get; init; }
        public required Framebuffer Framebuffer { get; init; }
        public required TextureInfo DepthTexture { get; init; }

        public static DirectionalShadowRenderPass Create(ResourceFactory factory, GraphicsDevice graphicsDevice, AssetManager assetManager)
        {
            ShaderDescription vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                assetManager.GetShaderCode("ShadowPass.vert"),
                "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                assetManager.GetShaderCode("ShadowPass.frag"),
                "main");
            var shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            var matricesBufferInfo = UniformBufferInfo.Create<MatrixUniforms>(factory, "Matrices", ShaderStages.Vertex);

            TextureDescription depthDescription = TextureDescription.Texture2D(
                1024,
                1024,
                1,
                1,
                PixelFormat.R32_Float,
                TextureUsage.DepthStencil | TextureUsage.Sampled);
            var depthTexture = TextureInfo.Create(factory, graphicsDevice, depthDescription);
            var frameBuffer = factory.CreateFramebuffer(
                new FramebufferDescription(
                    depthTarget: new FramebufferAttachmentDescription(depthTexture.Texture, 0),
                    colorTargets: []
                ));

            
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

            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { MeshInfo.GetVertexLayoutDescription() },
                shaders: shaders);

            pipelineDescription.Outputs = frameBuffer.OutputDescription;
            pipelineDescription.ResourceLayouts = new[]
            {
                matricesBufferInfo.ResourceLayout
            };
            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            return new DirectionalShadowRenderPass
            {
                DepthTexture = depthTexture,
                Framebuffer = frameBuffer,
                MatricesBufferInfo = matricesBufferInfo,
                Pipeline = pipeline,
                Shaders = shaders
            };
        }

        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene)
        {
            if (scene.SceneLights.DirectionalLight is null)
            {
                return;
            }

            Camera lightCamera = scene.SceneLights.DirectionalLight.Value.GetDirectionalLightCamera();

            commandList.SetFramebuffer(Framebuffer);

            commandList.ClearDepthStencil(1f);
            commandList.SetPipeline(Pipeline);
            commandList.SetGraphicsResourceSet(0, MatricesBufferInfo.ResourceSet);

            VeldridRenderer renderer = new VeldridRenderer(
                graphicsResources.Primitives,
                MatricesBufferInfo.DeviceBuffer,
                commandList,
                graphicsResources.AssetManager,
                lightCamera,
                false);
            scene.Draw(renderer);
        }

        public void Dispose()
        {
            foreach (var shader in Shaders)
            {
                shader.Dispose();
            }
            Pipeline.Dispose();
            MatricesBufferInfo.Dispose();
        }
    }

    // TODO: Get the shadow map and light viewprojection into main render shaders
    internal sealed class MainRenderPass : IRenderPass
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
            CameraBufferInfo.Dispose();
        }

        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene)
        {
            
            commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
            commandList.ClearColorTarget(0, RgbaFloat.Black);
            commandList.ClearDepthStencil(1f);
            commandList.SetPipeline(Pipeline);
            commandList.SetGraphicsResourceSet(0, MatricesBufferInfo.ResourceSet);
            commandList.SetGraphicsResourceSet(2, LightsBufferInfo.ResourceSet);
            commandList.SetGraphicsResourceSet(3, CameraBufferInfo.ResourceSet);

            PointLightsInfo pointLightInfo = scene.GetPointLights();
            CameraInfo cameraInfo = scene.GetCameraInfo();
            DirectionalLightInfo directionalLight = scene.GetDirectionalLight();

            commandList.UpdateBuffer(LightsBufferInfo.PointLightsBuffer, 0, ref pointLightInfo);
            commandList.UpdateBuffer(LightsBufferInfo.DirectionalLightBuffer, 0, ref directionalLight);
            commandList.UpdateBuffer(CameraBufferInfo.DeviceBuffer, 0, ref cameraInfo);

            VeldridRenderer renderer = new VeldridRenderer(graphicsResources.Primitives, MatricesBufferInfo.DeviceBuffer, commandList, graphicsResources.AssetManager, scene.Camera, true);
            scene.Draw(renderer);
        }
    }
}
