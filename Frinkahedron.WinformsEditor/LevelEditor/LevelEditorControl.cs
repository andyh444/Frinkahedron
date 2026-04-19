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
using Vulkan;

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
            levelEditor.RegisterTemplateChangedCallback(PopulateLevelObjectsListBox);
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
            PopulateLevelObjectsListBox();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            levelViewerControl1.TogglePlay();
        }

        private void PopulateLevelObjectsListBox()
        {
            if (levelEditor.Template.LevelObjects.Count != levelObjectsBox.Items.Count)
            {
                levelObjectsBox.Items.Clear();
                foreach (var levelObj in levelEditor.Template.LevelObjects)
                {
                    levelObjectsBox.Items.Add($"Object {levelObjectsBox.Items.Count + 1}");
                }
            }
        }

        private void levelObjectsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (levelObjectsBox.SelectedIndex < 0)
            {
                return;
            }
            var levelObj = levelEditor.Template.LevelObjects[levelObjectsBox.SelectedIndex];
            levelObjTransformControl.Initialise(levelObj.WorldTransform, false);
        }

        private void levelObjTransformControl_TransformChanged(object sender, TransformTemplate e)
        {
            if (levelObjectsBox.SelectedIndex < 0)
            {
                return;
            }
            var levelObj = levelEditor.Template.LevelObjects[levelObjectsBox.SelectedIndex];
            levelObj.WorldTransform = e;

            levelEditor.TemplateChanged();
        }
    }
}
