using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Template
{
    [JsonPolymorphic]
    [JsonDerivedType(typeof(DynamicBodyTemplate), nameof(DynamicBodyTemplate))]
    [JsonDerivedType(typeof(StaticBodyTemplate), nameof(StaticBodyTemplate))]
    public interface IRigidBodyTemplate
    {
        IRigidBody ToRigidBody(IShape shape);
    }

    public class DynamicBodyTemplate : IRigidBodyTemplate
    {
        public float Density { get; set; }

        public float Elasticity { get; set; }

        public float CoefficientOfFriction { get; set; }

        public IRigidBody ToRigidBody(IShape shape)
        {
            float mass = Density * shape.CalculateVolume();
            return new DynamicBody
            {
                Mass = mass,
                InverseInertia = shape.CalculateFilledInertia(mass).GetInverse(),
                Gravity = true,
                Material = new PhysicsMaterial(Elasticity, CoefficientOfFriction)
            };
        }
    }

    public class StaticBodyTemplate : IRigidBodyTemplate
    {
        public float Elasticity { get; set; }

        public float CoefficientOfFriction { get; set; }

        public IRigidBody ToRigidBody(IShape shape)
        {
            return new StaticBody(new PhysicsMaterial(Elasticity, CoefficientOfFriction));
        }
    }
}
