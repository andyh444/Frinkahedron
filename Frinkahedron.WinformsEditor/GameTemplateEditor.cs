using Frinkahedron.Core.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.WinformsEditor
{
    public sealed class GameTemplateEditor
    {
        public GameTemplate Template { get; }

        public GameTemplateEditor()
            :this(new GameTemplate())
        {
        }

        public GameTemplateEditor(GameTemplate template)
        {
            Template = template;
        }
    }
}
