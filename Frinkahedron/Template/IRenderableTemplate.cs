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

    public class ModelEntityRenderableTemplate : IRenderableTemplate
    {
        public string ModelID { get; set; } = string.Empty;

        public int Index { get; set; } = 0;

        public Matrix4x4 Transform { get; set; } = Matrix4x4.Identity;

        public IRenderable ToRenderable() => new ModelEntityRenderable(ModelID, Index, Transform);

        public void TrySetTransform(Matrix4x4 newTransform)
        {
            Transform = newTransform;
        }
    }

    public class CompositeRenderableTemplate : IRenderableTemplate
    {
        public List<IRenderableTemplate> Children { get; set; } = [];

        public IRenderable ToRenderable()
        {
            return new CompositeRenderable(Children.Select(x => x.ToRenderable()).ToList());
        }

        public void TrySetTransform(Matrix4x4 newTransform)
        {
            foreach (var child in Children)
            {
                child.TrySetTransform(newTransform);
            }
        }
    }
}
