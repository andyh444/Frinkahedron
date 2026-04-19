using Frinkahedron.Core;
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
        private List<Action> templateChangedCallbacks = new List<Action>();

        public LevelTemplate Template
        {
            get;
            set
            {
                field = value;
                TemplateChanged();
            }
        } = new LevelTemplate();

        public Unsubscriber RegisterTemplateChangedCallback(Action action)
        {
            templateChangedCallbacks.Add(action);
            return new Unsubscriber(() => templateChangedCallbacks.Remove(action));
        }

        public void TemplateChanged()
        {
            foreach (var callback in templateChangedCallbacks)
            {
                callback();
            }
        }
    }
}
