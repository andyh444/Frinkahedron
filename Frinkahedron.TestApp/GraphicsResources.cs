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
        
        public required Primitives Primitives { get; init; }

        public required MainRenderPass MainRenderPass { get; init; }

        public static GraphicsResources CreateResources(GraphicsDevice graphicsDevice)
        {
            ResourceFactory factory = graphicsDevice.ResourceFactory;
            return new GraphicsResources
            {
                CommandList = factory.CreateCommandList(),
                Primitives = Primitives.Create(graphicsDevice),
                MainRenderPass = MainRenderPass.Create(factory, graphicsDevice)
            };
        }

        public void Dispose()
        {
            CommandList.Dispose();
            Primitives.Dispose();
            MainRenderPass.Dispose();
        }
    }
}
