using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Template
{

    public sealed class GameTemplate
    {
        public List<ModelTemplate> Models { get; } = new List<ModelTemplate>();

        public List<GameObjectTemplate> GameObjects { get; } = new List<GameObjectTemplate>();

        public List<LevelTemplate> Levels { get; } = new List<LevelTemplate>();
    }

    public sealed class LevelTemplate
    {
        public List<LevelObjectTemplate> LevelObjects { get; } = new List<LevelObjectTemplate>();
    }

    public sealed class LevelObjectTemplate
    {
        // TODO: Replace this with an ID for the object
        public int GameObjectIndex { get; set; }

        public TransformTemplate WorldTransform { get; set; } = new TransformTemplate();
    }
}
