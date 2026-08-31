using Frinkahedron.Core.Colliders;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Frinkahedron.Core.Template
{
    [JsonPolymorphic]
    [JsonDerivedType(typeof(BoxTemplate), nameof(BoxTemplate))]
    [JsonDerivedType(typeof(SphereTemplate), nameof(SphereTemplate))]
    public interface IShapeTemplate
    {
        IShape ToShape();
    }

    public class BoxTemplate : IShapeTemplate
    {
        public Vector3 Dimensions { get; set; }

        public IShape ToShape() => new Box(Dimensions);
    }

    public class SphereTemplate : IShapeTemplate
    {
        public float Radius { get; set; }

        public IShape ToShape() => new Sphere(Radius);
    }
}
