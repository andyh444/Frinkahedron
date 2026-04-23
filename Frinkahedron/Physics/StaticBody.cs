using Frinkahedron.Core.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Physics
{
    public sealed class StaticBody(PhysicsMaterial material) : IRigidBody
    {
        public float Mass => float.PositiveInfinity;

        public float InverseMass => 0f;

        public DiagonalMatrix3x3 InverseInertia => new DiagonalMatrix3x3();

        public PhysicsMaterial Material { get; } = material;

        public Vector3 Velocity => Vector3.Zero;

        public Vector3 AngularVelocity => Vector3.Zero;

        public RigidBodyType RigidBodyType => RigidBodyType.Static;

        public void ApplyImpulse(Vector3 impulse, Vector3 contactVector, Position position)
        {
        }

        public void ApplyImpulse(Vector3 impulse, Vector3 ra, Matrix3x3 inverseInertiaA)
        {
        }

        public void ApplyTorque(Vector3 torque, float dt, Position position)
        {
        }

        public void IntegratePosition(float deltaTime, Position position)
        {
            // do nothing - static bodies never move
        }

        public Matrix3x3 InverseWorldInertia(Quaternion orientation) => new Matrix3x3();
    }
}
