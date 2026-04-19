using Frinkahedron.Core.Template;
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
        private GameTemplateEditor? gameEditor;

        public LevelEditorControl()
        {
            InitializeComponent();
            levelEditor = new LevelTemplateEditor();
        }

        public void Initialise(GameTemplateEditor gameEditor, GraphicsService graphicsService)
        {
            this.gameEditor = gameEditor;
            levelViewerControl1.Initialise(gameEditor, levelEditor, graphicsService, () => objectSelectionBox.SelectedIndex);

            IsShown(levelEditor.Template);
        }

        public void IsShown(LevelTemplate newTemplate)
        {
            levelEditor.Template = newTemplate;

            objectSelectionBox.Items.Clear();
            int index = 0;
            foreach (var obj in gameEditor.Template.GameObjects)
            {
                objectSelectionBox.Items.Add($"Object {++index}");
            }
            if (objectSelectionBox.Items.Count > 0)
            {
                objectSelectionBox.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            levelViewerControl1.TogglePlay();
        }
    }
}
