using Frinkahedron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Frinkahedron.TestApp
{
    public class VeldridRenderer : IRenderer
    {
        private CommandList commandList;
        private GraphicsDevice graphicsDevice;
        private DeviceBuffer matricesBuffer;
        private MeshInfo cubeInfo;
        private Camera camera;

        public VeldridRenderer(CommandList commandList, MeshInfo cubeInfo, Camera camera, GraphicsDevice graphicsDevice, DeviceBuffer matricesBuffer)
        {
            this.commandList = commandList;
            this.cubeInfo = cubeInfo;
            this.camera = camera;
            this.graphicsDevice = graphicsDevice;
            this.matricesBuffer = matricesBuffer;
        }

        public void DrawCuboid(Matrix4x4 transform)
        {
            MatrixUniforms uniforms = new MatrixUniforms
            {
                Model = transform,
                View = camera.ViewMatrix,
                Projection = camera.ProjectionMatrix
            };
            graphicsDevice.UpdateBuffer(matricesBuffer, 0, ref uniforms);
            cubeInfo.Draw(commandList);
        }
    }
}
