using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Template
{

    public sealed class GameTemplate
    {
        public List<ModelTemplate> Models { get; set; }

        public List<GameObjectTemplate> GameObjects { get; set; }

        public List<LevelTemplate> Levels { get; set; }

        public GameTemplate()
            :this(new List<ModelTemplate>(), new List<GameObjectTemplate>(), new List<LevelTemplate>())
        {
            
        }

        public GameTemplate(List<ModelTemplate> models, List<GameObjectTemplate> gameObjects, List<LevelTemplate> levels)
        {
            Models = models;
            GameObjects = gameObjects;
            Levels = levels;
        }
    }

    public sealed class LevelTemplate
    {
        public List<LevelObjectTemplate> LevelObjects { get; set; } = new List<LevelObjectTemplate>();

        public Scene ToScene(GameTemplate gameTemplate, Vector3 initialCameraPosition, Vector3 initialCameraDirection, float cameraAspectRatio)
        {
            List<GameObject> gameObjects = new List<GameObject>();
            int index = 0;
            foreach (var levelObj in LevelObjects)
            {
                var gameObjTemplate = gameTemplate.GameObjects[levelObj.GameObjectIndex];
                var gameObj = gameObjTemplate.ToGameObject(levelObj.WorldTransform, [], levelObj.GameObjectIndex);
                gameObjects.Add(gameObj);
            }
            var scene = new Scene(initialCameraPosition, initialCameraDirection, cameraAspectRatio, gameObjects);
            scene.SceneLights.DirectionalLight = new DirectionalLight(Vector3.Normalize(new Vector3(-1)), new Vector3(1));
            return scene;
        }
    }

    public sealed class LevelObjectTemplate
    {
        // TODO: Replace this with an ID for the object
        public int GameObjectIndex { get; set; }

        public TransformTemplate WorldTransform { get; set; } = new TransformTemplate();
    }
}
