using System.Numerics;

namespace Frinkahedron.Core.Template
{
    public interface IRenderableTemplate
    {
        public void TrySetTransform(Matrix4x4 newTransform);

        IRenderable ToRenderable();
    }

    public class ModelRenderableTemplate : IRenderableTemplate
    {
        public string ModelID { get; set; } = string.Empty;

        public Matrix4x4 Transform { get; set; } = Matrix4x4.Identity;

        public IRenderable ToRenderable() => new ModelRenderable(ModelID, Transform);

        public void TrySetTransform(Matrix4x4 newTransform)
        {
            Transform = newTransform;
        }
    }
}
