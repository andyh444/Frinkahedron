using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core
{
    public class SceneBuilder
    {
        private List<GameObject> objects;

        public SceneBuilder()
        {
            objects = new List<GameObject>();
        }

        public void AddObject(GameObject obj) => objects.Add(obj);

        public Scene ToScene(Vector3 initialCamPosition, Vector3 initialCamDirection, float cameraAspectRatio)
        {
            return new Scene(initialCamPosition, initialCamDirection, cameraAspectRatio, objects);
        }
    }
}
