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
    public readonly struct PhysicsMaterial(float elasticity, float frictionCoefficient)
    {
        public float Elasticity { get; } = elasticity;

        public float FrictionCoefficient { get; } = frictionCoefficient;
    }

    public sealed class RigidBody
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

        public static bool ResolveCollision(
            in WorldRigidBody worldBodyA,
            in WorldRigidBody worldBodyB)
        {
            var positionA = worldBodyA.Position;
            var positionB = worldBodyB.Position;
            var bodyA = worldBodyA.RigidBody;
            var bodyB = worldBodyB.RigidBody;
            var shapeA = worldBodyA.Collider;
            var shapeB = worldBodyB.Collider;

            float inverseMassA = bodyA.InverseMass;
            float inverseMassB = bodyB.InverseMass;

            if (inverseMassA < 1e-3 && inverseMassB < 1e-3)
            {
                return false;
            }

            var manifold = CollisionPairTester.Test(positionA, shapeA, positionB, shapeB);
            if (manifold.Points.Length == 0)
            {
                return false;
            }

            float inverseMassSum = inverseMassA + inverseMassB;
            var inverseInertiaA = worldBodyA.InverseWorldInertia;
            var inverseInertiaB = worldBodyB.InverseWorldInertia;

            Vector3 normal = manifold.Normal;
            float penetration = manifold.Penetration;

            float e = MathF.Max(bodyA.Material.Elasticity, bodyB.Material.Elasticity);
            float frictionCoefficient = MathF.Sqrt(bodyA.Material.FrictionCoefficient * bodyB.Material.FrictionCoefficient);

            foreach (var contactPoint in manifold.Points)
            {
                
                // contact offsets (vector from center to contact point)
                Vector3 ra = contactPoint - positionA.Centre;
                Vector3 rb = contactPoint - positionB.Centre;

                // relative velocity at contact
                Vector3 va = bodyA.Velocity + Vector3.Cross(bodyA.AngularVelocity, ra);
                Vector3 vb = bodyB.Velocity + Vector3.Cross(bodyB.AngularVelocity, rb);
                Vector3 rv = va - vb;

                
                float speedAlongNormal = Vector3.Dot(rv, normal);
                if (speedAlongNormal < 0)
                {
                    
                    float j = -(1 + e) * speedAlongNormal;


                    Vector3 cross = Vector3.Cross(inverseInertiaA * Vector3.Cross(ra, normal), ra)
                        + Vector3.Cross(inverseInertiaB * Vector3.Cross(rb, normal), rb);

                    float denom = (inverseMassSum + Vector3.Dot(normal, cross)) * manifold.Points.Length;

                    j /= denom;
                    Vector3 impulse = j * normal;

                    bodyA.ApplyImpulse(impulse, ra, inverseInertiaA);
                    bodyB.ApplyImpulse(-impulse, rb, inverseInertiaB);

                    Vector3 tangent = rv - speedAlongNormal * normal;
                    float speedAlongTangent = tangent.Length();
                    if (speedAlongTangent != 0)
                    {
                        tangent /= speedAlongTangent;

                        float jt = -Vector3.Dot(rv, tangent);

                        Vector3 crossT = Vector3.Cross(inverseInertiaA * Vector3.Cross(ra, tangent), ra)
                            + Vector3.Cross(inverseInertiaB * Vector3.Cross(rb, tangent), rb);

                        float denomT = (inverseMassSum + Vector3.Dot(tangent, crossT)) * manifold.Points.Length;
                        jt /= denomT;

                        float maxFriction = j * frictionCoefficient;
                        jt = Math.Clamp(jt, -maxFriction, maxFriction);
                        Vector3 frictionImpulse = jt * tangent;

                        bodyA.ApplyImpulse(frictionImpulse, ra, inverseInertiaA);
                        bodyB.ApplyImpulse(-frictionImpulse, rb, inverseInertiaB);
                    }
                }
            }

            float correctionMag = penetration * 0.6f / (inverseMassA + inverseMassB);
            Vector3 correction = correctionMag * normal;

            if (float.IsNaN(correction.X))
            {
                Debugger.Break();
            }

            positionA.Centre += inverseMassA * correction;
            positionB.Centre -= inverseMassB * correction;

            return true;
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
    }
}
