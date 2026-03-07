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
        private GraphicsResources graphicsResources;
        private Camera camera;

        public VeldridRenderer(GraphicsResources graphicsResources, Camera camera)
        {
            this.graphicsResources = graphicsResources;
            this.camera = camera;
        }

        public void DrawCuboid(Matrix4x4 transform)
        {
            DrawMesh(graphicsResources.CubeInfo, transform);
        }

        public void DrawEllipsoid(Matrix4x4 transform)
        {
            DrawMesh(graphicsResources.SphereInfo, transform);
        }

        private void DrawMesh(MeshInfo meshInfo, Matrix4x4 transform)
        {
            MatrixUniforms uniforms = new MatrixUniforms
            {
                Model = transform,
                View = camera.ViewMatrix,
                Projection = camera.ProjectionMatrix
            };
            graphicsResources.CommandList.UpdateBuffer(graphicsResources.MatricesBuffer, 0, ref uniforms);
            meshInfo.Draw(graphicsResources.CommandList);
        }
    }
}
