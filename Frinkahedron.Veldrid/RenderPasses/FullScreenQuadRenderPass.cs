using Frinkahedron.Core;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;
using Veldrid.SPIRV;
using Vulkan;
using static Frinkahedron.VeldridImplementation.VeldridRenderContext;

namespace Frinkahedron.VeldridImplementation.RenderPasses
{
    public class FullScreenQuadRenderPass : IRenderPass
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PostProcessSettings
        {
            public int enableFXAA;
            public int enableGaussian;
            public int gaussianRadius;
            public float gaussianSigma;

            public int enableMean;
            public int meanRadius;
            public int enablePixelation;
            public int pixelationRadius;

            public int enableSepia;
            public float sepiaStrength;

            public int enableGreyscale;
            // padding to align to 16 bytes for std140 compatibility
            public int _pad0;
            //public int _pad1;
            //public int _pad2;
        }

        private struct QuadVertex(Vector2 position, Vector2 uv)
        {
            public Vector2 Position = position;
            public Vector2 UV = uv;
        }

        public required Shader[] Shaders { get; init; }

        public required Pipeline Pipeline { get; init; }

        public required MeshInfo<TexVertex2> QuadMesh { get; init; }

        public List<(TextureInfo texture, PostProcessSettings settings)> Textures { get; } = new List<(TextureInfo texture, PostProcessSettings settings)>();

        public Swapchain? Swapchain { get; set; }

        public required UniformBufferInfo PostProcessSettingsBufferInfo { get; init; }

        public static FullScreenQuadRenderPass Create(ResourceFactory factory, GraphicsDevice graphicsDevice, IAssetManager assetManager, Swapchain? swapchain)
        {
            ShaderDescription vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                assetManager.GetShaderCode("ScreenQuadPass.vert"),
                "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                assetManager.GetShaderCode("ScreenQuadPass.frag"),
                "main");
            var shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            TexVertex2[] vertices = new[]
            {
                new TexVertex2(new Vector2(-1, 1), new Vector2(0, 0)),
                new TexVertex2(new Vector2(1, 1), new Vector2(1, 0)),
                new TexVertex2(new Vector2(-1, -1), new Vector2(0, 1)),
                new TexVertex2(new Vector2(1, -1), new Vector2(1, 1))
            };
            IndexTriangle[] triangles = new IndexTriangle[]
            {
                new IndexTriangle(0, 1, 2),
                new IndexTriangle(1, 2, 3),
            };

            TexMesh2 mesh = new TexMesh2(vertices, triangles);

            var quad = MeshInfo.Create(mesh, graphicsDevice);

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleAdditiveBlend;

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
                vertexLayouts: new VertexLayoutDescription[] { MeshInfo<TexVertex2>.GetVertexLayoutDescription() },
                shaders: shaders);

            pipelineDescription.Outputs = swapchain?.Framebuffer.OutputDescription ?? graphicsDevice.SwapchainFramebuffer.OutputDescription;
            // Create the post-process uniform buffer info (will be set as set=1)
            var postProcessInfo = UniformBufferInfo.Create<PostProcessSettings>(factory, "PostProcessSettings", ShaderStages.Fragment);

            // Resource layouts: set=0 is texture layout (texture + sampler), set=1 is the post-process uniform buffer
            pipelineDescription.ResourceLayouts = new[]
            {
                TextureInfo.GetResourceLayout(factory),
                postProcessInfo.ResourceLayout
            };

            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            return new FullScreenQuadRenderPass()
            {
                QuadMesh = quad,
                Pipeline = pipeline,
                Shaders = shaders,
                Swapchain = swapchain,
                PostProcessSettingsBufferInfo = postProcessInfo
            };
        }

        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene, IReadOnlyList<DrawInstruction> sceneDrawInstructions)
        {
            commandList.SetFramebuffer(Swapchain?.Framebuffer ?? graphicsDevice.SwapchainFramebuffer);
            commandList.ClearColorTarget(0, RgbaFloat.Black);
            commandList.SetPipeline(Pipeline);
            foreach ((var texture, var settings) in Textures)
            {
                // set 0: texture resource set (contains texture + sampler)
                commandList.SetGraphicsResourceSet(0, texture.ResourceSet);
                // set 1: post-process settings uniform buffer
                commandList.SetGraphicsResourceSet(1, PostProcessSettingsBufferInfo.ResourceSet);
                PostProcessSettings thisSettings = settings;
                commandList.UpdateBuffer(PostProcessSettingsBufferInfo.DeviceBuffer, 0, ref thisSettings);
                QuadMesh.Draw(commandList);
            }
        }

        public void Dispose()
        {
            QuadMesh.Dispose();
            Pipeline.Dispose();
            PostProcessSettingsBufferInfo.Dispose();
            foreach (var shader in Shaders)
            {
                shader.Dispose();
            }
        }
    }
}
