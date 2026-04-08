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

        public GameObject ToGameObject(Vector3 initialPosition, IReadOnlyList<Behaviour> additionalBehaviours)
        {
            return new GameObject(
                initialPosition,
                behaviour: new CompositeBehaviour(additionalBehaviours),
                colliderShape: Collider?.ToShape(),
                rigidBody: null,
                renderable: Renderable?.ToRenderable());
        }
    }
}
