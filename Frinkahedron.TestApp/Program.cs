using Frinkahedron;
using Frinkahedron.Core;
using Frinkahedron.TestApp;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

internal class Program
{
    private static GraphicsDevice? _graphicsDevice;
    private static CommandList _commandList;
    private static MeshInfo _meshInfo;
    private static Matrix4x4 _meshTransform;
    private static Shader[] _shaders;
    private static Pipeline _pipeline;

    private static DeviceBuffer _uniformBuffer;
    private static ResourceLayout _resourceLayout;
    private static ResourceSet _resourceSet;

    private const string VertexCode = @"
#version 450

layout(location = 0) in vec3 Position;
layout(location = 1) in vec4 Color;

layout(location = 0) out vec4 fsin_Color;

layout(set = 0, binding = 0) uniform Matrices
{
    mat4 model;
    mat4 view;
    mat4 projection;
};

void main()
{
    gl_Position = projection * view * model * vec4(Position, 1);
    fsin_Color = Color;
}";

    private const string FragmentCode = @"
#version 450

layout(location = 0) in vec4 fsin_Color;
layout(location = 0) out vec4 fsout_Color;

void main()
{
    fsout_Color = fsin_Color;
}";

    private static Camera _camera;

    private static void Main(string[] args)
    {
        WindowCreateInfo windowCI = new WindowCreateInfo()
        {
            X = 100,
            Y = 100,
            WindowWidth = 960,
            WindowHeight = 540,
            WindowTitle = "Veldrid Tutorial"
        };
        Sdl2Window window = VeldridStartup.CreateWindow(ref windowCI);

        GraphicsDeviceOptions options = new GraphicsDeviceOptions
        {
            PreferStandardClipSpaceYDirection = true,
            PreferDepthRangeZeroToOne = true,
            SwapchainDepthFormat = PixelFormat.D32_Float_S8_UInt,
        };
        _graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, options);
        _camera = new Camera(new Vector3(0, 0, -2), new Vector3(0, 0, 1));
        CreateResources();

        float zAngle = 0;
        float yAngle = 0;
        float xAngle = 0;
        while (window.Exists)
        {
            _ = window.PumpEvents();
            Draw();

            zAngle += 0.0002f;
            yAngle += 0.0001f;
            xAngle += 0.0003f;
            _meshTransform = Matrix4x4.CreateRotationX(xAngle) * Matrix4x4.CreateRotationY(yAngle)  * Matrix4x4.CreateRotationZ(zAngle);
        }
    }

    private static void Draw()
    {
        MatrixUniforms uniforms = new MatrixUniforms
        {
            Model = _meshTransform,
            View = _camera.ViewMatrix,
            Projection = _camera.ProjectionMatrix
        };
        _graphicsDevice.UpdateBuffer(_uniformBuffer, 0, ref uniforms);

        _commandList.Begin();
        _commandList.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);
        _commandList.ClearColorTarget(0, RgbaFloat.Black);
        _commandList.ClearDepthStencil(1f);

        _commandList.SetPipeline(_pipeline);

        _commandList.SetGraphicsResourceSet(0, _resourceSet);

        _meshInfo.Draw(_commandList);

        _commandList.End();

        _graphicsDevice.SubmitCommands(_commandList);
        _graphicsDevice.SwapBuffers();
    }

    private static void CreateResources()
    {
        ResourceFactory factory = _graphicsDevice.ResourceFactory;

        Mesh mesh = CreateCubeMesh();
        _meshInfo = MeshInfo.Create(mesh, _graphicsDevice);
        _meshTransform = Matrix4x4.Identity;

        ShaderDescription vertexShaderDesc = new ShaderDescription(
            ShaderStages.Vertex,
            Encoding.UTF8.GetBytes(VertexCode),
            "main");
        ShaderDescription fragmentShaderDesc = new ShaderDescription(
            ShaderStages.Fragment,
            Encoding.UTF8.GetBytes(FragmentCode),
            "main");

        _shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

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
            shaders: _shaders);

        pipelineDescription.Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription;

        // Create uniform buffer
        _uniformBuffer = factory.CreateBuffer(new BufferDescription(
            (uint)Unsafe.SizeOf<MatrixUniforms>(), BufferUsage.UniformBuffer | BufferUsage.Dynamic));

        // Create resource layout for the uniform buffer
        _resourceLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("Matrices", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

        // Create resource set for the uniform buffer
        _resourceSet = factory.CreateResourceSet(new ResourceSetDescription(
            _resourceLayout, _uniformBuffer));

        pipelineDescription.ResourceLayouts = new[] { _resourceLayout };

        _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

        _commandList = factory.CreateCommandList();
    }

    private static void DisposeResources()
    {
        _pipeline.Dispose();
        foreach (var shader in _shaders)
        {
            shader.Dispose();
        }
        _commandList.Dispose();
        _meshInfo.Dispose();
        _graphicsDevice.Dispose();

        _uniformBuffer.Dispose();
        _resourceLayout.Dispose();
        _resourceSet.Dispose();
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



struct MatrixUniforms
{
    public Matrix4x4 Model;
    public Matrix4x4 View;
    public Matrix4x4 Projection;
}