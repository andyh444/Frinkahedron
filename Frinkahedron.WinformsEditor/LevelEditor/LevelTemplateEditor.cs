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
        private List<Action> selectedIndexChangedCallbacks = new List<Action>();

        public LevelTemplate Template
        {
            get;
            set
            {
                field = value;
                TemplateChanged();
            }
        } = new LevelTemplate();

        public int LevelObjectSelectedIndex
        {
            get;
            set
            {
                field = value;
                SelectedIndexChanged();
            }
        }

        public Unsubscriber RegisterTemplateChangedCallback(Action action) => RegisterCallback(action, templateChangedCallbacks);

        public Unsubscriber RegisterSelectedIndexChangedCallback(Action action) => RegisterCallback(action, selectedIndexChangedCallbacks);

        private Unsubscriber RegisterCallback(Action action, List<Action> callbacks)
        {
            callbacks.Add(action);
            return new Unsubscriber(() => callbacks.Remove(action));
        }

        public void TemplateChanged() => templateChangedCallbacks.ForEach(callback => callback());

        public void SelectedIndexChanged() => selectedIndexChangedCallbacks.ForEach(callback => callback());
    }
}
