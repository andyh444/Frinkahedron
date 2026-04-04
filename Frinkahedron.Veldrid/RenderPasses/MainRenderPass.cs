using Frinkahedron.Core;
using System.Numerics;
using Veldrid;
using Veldrid.SPIRV;
using static Frinkahedron.VeldridImplementation.VeldridRenderContext;

namespace Frinkahedron.VeldridImplementation.RenderPasses
{

    public sealed class MainRenderPass : IRenderPass
    {
        public required Shader[] Shaders { get; init; }
        public required Pipeline Pipeline { get; init; }
        public required Framebuffer Framebuffer { get; init; }
        public required UniformBufferInfo ModelMatricesBufferInfo { get; init; }
        public required UniformBufferInfo CameraMatricesBufferInfo { get; init; }
        public required UniformBufferInfo LightMatricesBufferInfo { get; init; }
        public required LightingBufferInfo LightsBufferInfo { get; init; }
        public required UniformBufferInfo CameraBufferInfo { get; init; }
        public TextureInfo? ShadowMapTextureInfo { get; set; }

        public static MainRenderPass Create(ResourceFactory factory, GraphicsDevice graphicsDevice, IAssetManager assetManager, Framebuffer frameBuffer)
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

            pipelineDescription.Outputs = frameBuffer.OutputDescription;
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
                TextureInfo.GetResourceLayout(factory),
            };
            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            return new MainRenderPass
            {
                Shaders = shaders,
                Pipeline = pipeline,
                Framebuffer = frameBuffer,
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

        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene, IReadOnlyList<DrawInstruction> sceneDrawInstructions)
        {

            commandList.SetFramebuffer(Framebuffer);
            commandList.ClearColorTarget(0, RgbaFloat.Black);
            commandList.ClearDepthStencil(1f);
            commandList.SetPipeline(Pipeline);
            commandList.SetGraphicsResourceSet(0, ModelMatricesBufferInfo.ResourceSet);
            commandList.SetGraphicsResourceSet(1, CameraMatricesBufferInfo.ResourceSet);
            commandList.SetGraphicsResourceSet(3, LightsBufferInfo.ResourceSet);
            commandList.SetGraphicsResourceSet(4, CameraBufferInfo.ResourceSet);
            commandList.SetGraphicsResourceSet(5, LightMatricesBufferInfo.ResourceSet);
            if (ShadowMapTextureInfo is not null)
            {
                commandList.SetGraphicsResourceSet(6, ShadowMapTextureInfo.ResourceSet);
            }
            PointLightsInfo pointLightInfo = scene.GetPointLights();
            CameraInfo cameraInfo = scene.GetCameraInfo();
            DirectionalLightInfo directionalLight = scene.GetDirectionalLight();
            CameraMatrixInfo cameraMatrixInfo = new CameraMatrixInfo
            {
                Projection = scene.Camera.ProjectionMatrix,
                View = scene.Camera.ViewMatrix,
            };

            if (scene.SceneLights.DirectionalLight is not null)
            {
                var lightCam = scene.SceneLights.DirectionalLight.Value.GetDirectionalLightCamera();
                CameraMatrixInfo lightMatrixInfo = new CameraMatrixInfo
                {
                    Projection = lightCam.ProjectionMatrix,
                    View = lightCam.ViewMatrix,
                };
                commandList.UpdateBuffer(LightMatricesBufferInfo.DeviceBuffer, 0, ref lightMatrixInfo);
            }
            commandList.UpdateBuffer(CameraMatricesBufferInfo.DeviceBuffer, 0, ref cameraMatrixInfo);
            commandList.UpdateBuffer(LightsBufferInfo.PointLightsBuffer, 0, ref pointLightInfo);
            commandList.UpdateBuffer(LightsBufferInfo.DirectionalLightBuffer, 0, ref directionalLight);
            commandList.UpdateBuffer(CameraBufferInfo.DeviceBuffer, 0, ref cameraInfo);

            foreach (var instruction in sceneDrawInstructions)
            {
                if (instruction.InstructionType is InstructionType.Model)
                {
                    DoDrawInstruction(instruction, commandList, graphicsResources.AssetManager);
                }
            }
        }

        private void DoDrawInstruction(DrawInstruction drawInstruction, CommandList commandList, IAssetManager assetManager)
        {
            var model = assetManager.GetModel(drawInstruction.ModelID);
            foreach (var entity in model.Entities)
            {
                DrawMesh(entity.Mesh, entity.Transform * drawInstruction.Transform, entity.ColourTexture, entity.NormalMap, entity.MetallicRoughnessMap, commandList);
            }
        }

        private void DrawMesh(MeshInfo meshInfo, Matrix4x4 transform, TextureInfo? albedo, TextureInfo? normalMap, TextureInfo? metallicRoughnessMap, CommandList commandList)
        {
            ModelMatrixInfo modelInfo = new ModelMatrixInfo
            {
                Model = transform,
            };
            if (albedo is not null)
            {
                commandList.SetGraphicsResourceSet(2, albedo.ResourceSet);
            }
            if (normalMap is not null)
            {
                commandList.SetGraphicsResourceSet(7, normalMap.ResourceSet);
            }
            if (metallicRoughnessMap is not null)
            {
                commandList.SetGraphicsResourceSet(8, metallicRoughnessMap.ResourceSet);
            }
            commandList.UpdateBuffer(ModelMatricesBufferInfo.DeviceBuffer, 0, ref modelInfo);
            meshInfo.Draw(commandList);
        }
    }
}
