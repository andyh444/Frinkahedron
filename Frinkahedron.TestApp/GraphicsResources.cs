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
        public required CommandList CommandList { get; init; }
        public required MeshInfo CubeInfo { get; init; }
        public required MeshInfo SphereInfo { get; init; }
        public required Shader[] Shaders { get; init; }
        public required Pipeline Pipeline { get; init; }
        public required DeviceBuffer MatricesBuffer { get; init; }
        public required ResourceLayout ResourceLayout { get; init; }
        public required ResourceSet ResourceSet { get; init; }

        public static GraphicsResources CreateResources(GraphicsDevice graphicsDevice)
        {
            ResourceFactory factory = graphicsDevice.ResourceFactory;

            Mesh mesh = CreateUnitCubeMesh();
            var cubeInfo = MeshInfo.Create(mesh, graphicsDevice);
            var sphereInfo = MeshInfo.Create(CreateUnitUVSphere(24, 24, RgbaFloat.Red.ToVector4(), RgbaFloat.Blue.ToVector4()), graphicsDevice);

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

            return new GraphicsResources
            {
                CommandList = commandList,
                CubeInfo = cubeInfo,
                SphereInfo = sphereInfo,
                Shaders = shaders,
                Pipeline = pipeline,
                MatricesBuffer = uniformBuffer,
                ResourceLayout = resourceLayout,
                ResourceSet = resourceSet,
            };
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

        private static Mesh CreateUnitCubeMesh()
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

        /// <summary>
        /// Generates a UV sphere mesh.
        /// </summary>
        /// <param name="radius">Sphere radius</param>
        /// <param name="longitudeSegments">Number of longitudinal slices</param>
        /// <param name="latitudeSegments">Number of latitudinal slices</param>
        /// <param name="color">Vertex color</param>
        /// <param name="vertices">Output vertex array</param>
        /// <param name="indices">Output triangle indices</param>
        public static Mesh CreateUnitUVSphere(int longitudeSegments, int latitudeSegments, Vector4 topColor, Vector4 bottomColour)
        {
            var vertList = new List<VertexPositionColor>();
            var indexList = new List<ushort>();
            float radius = 0.5f;
            // Generate vertices
            for (int lat = 0; lat <= latitudeSegments; lat++)
            {
                float theta = lat * MathF.PI / latitudeSegments; // 0 to π
                float sinTheta = MathF.Sin(theta);
                float cosTheta = MathF.Cos(theta);

                Vector4 colour = Vector4.Lerp(topColor, bottomColour, (float)lat / latitudeSegments);
                for (int lon = 0; lon <= longitudeSegments; lon++)
                {
                    float phi = lon * 2f * MathF.PI / longitudeSegments; // 0 to 2π
                    float sinPhi = MathF.Sin(phi);
                    float cosPhi = MathF.Cos(phi);

                    Vector3 pos = new Vector3(
                        radius * sinTheta * cosPhi,
                        radius * cosTheta,
                        radius * sinTheta * sinPhi
                    );

                    vertList.Add(new VertexPositionColor(pos, colour));
                }
            }

            // Generate indices
            for (int lat = 0; lat < latitudeSegments; lat++)
            {
                for (int lon = 0; lon < longitudeSegments; lon++)
                {
                    ushort first = (ushort)((lat * (longitudeSegments + 1)) + lon);
                    ushort second = (ushort)(first + longitudeSegments + 1);

                    // Each quad is split into two triangles
                    indexList.Add(first);
                    indexList.Add(second);
                    indexList.Add((ushort)(first + 1));

                    indexList.Add(second);
                    indexList.Add((ushort)(second + 1));
                    indexList.Add((ushort)(first + 1));
                }
            }
            return new Mesh(vertList.ToArray(), indexList.ToArray());
        }
    }
}
