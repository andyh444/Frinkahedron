using Frinkahedron.Core.Colliders;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Frinkahedron.Core.Template
{
    [JsonPolymorphic]
    [JsonDerivedType(typeof(BoxTemplate), nameof(BoxTemplate))]
    public interface IShapeTemplate
    {
        IShape ToShape();
    }

    public class BoxTemplate : IShapeTemplate
    {
        public Vector3 Dimensions { get; set; }

        public IShape ToShape() => new Box(Dimensions);
    }
}
