using Frinkahedron.Core.Maths;
using System.Numerics;

namespace Frinkahedron.Core.Physics
{
    public enum RigidBodyType
    {
        Static,
        Dynamic,
    }

    public interface IRigidBody
    {
        Vector3 Velocity { get; }

        Vector3 AngularVelocity { get; }

        float Mass { get; }

        float InverseMass { get; }

        DiagonalMatrix3x3 InverseInertia { get; }

        PhysicsMaterial Material { get; }

        RigidBodyType RigidBodyType { get; }

        public void IntegratePosition(float deltaTime, Position position);

        public void ApplyImpulse(Vector3 impulse, Vector3 contactVector, Position position);
        public void ApplyTorque(Vector3 torque, float dt, Position position);
        Matrix3x3 InverseWorldInertia(Quaternion orientation);
        void ApplyImpulse(Vector3 impulse, Vector3 ra, Matrix3x3 inverseInertiaA);
    }
}
