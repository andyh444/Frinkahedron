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
        private readonly bool texturesEnabled;

        public VeldridRenderer(Primitives primitives, DeviceBuffer modelMatrixBuffer, CommandList commandList, AssetManager assets, bool texturesEnabled)
        {
            this.primitives = primitives;
            this.matricesBuffer = modelMatrixBuffer;
            this.commandList = commandList;
            this.assets = assets;
            this.texturesEnabled = texturesEnabled;
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
            ModelMatrixInfo modelInfo = new ModelMatrixInfo
            {
                Model = transform,
            };
            if (texturesEnabled)
            {
                commandList.SetGraphicsResourceSet(2, assets.GetTextureResourceSet(textureID));
            }
            commandList.UpdateBuffer(matricesBuffer, 0, ref modelInfo);
            meshInfo.Draw(commandList);
        }
    }
}
