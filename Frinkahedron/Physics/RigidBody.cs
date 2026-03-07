using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Physics
{
    public sealed class RigidBody
    {
        public Vector3 Velocity { get; set; }

        public Vector3 AngularVelocity { get; set; }

        public required float Mass { get; init; }

        public Matrix3x3 Inertia { get; set; }

        public bool Gravity { get; set; }

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

        public bool ResolveCollision(Position thisPosition, ICollider thisCollider, RigidBody other, Position otherPosition, ICollider otherCollider)
        {
            var manifold = thisCollider.CheckForCollisions(thisPosition, otherCollider, otherPosition);
            if (manifold.Points.Length == 0)
            {
                return false;
            }

            Vector3 contactPoint = manifold.Points.Single(); // TODO: Handle multiple points later
            Vector3 normal = manifold.Normal;
            float penetration = manifold.Penetration;

            // contact offsets (vector from center to contact point)
            Vector3 ra = contactPoint - thisPosition.Centre;
            Vector3 rb = contactPoint - otherPosition.Centre;

            // relative velocity at contact
            Vector3 va = Velocity; // + AngularVelocity * ra.GetPerpendicular();
            Vector3 vb = other.Velocity; // + other.AngularVelocity * rb.GetPerpendicular();
            Vector3 rv = va - vb;

            float inverseMass = this.InverseMass;
            float otherInverseMass = other.InverseMass;

            float speedAlongNormal = Vector3.Dot(rv, normal);
            if (speedAlongNormal < 0)
            {
                float e = 0.8f; // Coefficient of restitution (elasticity), adjust as needed
                float j = -(1 + e) * speedAlongNormal;
                j /= inverseMass
                    + otherInverseMass
                    /* + Vector3.Dot(normal, Vector3.Cross(Inertia * Vector3.Cross(ra, normal), ra)
                    + Vector3.Cross(other.Inertia * Vector3.Cross(rb, normal), rb))*/;
                Vector3 impulse = j * normal;

                ApplyImpulse(impulse, ra);
                other.ApplyImpulse(-impulse, rb);
            }

            float correctionMag = penetration * 0.8f / (inverseMass + otherInverseMass);
            Vector3 correction = correctionMag * normal;

            thisPosition.Centre += inverseMass * correction;
            otherPosition.Centre -= otherInverseMass * correction;

            return true;
        }

        public void ApplyImpulse(Vector3 impulse, Vector3 contactVector)
        {
            Velocity += InverseMass * impulse;
            // TODO: Angular velocity
        }
    }
}
