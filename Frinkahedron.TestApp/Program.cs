using Frinkahedron;
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
    private static DeviceBuffer _vertexBuffer;
    private static DeviceBuffer _indexBuffer;
    private static Shader[] _shaders;
    private static Pipeline _pipeline;

    private static DeviceBuffer _uniformBuffer;
    private static ResourceLayout _resourceLayout;
    private static ResourceSet _resourceSet;

    private const string VertexCode = @"
#version 450

layout(location = 0) in vec4 Position;
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
    gl_Position = projection * view * model * Position;
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
            PreferDepthRangeZeroToOne = true
        };
        _graphicsDevice = VeldridStartup.CreateGraphicsDevice(window, options);
        _camera = new Camera(new Vector3(0, 0, -1), new Vector3(0, 0, 1));
        CreateResources();

        while (window.Exists)
        {
            _ = window.PumpEvents();
            Draw();
        }
    }

    private static void Draw()
    {
        MatrixUniforms uniforms = new MatrixUniforms
        {
            Model = Matrix4x4.Identity,
            View = _camera.ViewMatrix,
            Projection = _camera.ProjectionMatrix
        };
        _graphicsDevice.UpdateBuffer(_uniformBuffer, 0, ref uniforms);


        _commandList.Begin();
        _commandList.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);
        _commandList.ClearColorTarget(0, RgbaFloat.Red);

        _commandList.SetVertexBuffer(0, _vertexBuffer);
        _commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
        _commandList.SetPipeline(_pipeline);

        _commandList.SetGraphicsResourceSet(0, _resourceSet);

        _commandList.DrawIndexed(
            indexCount: 4,
            instanceCount: 1,
            indexStart: 0,
            vertexOffset: 0,
            instanceStart: 0);

        _commandList.End();

        _graphicsDevice.SubmitCommands(_commandList);
        _graphicsDevice.SwapBuffers();
    }

    private static void CreateResources()
    {
        ResourceFactory factory = _graphicsDevice.ResourceFactory;

        VertexPositionColor[] quadVertices =
        {
            new VertexPositionColor(new Vector4(-0.75f, 0.75f, 0, 1), RgbaFloat.Red),
            new VertexPositionColor(new Vector4(0.75f, 0.75f, 0, 1), RgbaFloat.Green),
            new VertexPositionColor(new Vector4(-0.75f, -0.75f, 0, 1), RgbaFloat.Blue),
            new VertexPositionColor(new Vector4(0.75f, -0.75f, 0, 1), RgbaFloat.Yellow)
        };

        ushort[] quadIndices = { 0, 1, 2, 3 };

        _vertexBuffer = factory.CreateBuffer(new BufferDescription(4 * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
        _indexBuffer = factory.CreateBuffer(new BufferDescription(4 * sizeof(ushort), BufferUsage.IndexBuffer));

        _graphicsDevice.UpdateBuffer(_vertexBuffer, 0, quadVertices);
        _graphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);

        VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
            new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4),
            new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));

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

        pipelineDescription.DepthStencilState = DepthStencilStateDescription.Disabled; /*new DepthStencilStateDescription(
            depthTestEnabled: true,
            depthWriteEnabled: true,
            comparisonKind: ComparisonKind.LessEqual);*/

        pipelineDescription.RasterizerState = new RasterizerStateDescription(
            cullMode: FaceCullMode.None,
            fillMode: PolygonFillMode.Solid,
            frontFace: FrontFace.Clockwise,
            depthClipEnabled: true,
            scissorTestEnabled: false);

        pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
        //pipelineDescription.ResourceLayouts = System.Array.Empty<ResourceLayout>();

        pipelineDescription.ShaderSet = new ShaderSetDescription(
            vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
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
}

struct VertexPositionColor
{
    public Vector4 Position;
    public RgbaFloat Color;
    public VertexPositionColor(Vector4 position, RgbaFloat color)
    {
        Position = position;
        Color = color;
    }
    public const uint SizeInBytes = 32;
}

struct MatrixUniforms
{
    public Matrix4x4 Model;
    public Matrix4x4 View;
    public Matrix4x4 Projection;
}