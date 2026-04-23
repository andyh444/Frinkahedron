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

        public GameObject ToGameObject(TransformTemplate worldTransform, IReadOnlyList<Behaviour> additionalBehaviours, int index)
        {
            // todo remove index

            DynamicBody? rigidBody = null;
            Behaviour behaviour = new CompositeBehaviour(additionalBehaviours);
            var collider = Collider?.ToShape();
            if (index == 0)
            {
                // box
                rigidBody = new DynamicBody { Gravity = false, Mass = float.PositiveInfinity, InverseInertia = new DiagonalMatrix3x3(), Material = new PhysicsMaterial(0.2f, 0.8f) };
            }
            else if (index == 1)
            {
                // car
                var mass = 1 * collider?.CalculateVolume() ?? 1;
                rigidBody = new DynamicBody { Mass = mass, Gravity = true, InverseInertia = collider?.CalculateFilledInertia(mass).GetInverse() ?? DiagonalMatrix3x3.Identity(), Material = new PhysicsMaterial(0.2f, 0.8f) };
                behaviour = new CompositeBehaviour([new CarCameraFollowBehaviour(), new CarBehaviour(), ..additionalBehaviours]);
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
