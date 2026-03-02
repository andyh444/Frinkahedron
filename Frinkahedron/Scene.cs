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