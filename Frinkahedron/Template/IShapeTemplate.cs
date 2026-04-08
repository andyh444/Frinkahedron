using Frinkahedron.Core.Colliders;
using System.Numerics;

namespace Frinkahedron.Core.Template
{
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
