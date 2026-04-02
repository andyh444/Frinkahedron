using Frinkahedron.Core;
using System.Numerics;
using Veldrid;
using Veldrid.SPIRV;
using static Frinkahedron.VeldridImplementation.VeldridRenderContext;

namespace Frinkahedron.VeldridImplementation.RenderPasses
{
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

        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene, IReadOnlyList<DrawInstruction> sceneDrawInstructions)
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

            foreach (var instruction in sceneDrawInstructions)
            {
                if (instruction.InstructionType is InstructionType.Model)
                {
                    DoDrawInstruction(instruction, commandList, graphicsResources.AssetManager);
                }
            }
        }

        private void DoDrawInstruction(DrawInstruction drawInstruction, CommandList commandList, AssetManager assetManager)
        {
            var model = assetManager.GetModel(drawInstruction.ModelID);
            foreach (var entity in model.Entities)
            {
                DrawMesh(entity.Mesh, entity.Transform * drawInstruction.Transform, commandList);
            }
        }

        private void DrawMesh(MeshInfo meshInfo, Matrix4x4 transform, CommandList commandList)
        {
            ModelMatrixInfo modelInfo = new ModelMatrixInfo
            {
                Model = transform,
            };
            commandList.UpdateBuffer(ModelMatricesBufferInfo.DeviceBuffer, 0, ref modelInfo);
            meshInfo.Draw(commandList);
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
}
