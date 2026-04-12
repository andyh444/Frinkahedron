using Frinkahedron.Core.Template;
using Frinkahedron.VeldridImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.WinformsEditor.GameObjectEditor
{
    public readonly record struct ModelInfo(Model Model, string ModelID);

    public sealed class GameObjectTemplateEditor
    {
        public GameObjectTemplate Template
        {
            get;
            set
            {
                field = value;
                TemplateChangedCallback?.Invoke();
            }
        }

        public Action? TemplateChangedCallback { get; set; }

        public Func<string, ModelInfo?>? LoadModelFunc { get; set; }

        public GameObjectTemplateEditor()
        {
            Template = new GameObjectTemplate();
        }
    }
}
