using Frinkahedron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Frinkahedron.VeldridImplementation
{
    public sealed class VeldridRenderer : IRenderer
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

        public void DrawDisc(Matrix4x4 transform)
        {
            DrawMesh(primitives.DiscInfo, transform, "woodencontainer");
        }

        public void DrawEllipsoid(Matrix4x4 transform)
        {
            //DrawMesh(primitives.SphereInfo, transform, "football");

            var model = assets.GetModel("bowlingball");
            DrawMesh(model.Entities[0].Mesh, transform, model.Entities[0].ColourTexture);
        }

        private void DrawMesh(MeshInfo meshInfo, Matrix4x4 transform, string textureID)
        {
            DrawMesh(meshInfo, transform, texturesEnabled ? assets.GetTexture(textureID) : null);
        }

        private void DrawMesh(MeshInfo meshInfo, Matrix4x4 transform, TextureInfo? texture)
        {
            ModelMatrixInfo modelInfo = new ModelMatrixInfo
            {
                Model = transform,
            };
            if (texture is not null && texturesEnabled)
            {
                commandList.SetGraphicsResourceSet(2, texture.ResourceSet);
            }
            commandList.UpdateBuffer(matricesBuffer, 0, ref modelInfo);
            meshInfo.Draw(commandList);
        }
    }
}
