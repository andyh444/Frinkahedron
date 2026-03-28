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

namespace Frinkahedron.VeldridImplementation
{
    public interface IRenderPass : IDisposable
    {
        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene);
    }

    public sealed class DirectionalShadowRenderPass : IRenderPass
    {
        public required Shader[] Shaders { get; init; }
        public required Pipeline Pipeline { get; init; }
        public required UniformBufferInfo ModelMatricesBufferInfo { get; init; }
        public required UniformBufferInfo CameraMatricesBufferInfo { get; init; }
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

            var modelBufferInfo = UniformBufferInfo.Create<ModelMatrixInfo>(factory, "ModelMatrices", ShaderStages.Vertex);
            var cameraMatrixBufferInfo = UniformBufferInfo.Create<CameraMatrixInfo>(factory, "CameraMatrices", ShaderStages.Vertex);

            TextureDescription depthDescription = TextureDescription.Texture2D(
                4096,
                4096,
                1,
                1,
                PixelFormat.D32_Float_S8_UInt,
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
                modelBufferInfo.ResourceLayout,
                cameraMatrixBufferInfo.ResourceLayout
            };
            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            return new DirectionalShadowRenderPass
            {
                DepthTexture = depthTexture,
                Framebuffer = frameBuffer,
                ModelMatricesBufferInfo = modelBufferInfo,
                CameraMatricesBufferInfo = cameraMatrixBufferInfo,
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
            commandList.SetGraphicsResourceSet(0, ModelMatricesBufferInfo.ResourceSet);
            commandList.SetGraphicsResourceSet(1, CameraMatricesBufferInfo.ResourceSet);

            CameraMatrixInfo cameraMatrixInfo = new CameraMatrixInfo
            {
                Projection = lightCamera.ProjectionMatrix,
                View = lightCamera.ViewMatrix,
            };

            commandList.UpdateBuffer(CameraMatricesBufferInfo.DeviceBuffer, 0, ref cameraMatrixInfo);

            VeldridRenderer renderer = new VeldridRenderer(
                graphicsResources.Primitives,
                ModelMatricesBufferInfo.DeviceBuffer,
                commandList,
                graphicsResources.AssetManager,
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
            ModelMatricesBufferInfo.Dispose();
            CameraMatricesBufferInfo.Dispose();
        }
    }

    public sealed class MainRenderPass : IRenderPass
    {
        public required Shader[] Shaders { get; init; }
        public required Pipeline Pipeline { get; init; }
        public required UniformBufferInfo ModelMatricesBufferInfo { get; init; }
        public required UniformBufferInfo CameraMatricesBufferInfo { get; init; }
        public required UniformBufferInfo LightMatricesBufferInfo { get; init; }
        public required LightingBufferInfo LightsBufferInfo { get; init; }
        public required UniformBufferInfo CameraBufferInfo { get; init; }
        public TextureInfo? ShadowMapTextureInfo { get; set; }

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

            var modelBufferInfo = UniformBufferInfo.Create<ModelMatrixInfo>(factory, "ModelMatrices", ShaderStages.Vertex);
            var cameraMatrixBufferInfo = UniformBufferInfo.Create<CameraMatrixInfo>(factory, "CameraMatrices", ShaderStages.Vertex);
            var lightMatrixBufferInfo = UniformBufferInfo.Create<CameraMatrixInfo>(factory, "LightMatrices", ShaderStages.Vertex);
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
                modelBufferInfo.ResourceLayout,
                cameraMatrixBufferInfo.ResourceLayout,
                TextureInfo.GetResourceLayout(factory),
                lightsBufferInfo.ResourceLayout,
                cameraBufferInfo.ResourceLayout,
                lightMatrixBufferInfo.ResourceLayout,
                TextureInfo.GetResourceLayout(factory),
                TextureInfo.GetResourceLayout(factory),
            };
            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            return new MainRenderPass
            {
                Shaders = shaders,
                Pipeline = pipeline,
                ModelMatricesBufferInfo = modelBufferInfo,
                CameraMatricesBufferInfo = cameraMatrixBufferInfo,
                LightMatricesBufferInfo = lightMatrixBufferInfo,
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
            ModelMatricesBufferInfo.Dispose();
            CameraMatricesBufferInfo.Dispose();
            LightMatricesBufferInfo.Dispose();
            LightsBufferInfo.Dispose();
            CameraBufferInfo.Dispose();
        }

        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene)
        {
            
            commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
            commandList.ClearColorTarget(0, RgbaFloat.Black);
            commandList.ClearDepthStencil(1f);
            commandList.SetPipeline(Pipeline);
            commandList.SetGraphicsResourceSet(0, ModelMatricesBufferInfo.ResourceSet);
            commandList.SetGraphicsResourceSet(1, CameraMatricesBufferInfo.ResourceSet);
            commandList.SetGraphicsResourceSet(3, LightsBufferInfo.ResourceSet);
            commandList.SetGraphicsResourceSet(4, CameraBufferInfo.ResourceSet);
            commandList.SetGraphicsResourceSet(5, LightMatricesBufferInfo.ResourceSet);
            commandList.SetGraphicsResourceSet(6, ShadowMapTextureInfo.ResourceSet);

            PointLightsInfo pointLightInfo = scene.GetPointLights();
            CameraInfo cameraInfo = scene.GetCameraInfo();
            DirectionalLightInfo directionalLight = scene.GetDirectionalLight();
            CameraMatrixInfo cameraMatrixInfo = new CameraMatrixInfo
            {
                Projection = scene.Camera.ProjectionMatrix,
                View = scene.Camera.ViewMatrix,
            };

            var lightCam = scene.SceneLights.DirectionalLight.Value.GetDirectionalLightCamera();
            CameraMatrixInfo lightMatrixInfo = new CameraMatrixInfo
            {
                Projection = lightCam.ProjectionMatrix,
                View = lightCam.ViewMatrix,
            };

            commandList.UpdateBuffer(CameraMatricesBufferInfo.DeviceBuffer, 0, ref cameraMatrixInfo);
            commandList.UpdateBuffer(LightMatricesBufferInfo.DeviceBuffer, 0, ref lightMatrixInfo);
            commandList.UpdateBuffer(LightsBufferInfo.PointLightsBuffer, 0, ref pointLightInfo);
            commandList.UpdateBuffer(LightsBufferInfo.DirectionalLightBuffer, 0, ref directionalLight);
            commandList.UpdateBuffer(CameraBufferInfo.DeviceBuffer, 0, ref cameraInfo);

            VeldridRenderer renderer = new VeldridRenderer(graphicsResources.Primitives, ModelMatricesBufferInfo.DeviceBuffer, commandList, graphicsResources.AssetManager, true);
            scene.Draw(renderer);
        }
    }
}
