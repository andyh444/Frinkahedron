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
    public sealed class VeldridRenderContext : IRenderContext
    {
        public readonly struct DrawInstruction
        {
            public required string ModelID { get; init; }
            public required Matrix4x4 Transform { get; init; }
        };

        private readonly List<DrawInstruction> drawInstructions;

        public IReadOnlyList<DrawInstruction> DrawInstructions => drawInstructions;

        public VeldridRenderContext()
        {
            drawInstructions = new List<DrawInstruction>();
        }

        public void DrawCuboid(Matrix4x4 transform)
        {
            //DrawMesh(primitives.CubeInfo, transform, "woodencontainer", "NeutralNormalMap", "NeutralMetallicRoughnessMap");

            //var model = assets.GetModel("crate");
            //DrawEntity(model.Entities[0], Matrix4x4.CreateScale(1f / 8f) * transform);
        }

        public void DrawCylinder(Matrix4x4 transform)
        {
            //DrawMesh(primitives.CylinderInfo, transform, "woodencontainer", "NeutralNormalMap", "NeutralMetallicRoughnessMap");

            //var model = assets.GetModel("tincan");
            //DrawEntity(model.Entities[0], Matrix4x4.CreateRotationX(-MathF.PI / 2) * Matrix4x4.CreateScale(1 / 0.053f, 1 / 0.158f, 1 / 0.053f) * transform);
        }

        public void DrawDisc(Matrix4x4 transform)
        {
           // DrawMesh(primitives.DiscInfo, transform, "woodencontainer", "NeutralNormalMap", "NeutralMetallicRoughnessMap");
        }

        public void DrawEllipsoid(Matrix4x4 transform)
        {
            //DrawMesh(primitives.SphereInfo, transform, "football", "NeutralNormalMap", "NeutralMetallicRoughnessMap");
        }

        public void DrawModel(string modelID, Matrix4x4 transform)
        {
            drawInstructions.Add(new DrawInstruction
            {
                ModelID = modelID,
                Transform = transform
            });
            //var model = assets.GetModel(modelID);
            //DrawEntity(model.Entities[0], transform); // TODO: All entities
        }

        /*private void DrawEntity(Entity entity, Matrix4x4 transform)
        {
            DrawMesh(entity.Mesh, transform, entity.ColourTexture, entity.NormalMap, entity.MetallicRoughnessMap);
        }*/

        /*private void DrawMesh(MeshInfo meshInfo, Matrix4x4 transform, string textureID, string normalID, string metallicRoughnessID)
        {
            DrawMesh(
            meshInfo,
            transform,
            texturesEnabled ? assets.GetTexture(textureID) : null,
            texturesEnabled ? assets.GetTexture(normalID) : null,
                texturesEnabled ? assets.GetTexture(metallicRoughnessID) : null);
        }*/

        /*private void DrawMesh(MeshInfo meshInfo, Matrix4x4 transform, TextureInfo? albedo, TextureInfo? normalMap, TextureInfo? metallicRoughnessMap)
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
        }*/
    }
}
