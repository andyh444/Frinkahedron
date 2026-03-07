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

        public Matrix3x3 InverseWorldInertia(Quaternion orientation)
        {
            Matrix4x4 rot4 = Matrix4x4.CreateFromQuaternion(orientation);

            Matrix3x3 R = new Matrix3x3(
                new Vector3(rot4.M11, rot4.M12, rot4.M13),
                new Vector3(rot4.M21, rot4.M22, rot4.M23),
                new Vector3(rot4.M31, rot4.M32, rot4.M33));

            Matrix3x3 Rt = R.Transpose();

            var worldInertia = R * Inertia * Rt;
            return worldInertia.GetInverse();
        }

        public bool HasInfiniteInertia => float.IsPositiveInfinity(Inertia.Row1.X);

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

            foreach (var contactPoint in manifold.Points)
            {
                Vector3 normal = manifold.Normal;
                float penetration = manifold.Penetration;

                // contact offsets (vector from center to contact point)
                Vector3 ra = contactPoint - thisPosition.Centre;
                Vector3 rb = contactPoint - otherPosition.Centre;

                // relative velocity at contact
                Vector3 va = Velocity + Vector3.Cross(AngularVelocity, ra);
                Vector3 vb = other.Velocity + Vector3.Cross(other.AngularVelocity, rb);
                Vector3 rv = va - vb;

                float inverseMass = this.InverseMass;
                float otherInverseMass = other.InverseMass;

                float speedAlongNormal = Vector3.Dot(rv, normal);
                if (speedAlongNormal < 0)
                {
                    float e = 0.8f; // Coefficient of restitution (elasticity), adjust as needed
                    float j = -(1 + e) * speedAlongNormal;

                    float denom = inverseMass + otherInverseMass;

                    Vector3 cross = new Vector3();
                    if (!HasInfiniteInertia)
                    {
                        cross += Vector3.Cross(InverseWorldInertia(thisPosition.Orientation) * Vector3.Cross(ra, normal), ra);
                    }
                    if (!other.HasInfiniteInertia)
                    {
                        cross += Vector3.Cross(other.InverseWorldInertia(otherPosition.Orientation) * Vector3.Cross(rb, normal), rb);
                    }
                    denom += Vector3.Dot(normal, cross);

                    j /= denom;
                    Vector3 impulse = j * normal;

                    ApplyImpulse(impulse, ra, thisPosition.Orientation);
                    other.ApplyImpulse(-impulse, rb, otherPosition.Orientation);

                    Vector3 tangent = rv - speedAlongNormal * normal;
                    float speedAlongTangent = tangent.Length();
                    if (speedAlongTangent != 0)
                    {
                        tangent /= speedAlongTangent;

                        float jt = -Vector3.Dot(rv, tangent);
                        float denomT = inverseMass + otherInverseMass;

                        Vector3 crossT = new Vector3();
                        if (!HasInfiniteInertia)
                        {
                            crossT += Vector3.Cross(InverseWorldInertia(thisPosition.Orientation) * Vector3.Cross(ra, tangent), ra);
                        }
                        if (!other.HasInfiniteInertia)
                        {
                            crossT += Vector3.Cross(other.InverseWorldInertia(otherPosition.Orientation) * Vector3.Cross(rb, tangent), rb);
                        }
                        denomT += Vector3.Dot(tangent, crossT);

                        jt /= denomT;

                        float frictionCoefficient = 1.6f;// 0.8f;
                        float maxFriction = j * frictionCoefficient;
                        jt = Math.Clamp(jt, -maxFriction, maxFriction);
                        Vector3 frictionImpulse = jt * tangent;

                        ApplyImpulse(frictionImpulse, ra, thisPosition.Orientation);
                        other.ApplyImpulse(-frictionImpulse, rb, otherPosition.Orientation);

                    }
                }

                float correctionMag = penetration * 1.6f / (inverseMass + otherInverseMass);
                Vector3 correction = correctionMag * normal;

                thisPosition.Centre += inverseMass * correction;
                otherPosition.Centre -= otherInverseMass * correction;
            }
            return true;
        }

        public void ApplyImpulse(Vector3 impulse, Vector3 contactVector, Quaternion orientation)
        {
            Velocity += InverseMass * impulse;
            if (!HasInfiniteInertia)
            {
                AngularVelocity += InverseWorldInertia(orientation) * Vector3.Cross(contactVector, impulse);
            }
        }
    }
}
