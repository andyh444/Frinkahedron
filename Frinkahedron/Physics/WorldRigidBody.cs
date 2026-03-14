using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Physics
{
    public readonly struct WorldRigidBody
    {
        public Position Position { get; }

        public RigidBody RigidBody { get; }

        public IShape Collider { get; }

        public Matrix3x3 InverseWorldInertia { get; }

        public AxisAlignedBoundingBox BoundingBox { get; }

        public WorldRigidBody(Position position, RigidBody rigidBody, IShape collider)
        {
            Position = position;
            RigidBody = rigidBody;
            Collider = collider;
            InverseWorldInertia = rigidBody.InverseWorldInertia(position.Orientation);
            BoundingBox = collider.CalculateAABB(position);
        }
    }
}
