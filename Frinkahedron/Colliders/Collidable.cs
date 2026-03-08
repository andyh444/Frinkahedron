using Frinkahedron.Core.Physics;

namespace Frinkahedron.Core.Colliders
{
    public readonly record struct Collidable<TShape>(Position Position, TShape Shape) where TShape : IShape;
}
