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
        public required TextureInfo MainTexture { get; init; }

        public static MainRenderPass Create(ResourceFactory factory, GraphicsDevice graphicsDevice)
        {
            ShaderDescription vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                Encoding.UTF8.GetBytes(Frinkahedron.TestApp.Shaders.PositionUvVertexShader),
                "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                Encoding.UTF8.GetBytes(Frinkahedron.TestApp.Shaders.TextureFragmentShader),
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

            var mainTexture = TextureInfo.Create(factory, graphicsDevice);

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
            pipelineDescription.ResourceLayouts = new[] { resourceLayout, mainTexture.ResourceLayout };
            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            return new MainRenderPass
            {
                Shaders = shaders,
                Pipeline = pipeline,
                MatricesBuffer = uniformBuffer,
                ResourceLayout = resourceLayout,
                ResourceSet = resourceSet,
                MainTexture = TextureInfo.Create(factory, graphicsDevice)
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
            commandList.SetGraphicsResourceSet(1, MainTexture.ResourceSet);

            VeldridRenderer renderer = new VeldridRenderer(graphicsResources.Primitives, MatricesBuffer, commandList, scene.Camera);
            scene.Draw(renderer);

            commandList.End();

            graphicsDevice.SubmitCommands(commandList);
        }
    }

    public class TextureInfo : IDisposable
    {
        public required Texture Texture { get; init; }
        public required TextureView TextureView { get; init; }
        public required Sampler Sampler { get; init; }
        public required ResourceSet ResourceSet { get; init; }
        public required ResourceLayout ResourceLayout { get; init; }

        public static TextureInfo Create(ResourceFactory factory, GraphicsDevice graphicsDevice)
        {
            Texture texture = factory.CreateTexture(
                TextureDescription.Texture2D(
                width: 64,
                height: 64,
                mipLevels: 1,
                arrayLayers: 1,
                PixelFormat.R8_G8_B8_A8_UNorm,
                TextureUsage.Sampled));

            TextureView textureView = factory.CreateTextureView(texture);

            Span<int> pixels = new int[texture.Width * texture.Height];
            for (int y = 0; y < texture.Height; y++)
            {
                for (int x =  0; x < texture.Height; x++)
                {
                    byte intensity8 = (byte)(x * 255 / texture.Width);
                    byte r = (byte)Random.Shared.Next(0, 255); //intensity8;
                    byte g = (byte)Random.Shared.Next(0, 255); //intensity8;
                    byte b = (byte)Random.Shared.Next(0, 255); //intensity8;
                    byte alpha = 255;
                    int index = y * (int)texture.Width + x;
                    pixels[index] = alpha << 24 | b << 16 | g << 8 | r;
                }
            }
            graphicsDevice.UpdateTexture(texture, pixels, 0, 0, 0, texture.Width, texture.Height, texture.Depth, 0, 0);

            Sampler sampler = graphicsDevice.Aniso4xSampler;

            var textureLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Texture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("TextureSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

            ResourceSet textureSet = factory.CreateResourceSet(
                new ResourceSetDescription(
                    textureLayout,
                    textureView,
                    sampler));

            return new TextureInfo
            {
                Texture = texture,
                TextureView = textureView,
                Sampler = sampler,
                ResourceSet = textureSet,
                ResourceLayout = textureLayout
            };
        }

        public void Dispose()
        {
            Texture.Dispose();
            TextureView.Dispose();
            Sampler.Dispose();
            ResourceSet.Dispose();
            ResourceLayout.Dispose();
        }
    }
}
