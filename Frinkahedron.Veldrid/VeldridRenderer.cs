using Frinkahedron.Core;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
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
            //DrawMesh(primitives.CubeInfo, transform, "woodencontainer", "NeutralNormalMap", "NeutralMetallicRoughnessMap");

            var model = assets.GetModel("crate");
            DrawEntity(model.Entities[0], Matrix4x4.CreateScale(1f / 8f) * transform);
        }

        public void DrawCylinder(Matrix4x4 transform)
        {
            DrawMesh(primitives.CylinderInfo, transform, "woodencontainer", "NeutralNormalMap", "NeutralMetallicRoughnessMap");
        }

        public void DrawDisc(Matrix4x4 transform)
        {
            DrawMesh(primitives.DiscInfo, transform, "woodencontainer", "NeutralNormalMap", "NeutralMetallicRoughnessMap");
        }

        public void DrawEllipsoid(Matrix4x4 transform)
        {
            //DrawMesh(primitives.SphereInfo, transform, "football");

            var model = assets.GetModel("bowlingball");
            DrawEntity(model.Entities[0], transform);
        }

        private void DrawEntity(Entity entity, Matrix4x4 transform)
        {
            DrawMesh(entity.Mesh, transform, entity.ColourTexture, entity.NormalMap, entity.MetallicRoughnessMap);
        }
        private void DrawMesh(MeshInfo meshInfo, Matrix4x4 transform, string textureID, string normalID, string metallicRoughnessID)
        {
            DrawMesh(
            meshInfo,
            transform,
            texturesEnabled ? assets.GetTexture(textureID) : null,
            texturesEnabled ? assets.GetTexture(normalID) : null,
                texturesEnabled ? assets.GetTexture(metallicRoughnessID) : null);
        }

        private void DrawMesh(MeshInfo meshInfo, Matrix4x4 transform, TextureInfo? albedo, TextureInfo? normalMap, TextureInfo? metallicRoughnessMap)
        {
            ModelMatrixInfo modelInfo = new ModelMatrixInfo
            {
                Model = transform,
            };
            if (texturesEnabled)
            {
                if (albedo is not null)
                {
                    commandList.SetGraphicsResourceSet(2, albedo.ResourceSet);
                }
                if (normalMap is not null)
                {
                    commandList.SetGraphicsResourceSet(7, normalMap.ResourceSet);
                }
                if (metallicRoughnessMap is not null)
                {
                    commandList.SetGraphicsResourceSet(8, metallicRoughnessMap.ResourceSet);
                }
            }
            commandList.UpdateBuffer(matricesBuffer, 0, ref modelInfo);
            meshInfo.Draw(commandList);
        }
    }
}
