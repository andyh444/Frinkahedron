using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core
{
    public interface IRenderable
    {
        void Render(IRenderContext renderContext, Matrix4x4 worldTransform);
    }

    public class CompositeRenderable(IReadOnlyList<IRenderable> renderables) : IRenderable
    {
        public void Render(IRenderContext renderContext, Matrix4x4 worldTransform)
        {
            foreach (IRenderable renderable in renderables)
            {
                renderable.Render(renderContext, worldTransform);
            }
        }
    }

    public class ModelRenderable(string modelID, Matrix4x4 modelTransform) : IRenderable
    {
        public string ModelID { get; } = modelID;

        public void Render(IRenderContext renderContext, Matrix4x4 worldTransform)
        {
            renderContext.DrawModel(ModelID, modelTransform * worldTransform);
        }
    }

    public class ModelEntityRenderable(string modelID, int entityIndex, Matrix4x4 modelTransform) : IRenderable
    {
        public string ModelID { get; } = modelID;

        public int EntityIndex { get; } = entityIndex;

        public void Render(IRenderContext renderContext, Matrix4x4 worldTransform)
        {
            renderContext.DrawModelEntity(ModelID, EntityIndex, modelTransform * worldTransform);
        }
    }
}
