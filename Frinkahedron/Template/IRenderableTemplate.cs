using System.Numerics;
using System.Text.Json.Serialization;

namespace Frinkahedron.Core.Template
{
    [JsonPolymorphic()]
    [JsonDerivedType(typeof(ModelRenderableTemplate), nameof(ModelRenderableTemplate))]
    [JsonDerivedType(typeof(ModelEntitiesRenderableTemplate), nameof(ModelEntitiesRenderableTemplate))]
    [JsonDerivedType(typeof(ModelEntityRenderableTemplate), nameof(ModelEntityRenderableTemplate))]
    [JsonDerivedType(typeof(CompositeRenderableTemplate), nameof(CompositeRenderableTemplate))]
    public interface IRenderableTemplate
    {
        TransformTemplate Transform { get; set; }

        IRenderable ToRenderable();
    }

    public class ModelRenderableTemplate : IRenderableTemplate
    {
        public string ModelID { get; set; } = string.Empty;

        public TransformTemplate Transform { get; set; } = new TransformTemplate();

        public IRenderable ToRenderable() => new ModelRenderable(ModelID, Transform.ToMatrix());
    }

    public class ModelEntitiesRenderableTemplate : IRenderableTemplate
    {
        public string ModelID { get; set; } = string.Empty;

        public TransformTemplate Transform { get; set; } = new TransformTemplate();

        public bool[] EnabledIndices { get; set; } = [];

        public IRenderable ToRenderable()
        {
            var matrix = Transform.ToMatrix();
            List<IRenderable> renderables = new List<IRenderable>();
            
            for (int i = 0; i < EnabledIndices.Length; i++)
            {
                if (EnabledIndices[i])
                {
                    renderables.Add(new ModelEntityRenderable(ModelID, i, matrix));
                }
            }

            return new CompositeRenderable(renderables);
        }
    }

    public class ModelEntityRenderableTemplate : IRenderableTemplate
    {
        public string ModelID { get; set; } = string.Empty;

        public int Index { get; set; } = 0;

        public TransformTemplate Transform { get; set; } = new TransformTemplate();

        public IRenderable ToRenderable() => new ModelEntityRenderable(ModelID, Index, Transform.ToMatrix());
    }

    public class CompositeRenderableTemplate : IRenderableTemplate
    {
        private TransformTemplate transform;

        public List<IRenderableTemplate> Children { get; set; } = [];

        public TransformTemplate Transform
        {
            get => transform;
            set
            {
                transform = value;
                foreach (var child in Children)
                {
                    child.Transform = transform;
                }
            }
        }

        public IRenderable ToRenderable()
        {
            return new CompositeRenderable(Children.Select(x => x.ToRenderable()).ToList());
        }
    }
}
