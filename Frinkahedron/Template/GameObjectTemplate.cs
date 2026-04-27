using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Template
{
    public class GameObjectTemplate
    {
        public IShapeTemplate? Collider { get; set; }

        public IRenderableTemplate? Renderable { get; set; }

        public IRigidBodyTemplate? RigidBody { get; set; }

        public GameObject ToGameObject(TransformTemplate worldTransform, IReadOnlyList<Behaviour> additionalBehaviours, int index)
        {
            // todo remove index

            Behaviour behaviour = new CompositeBehaviour(additionalBehaviours);
            var collider = Collider?.ToShape();
            var rigidBody = RigidBody?.ToRigidBody(collider);
            if (index == 1)
            {
                behaviour = new CompositeBehaviour([new CarCameraFollowBehaviour(), new CarBehaviour(), .. additionalBehaviours]);
            }

            var obj = new GameObject(
                worldTransform.Translation,
                behaviour: behaviour,
                colliderShape: collider,
                rigidBody: rigidBody,
                renderable: Renderable?.ToRenderable());

            var rot = worldTransform.RotationEulerAngles;
            obj.Position.Orientation = Quaternion.CreateFromYawPitchRoll(rot.Y, rot.X, rot.Z);

            // TODO: World transform scale?

            return obj;
        }
    }
}
