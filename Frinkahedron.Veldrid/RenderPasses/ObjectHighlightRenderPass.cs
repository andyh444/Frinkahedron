using Frinkahedron.Core;
using System.Numerics;
using Veldrid;
using Veldrid.SPIRV;
using static Frinkahedron.VeldridImplementation.VeldridRenderContext;

namespace Frinkahedron.VeldridImplementation.RenderPasses
{
    public sealed class ObjectHighlightRenderPass : IRenderPass
    {
        public required Shader[] Shaders { get; init; }

        public required Pipeline Pipeline { get; init; }

        public required Framebuffer Framebuffer { get; init; }

        public required UniformBufferInfo ModelMatricesBufferInfo { get; init; }

        public required UniformBufferInfo CameraMatricesBufferInfo { get; init; }

        public required UniformBufferInfo HighlightParamsBufferInfo { get; init; }

        public static ObjectHighlightRenderPass Create(ResourceFactory factory, GraphicsDevice graphicsDevice, IAssetManager assetManager, Framebuffer frameBuffer)
        {
            ShaderDescription vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                assetManager.GetShaderCode("ObjectHighlightPass.vert"),
                "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                assetManager.GetShaderCode("ObjectHighlightPass.frag"),
                "main");
            var shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            var modelBufferInfo = UniformBufferInfo.Create<ModelMatrixInfo>(factory, "ModelMatrices", ShaderStages.Vertex);
            var cameraMatrixBufferInfo = UniformBufferInfo.Create<CameraMatrixInfo>(factory, "CameraMatrices", ShaderStages.Vertex);
            var highlightParamsBufferInfo = UniformBufferInfo.Create<HighlightParams>(factory, "HighlightParams", ShaderStages.Vertex | ShaderStages.Fragment);

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

            // disable depth testing here - I want whatever is drawn last to be drawn on top
            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: false,
                depthWriteEnabled: false,
                comparisonKind: ComparisonKind.LessEqual);

            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.None,
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
                highlightParamsBufferInfo.ResourceLayout,
            };
            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            return new ObjectHighlightRenderPass
            {
                Shaders = shaders,
                Framebuffer = frameBuffer,
                Pipeline = pipeline,
                ModelMatricesBufferInfo = modelBufferInfo,
                CameraMatricesBufferInfo = cameraMatrixBufferInfo,
                HighlightParamsBufferInfo = highlightParamsBufferInfo,
            };
        }

        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene, IReadOnlyList<DrawInstruction> sceneDrawInstructions)
        {
            commandList.SetFramebuffer(Framebuffer);
            commandList.ClearColorTarget(0, RgbaFloat.Clear);
            commandList.ClearDepthStencil(1f);
            commandList.SetPipeline(Pipeline);
            commandList.SetGraphicsResourceSet(0, ModelMatricesBufferInfo.ResourceSet);
            commandList.SetGraphicsResourceSet(1, CameraMatricesBufferInfo.ResourceSet);
            commandList.SetGraphicsResourceSet(2, HighlightParamsBufferInfo.ResourceSet);
            
            CameraMatrixInfo cameraMatrixInfo = new CameraMatrixInfo
            {
                Projection = scene.Camera.Projection.Matrix,
                View = scene.Camera.ViewMatrix,
            };

            commandList.UpdateBuffer(CameraMatricesBufferInfo.DeviceBuffer, 0, ref cameraMatrixInfo);

            foreach (var instruction in sceneDrawInstructions)
            {
                if (instruction.InstructionType is InstructionType.ModelEntityHighlight)
                {
                    var highlightParams = instruction.HighlightParams;
                    commandList.UpdateBuffer(HighlightParamsBufferInfo.DeviceBuffer, 0, ref highlightParams);

                    var model = graphicsResources.AssetManager.GetModel(instruction.ModelID);
                    DrawMesh(model.Entities[instruction.EntityIndex].Mesh, instruction.Transform, commandList);
                }
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
            HighlightParamsBufferInfo.Dispose();
        }
    }
}
