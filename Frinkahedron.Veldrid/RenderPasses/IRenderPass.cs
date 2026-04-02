using Frinkahedron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using static Frinkahedron.VeldridImplementation.VeldridRenderContext;

namespace Frinkahedron.VeldridImplementation.RenderPasses
{
    public interface IRenderPass : IDisposable
    {
        public void RenderScene(GraphicsDevice graphicsDevice, CommandList commandList, GraphicsResources graphicsResources, Scene scene, IReadOnlyList<DrawInstruction> sceneDrawInstructions);
    }
}
