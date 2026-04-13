using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frinkahedron.WinformsEditor.LevelEditor
{
    public partial class LevelEditorControl : UserControl
    {
        private LevelTemplateEditor levelEditor;

        public LevelEditorControl()
        {
            InitializeComponent();
            levelEditor = new LevelTemplateEditor();
        }

        public void Initialise(GameTemplateEditor gameEditor, GraphicsService graphicsService)
        {
            levelViewerControl1.Initialise(gameEditor, levelEditor, graphicsService);
        }
    }
}
