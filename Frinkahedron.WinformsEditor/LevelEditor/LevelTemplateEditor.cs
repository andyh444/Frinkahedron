using Frinkahedron.Core.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.WinformsEditor.LevelEditor
{
    public sealed class LevelTemplateEditor
    {
        public LevelTemplate Template
        {
            get;
            set
            {
                field = value;
                TemplateChangedCallback?.Invoke();
            }
        } = new LevelTemplate();

        public Action? TemplateChangedCallback { get; set; }

    }
}
