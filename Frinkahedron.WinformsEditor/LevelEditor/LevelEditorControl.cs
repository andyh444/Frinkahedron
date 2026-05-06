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
        private bool freezeSelectionChanged;

        public LevelEditorControl()
        {
            InitializeComponent();
            levelEditor = new LevelTemplateEditor();
            _ = levelEditor.RegisterTemplateChangedCallback(PopulateLevelObjectsListBox);
            _ = levelEditor.RegisterSelectedIndexChangedCallback(SetSelectedIndex);
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
            SetSelectedIndex();
        }

        private void SetSelectedIndex()
        {
            if (levelObjectsBox.Items.Count == 0)
            {
                return;
            }
            freezeSelectionChanged = true;
            levelObjectsBox.SelectedIndex = levelEditor.LevelObjectSelectedIndex;
            freezeSelectionChanged = false; 
        }

        private void levelObjectsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (levelObjectsBox.SelectedIndex < 0 || freezeSelectionChanged)
            {
                return;
            }
            var levelObj = levelEditor.Template.LevelObjects[levelObjectsBox.SelectedIndex];
            levelObjTransformControl.Initialise(levelObj.WorldTransform, false);

            levelEditor.LevelObjectSelectedIndex = levelObjectsBox.SelectedIndex;
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

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (levelObjectsBox.SelectedIndex < 0)
            {
                return;
            }
            int selectedIndex = levelObjectsBox.SelectedIndex;
            levelEditor.Template.LevelObjects.RemoveAt(selectedIndex);
            levelObjectsBox.Items.RemoveAt(selectedIndex);
            if (levelObjectsBox.Items.Count > 0)
            {
                levelObjectsBox.SelectedIndex = Math.Clamp(selectedIndex, 0, levelObjectsBox.Items.Count - 1);
            }

            levelEditor.TemplateChanged();
        }

        private void levelObjectsBox_DoubleClick(object sender, EventArgs e)
        {
            if (levelObjectsBox.SelectedIndex < 0)
            {
                return;
            }
            levelViewerControl1.CentreCameraOnObject(levelObjectsBox.SelectedIndex);
        }

        private void placeObjectButton_CheckedChanged(object sender, EventArgs e)
        {
            levelViewerControl1.SetClickMode(LevelViewerClickMode.Place);
        }

        private void selectObjectButton_CheckedChanged(object sender, EventArgs e)
        {
            levelViewerControl1.SetClickMode(LevelViewerClickMode.Select);
        }
    }
}
