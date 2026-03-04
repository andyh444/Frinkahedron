using System.Numerics;
using System.Text;
using Veldrid;
using Frinkahedron.Core;
using System.Runtime.CompilerServices;
using Veldrid.SPIRV;

namespace Frinkahedron.TestApp
{
    internal sealed class GraphicsResources : IDisposable
    {
        public CommandList CommandList { get; }
        public MeshInfo CubeInfo { get; }
        public Shader[] Shaders { get; }
        public Pipeline Pipeline { get; }
        public DeviceBuffer MatricesBuffer { get; }
        public ResourceLayout ResourceLayout { get; }
        public ResourceSet ResourceSet { get; }

        private GraphicsResources(CommandList commandList, MeshInfo meshInfo, Shader[] shaders, Pipeline pipeline, DeviceBuffer uniformBuffer, ResourceLayout resourceLayout, ResourceSet resourceSet)
        {
            CommandList = commandList;
            CubeInfo = meshInfo;
            Shaders = shaders;
            Pipeline = pipeline;
            MatricesBuffer = uniformBuffer;
            ResourceLayout = resourceLayout;
            ResourceSet = resourceSet;
        }

        public static GraphicsResources CreateResources(GraphicsDevice graphicsDevice)
        {
            ResourceFactory factory = graphicsDevice.ResourceFactory;

            Mesh mesh = CreateCubeMesh();
            var meshInfo = MeshInfo.Create(mesh, graphicsDevice);

            ShaderDescription vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                Encoding.UTF8.GetBytes(Frinkahedron.TestApp.Shaders.ColouredVertexShader),
                "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                Encoding.UTF8.GetBytes(Frinkahedron.TestApp.Shaders.SimpleFragmentShader),
                "main");

            var shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: true,
                depthWriteEnabled: true,
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

            pipelineDescription.Outputs = graphicsDevice.SwapchainFramebuffer.OutputDescription;

            // Create uniform buffer
            var uniformBuffer = factory.CreateBuffer(new BufferDescription(
                (uint)Unsafe.SizeOf<MatrixUniforms>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));

            // Create resource layout for the uniform buffer
            var resourceLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("Matrices", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            // Create resource set for the uniform buffer
            var resourceSet = factory.CreateResourceSet(new ResourceSetDescription(
                resourceLayout, uniformBuffer));

            pipelineDescription.ResourceLayouts = new[] { resourceLayout };

            var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            var commandList = factory.CreateCommandList();

            return new GraphicsResources(commandList, meshInfo, shaders, pipeline, uniformBuffer, resourceLayout, resourceSet);
        }

        public void Dispose()
        {
            Pipeline.Dispose();
            foreach (var shader in Shaders)
            {
                shader.Dispose();
            }
            CommandList.Dispose();
            CubeInfo.Dispose();

            MatricesBuffer.Dispose();
            ResourceLayout.Dispose();
            ResourceSet.Dispose();
        }

        private static Mesh CreateQuadMesh()
        {
            VertexPositionColor[] quadVertices =
            {
            new VertexPositionColor(new Vector3(-0.75f, 0.75f, 0), RgbaFloat.Red.ToVector4()),
            new VertexPositionColor(new Vector3(0.75f, 0.75f, 0), RgbaFloat.Green.ToVector4()),
            new VertexPositionColor(new Vector3(-0.75f, -0.75f, 0), RgbaFloat.Blue.ToVector4()),
            new VertexPositionColor(new Vector3(0.75f, -0.75f, 0), RgbaFloat.Yellow.ToVector4())
        };

            ushort[] quadIndices = { 0, 1, 2, 3 };

            return new Mesh(quadVertices, quadIndices);
        }

        private static Mesh CreateCubeMesh()
        {
            VertexPositionColor[] vertices =
            {
               new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0.5f), RgbaFloat.Red.ToVector4()),
               new VertexPositionColor(new Vector3(0.5f, 0.5f, 0.5f), RgbaFloat.Green.ToVector4()),
               new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.5f), RgbaFloat.Blue.ToVector4()),
               new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.5f), RgbaFloat.Yellow.ToVector4()),

               new VertexPositionColor(new Vector3(-0.5f, 0.5f, -0.5f), RgbaFloat.Green.ToVector4()),
               new VertexPositionColor(new Vector3(0.5f, 0.5f, -0.5f), RgbaFloat.Blue.ToVector4()),
               new VertexPositionColor(new Vector3(-0.5f, -0.5f, -0.5f), RgbaFloat.Yellow.ToVector4()),
               new VertexPositionColor(new Vector3(0.5f, -0.5f, -0.5f), RgbaFloat.Red.ToVector4())
           };

            ushort[] indices =
            {  
               // Front face  
               0, 1, 2,
               2, 1, 3,  

               // Back face  
               4, 6, 5,
               5, 6, 7,  

               // Left face  
               4, 0, 6,
               6, 0, 2,  

               // Right face  
               1, 5, 3,
               3, 5, 7,  

               // Top face  
               4, 5, 0,
               0, 5, 1,  

               // Bottom face  
               2, 3, 6,
               6, 3, 7
           };

            return new Mesh(vertices, indices);
        }
    }
}
