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

    public class ModelRenderable(string modelID, Matrix4x4 modelTransform, bool[]? enabledEntities = null) : IRenderable
    {
        public string ModelID { get; } = modelID;

        public bool[]? EnabledEntities { get; } = enabledEntities;

        public void Render(IRenderContext renderContext, Matrix4x4 worldTransform)
        {
            renderContext.DrawModel(ModelID, modelTransform * worldTransform, EnabledEntities);
        }
    }
}
