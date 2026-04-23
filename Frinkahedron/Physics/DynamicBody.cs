using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Physics
{

    public sealed class DynamicBody : IRigidBody
    {
        public Vector3 Velocity { get; set; }

        public Vector3 AngularVelocity { get; set; }

        public required float Mass { get; init; }

        /// <summary>
        /// Inverse body inertia tensor
        /// </summary>
        public DiagonalMatrix3x3 InverseInertia { get; set; }

        public bool Gravity { get; set; }

        public PhysicsMaterial Material { get; init; } = new PhysicsMaterial(0.6f, 0.8f);

        public float InverseMass
        {
            get
            {
                if (float.IsPositiveInfinity(Mass))
                {
                    return 0;
                }
                return 1 / Mass;
            }
        }

        public RigidBodyType RigidBodyType => RigidBodyType.Dynamic;

        public Matrix3x3 InverseWorldInertia(Quaternion orientation)
        {
            Matrix3x3 R = Matrix3x3.CreateFromQuaternion(orientation);
            Matrix3x3 Rt = R.Transpose();

            return R * InverseInertia * Rt;
        }

        public void IntegratePosition(float deltaTime, Position position)
        {
            position.Centre += deltaTime * Velocity;
            float angle = AngularVelocity.Length() * deltaTime;
            if (angle > 0)
            {
                Vector3 axis = Vector3.Normalize(AngularVelocity);
                Quaternion deltaRot = Quaternion.CreateFromAxisAngle(axis, angle);
                position.Orientation = Quaternion.Normalize(deltaRot * position.Orientation);
            }
            if (Gravity)
            {
                Velocity += deltaTime * new Vector3(0, -9.81f, 0);
            }
        }

        public void ApplyImpulse(Vector3 impulse, Vector3 contactVector, Position position)
        {
            var inverseWorldInertia = InverseWorldInertia(position.Orientation);
            ApplyImpulse(impulse, contactVector, inverseWorldInertia);
        }

        private void ApplyImpulse(Vector3 impulse, Vector3 contactVector, Matrix3x3 inverseWorldInertia)
        {
            Velocity += InverseMass * impulse;
            AngularVelocity += inverseWorldInertia * Vector3.Cross(contactVector, impulse);
        }

        public void ApplyTorque(Vector3 torque, float dt, Position position)
        {
            var inverseWorldInertia = InverseWorldInertia(position.Orientation);
            Vector3 angularAcceleration = inverseWorldInertia * torque;
            AngularVelocity += angularAcceleration * dt;
        }

        void IRigidBody.ApplyImpulse(Vector3 impulse, Vector3 ra, Matrix3x3 inverseInertiaA)
        {
            ApplyImpulse(impulse, ra, inverseInertiaA);
        }
    }
}
