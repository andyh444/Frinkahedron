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
    internal sealed class VeldridRenderer : IRenderer
    {
        private readonly Primitives primitives;
        private readonly DeviceBuffer matricesBuffer;
        private readonly CommandList commandList;
        private readonly AssetManager assets;
        private Camera camera;

        public VeldridRenderer(Primitives primitives, DeviceBuffer matricesBuffer, CommandList commandList, AssetManager assets, Camera camera)
        {
            this.primitives = primitives;
            this.matricesBuffer = matricesBuffer;
            this.commandList = commandList;
            this.assets = assets;
            this.camera = camera;
        }

        public void DrawCuboid(Matrix4x4 transform)
        {
            DrawMesh(primitives.CubeInfo, transform, "woodencontainer");
        }

        public void DrawCylinder(Matrix4x4 transform)
        {
            DrawMesh(primitives.CylinderInfo, transform, "woodencontainer");
        }

        public void DrawEllipsoid(Matrix4x4 transform)
        {
            DrawMesh(primitives.SphereInfo, transform, "football");
        }

        private void DrawMesh(MeshInfo meshInfo, Matrix4x4 transform, string textureID)
        {
            MatrixUniforms uniforms = new MatrixUniforms
            {
                Model = transform,
                View = camera.ViewMatrix,
                Projection = camera.ProjectionMatrix
            };
            commandList.SetGraphicsResourceSet(1, assets.GetTextureResourceSet(textureID));
            commandList.UpdateBuffer(matricesBuffer, 0, ref uniforms);
            meshInfo.Draw(commandList);
        }
    }
}
