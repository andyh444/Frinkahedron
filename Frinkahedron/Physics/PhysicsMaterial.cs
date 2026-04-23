namespace Frinkahedron.Core.Physics
{
    public readonly struct PhysicsMaterial(float elasticity, float frictionCoefficient)
    {
        public float Elasticity { get; } = elasticity;

        public float FrictionCoefficient { get; } = frictionCoefficient;
    }
}
