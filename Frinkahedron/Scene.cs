using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core
{
    public sealed class Scene
    {
        public Camera Camera { get; }

        public IReadOnlyList<GameObject> Objects { get; }

        public Scene(Vector3 initialCameraPosition, Vector3 initialCameraDirection, IReadOnlyList<GameObject> objects)
        {
            Camera = new Camera(initialCameraPosition, initialCameraDirection);
            Objects = objects;
        }

        public void Update(GameState gameState)
        {
            foreach (var obj in Objects)
            {
                obj.Update(gameState);
            }

            // resolve collisions
            for (int i = 0; i < Objects.Count; i++)
            {
                var object1 = Objects[i];
                if (object1.Collider is not null
                    && object1.RigidBody is not null)
                {
                    for (int j = i + 1; j < Objects.Count; j++)
                    {
                        var object2 = Objects[j];
                        if (object2.Collider is not null
                            && object2.RigidBody is not null)
                        {
                            object1.RigidBody.ResolveCollision(
                                object1.Position,
                                object1.Collider,
                                object2.RigidBody,
                                object2.Position,
                                object2.Collider);
                        }
                    }
                }
            }
        }

        public void Draw(IRenderer renderer)
        {
            foreach (var obj in Objects)
            {
                obj.Draw(renderer);
            }
        }
    }
}