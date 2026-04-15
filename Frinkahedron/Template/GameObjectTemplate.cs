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

        public GameObject ToGameObject(TransformTemplate worldTransform, IReadOnlyList<Behaviour> additionalBehaviours)
        {
            var obj = new GameObject(
                worldTransform.Translation,
                behaviour: new CompositeBehaviour(additionalBehaviours),
                colliderShape: Collider?.ToShape(),
                rigidBody: null,
                renderable: Renderable?.ToRenderable());

            var rot = worldTransform.RotationEulerAngles;
            obj.Position.Orientation = Quaternion.CreateFromYawPitchRoll(rot.Y, rot.X, rot.Z);

            // TODO: World transform scale?

            return obj;
        }
    }
}
