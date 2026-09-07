using Frinkahedron.Core;
using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Meshes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Veldrid.SPIRV;

namespace Frinkahedron.VeldridImplementation.RenderPasses
{
    internal class ScreenMesh2RenderPass : IRenderPass
    {
        private class RectPos(Vector2 initialPos, Vector2 scale, float initialRotation, Vector2 velocity, float angularVelocity)
        {
            public Vector2 Position { get; private set; } = initialPos;

            public Vector2 Scale { get; } = scale;

            public float Rotation { get; private set; } = initialRotation;

            public Vector2 Velocity { get; } = velocity;

            public float AngularVelocity { get; } = angularVelocity;

            public void Increment(float timeStep)
            {
                Position += Velocity * timeStep;
                Rotation += AngularVelocity * timeStep;
            }
        }

        public required Shader[] Shaders { get; init; }

        public required Pipeline Pipeline { get; init; }

        public required Framebuffer Framebuffer { get; init; }

        public required UniformBufferInfo ModelMatricesBufferInfo { get; init; }

        public required List<MeshInfo<ColourVertex2>> meshes;

        private List<RectPos> positions;

        public static ScreenMesh2RenderPass Create(ResourceFactory factory, GraphicsDevice graphicsDevice, IAssetManager assetManager, Framebuffer frameBuffer)
        {
            ShaderDescription vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                assetManager.GetShaderCode("ScreenMesh2Pass.vert"),
                "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                assetManager.GetShaderCode("ScreenMesh2Pass.frag"),
                "main");
            var shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            var modelBufferInfo = UniformBufferInfo.Create<Model2MatrixInfo>(factory, "ModelMatrices", ShaderStages.Vertex);

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: false,
                depthWriteEnabled: false,
                comparisonKind: ComparisonKind.LessEqual);

            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.None,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: false,
                scissorTestEnabled: false);

            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleList;

            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { MeshInfo<ColourVertex2>.GetVertexLayoutDescription() },
                shaders: shaders);

            pipelineDescription.Outputs = frameBuffer.OutputDescription;
            pipelineDescription.ResourceLayouts = new[]
            {
                modelBufferInfo.ResourceLayout
            };
            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            List<RectPos> positions = new List<RectPos>();
            Random r = Random.Shared;
            for (int i = 0; i < 10; i++)
            {
                positions.Add(new RectPos(
                    new Vector2(r.NextSingle(0, 800), r.NextSingle(0, 600)),
                    new Vector2(r.NextSingle(50, 200), r.NextSingle(50, 500)),
                    r.NextSingle(0, MathF.PI),
                    new Vector2(r.NextSingle(-2, 2), r.NextSingle(-2, 2)),
                    r.NextSingle(0, MathF.PI)));
            }

            return new ScreenMesh2RenderPass
            {
                Shaders = shaders,
                Framebuffer = frameBuffer,
                Pipeline = pipeline,
                ModelMatricesBufferInfo = modelBufferInfo,
                meshes = [MeshInfo<ColourVertex2>.Create(TexMesh2Factory.CreateUnitQuad(0.1f, RgbaFloat.Red.ToVector4()), graphicsDevice)],
                positions = positions
            };
        }

        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene, IReadOnlyList<VeldridRenderContext.DrawInstruction> sceneDrawInstructions)
        {
            commandList.SetFramebuffer(Framebuffer);
            //commandList.ClearColorTarget(0, RgbaFloat.Red);
            //commandList.ClearDepthStencil(1f);
            commandList.SetPipeline(Pipeline);
            commandList.SetGraphicsResourceSet(0, ModelMatricesBufferInfo.ResourceSet);

            var projection = Create2DProjectionMatrix(graphicsResources.ScreenWidth, graphicsResources.ScreenHeight);

            /*foreach (var p in positions)
            {
                DrawRectangle(p.Position, p.Scale, p.Rotation, commandList, projection);
                p.Increment(1 / 100f);
            }*/
        }

        private Matrix3x2 Create2DProjectionMatrix(int screenWidth, int screenHeight)
        {
            // map screen coordinates (0 to width/height) to NDC coordinates (-1 to +1)
            // includes inverting Y
            return Matrix3x2.CreateScale(2f / screenWidth, 2f / -screenHeight)
                * Matrix3x2.CreateTranslation(-1f, 1f);
        }

        private void DrawRectangle(Vector2 centre, Vector2 dimensions, float rotation, CommandList commandList, Matrix3x2 projection)
        {
            var scale = Matrix3x2.CreateScale(dimensions);
            var rot = Matrix3x2.CreateRotation(rotation);
            var translation = Matrix3x2.CreateTranslation(centre);

            var model = scale * rot * translation;

            Model2MatrixInfo modelInfo = Model2MatrixInfo.FromMatrix3x2(model * projection);
            commandList.UpdateBuffer(ModelMatricesBufferInfo.DeviceBuffer, 0, ref modelInfo);
            meshes.First().Draw(commandList);
        }

        public void Dispose()
        {
            foreach (var shader in Shaders)
            {
                shader.Dispose();
            }
            Pipeline.Dispose();
            ModelMatricesBufferInfo.Dispose();
        }
    }
}
