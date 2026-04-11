using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frinkahedron.WinformsEditor.GameObjectEditor
{
    public partial class GameObjectEditorControl : UserControl
    {
        private GameObjectTemplateEditor editor;

        public GameObjectEditorControl()
        {
            InitializeComponent();
            editor = new GameObjectTemplateEditor();
            veldridControl1.Initialise(editor);
            renderableTemplateControl1.Initialise(editor);
            colliderControl1.Initialise(editor);
        }
    }
}
