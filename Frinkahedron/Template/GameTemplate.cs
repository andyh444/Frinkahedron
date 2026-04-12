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
    }
}
