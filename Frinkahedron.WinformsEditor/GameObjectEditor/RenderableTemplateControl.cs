using Frinkahedron.Core.Template;
using Frinkahedron.VeldridImplementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frinkahedron.WinformsEditor.GameObjectEditor
{
    public partial class RenderableTemplateControl : UserControl
    {
        private GameTemplateEditor? gameEditor;
        private GameObjectTemplateEditor? objectEditor;
        private bool[] activeIndices;
        private TransformTemplate currentTransform;
        private bool freeze;

        public RenderableTemplateControl()
        {
            InitializeComponent();
            activeIndices = [];
            currentTransform = new TransformTemplate();
        }

        public void Initialise(GameTemplateEditor gameEditor, GameObjectTemplateEditor objectEditor)
        {
            this.gameEditor = gameEditor;
            this.objectEditor = objectEditor;

            transformControl1.Initialise(objectEditor.Template.Renderable?.Transform ?? new TransformTemplate());
            InitialiseCheckedListBox(objectEditor.Template.Renderable);
            InitialiseModelComboBox(gameEditor, objectEditor);

            currentTransform = transformControl1.GetTransform();
        }

        private void InitialiseModelComboBox(GameTemplateEditor gameEditor, GameObjectTemplateEditor objectEditor)
        {
            freeze = true;
            comboBox1.Items.Clear();
            comboBox1.Items.Add(string.Empty);
            foreach (var model in gameEditor.Template.Models)
            {
                comboBox1.Items.Add(model.ModelID);
            }
            if (objectEditor.Template.Renderable is ModelEntitiesRenderableTemplate mert)
            {
                comboBox1.SelectedItem = mert.ModelID;
            }
            else
            {
                comboBox1.SelectedItem = string.Empty;
            }
            freeze = false;
        }

        private void InitialiseCheckedListBox(IRenderableTemplate? template)
        {
            freeze = true;
            checkedListBox1.Items.Clear();
            if (template is ModelEntitiesRenderableTemplate mert)
            {
                for (int i = 0; i < mert.EnabledIndices.Length; i++)
                {
                    var index = checkedListBox1.Items.Add($"Entity {i + 1}");
                    checkedListBox1.SetItemChecked(index, mert.EnabledIndices[i]);
                }
                activeIndices = mert.EnabledIndices;
            }
            freeze = false;
        }

        private void transformControl1_TransformChanged(object sender, TransformTemplate e)
        {
            currentTransform = e;
            UpdateRenderableTemplate();
        }

        private void checkedListBox1_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            if (freeze)
            {
                return;
            }
            activeIndices[e.Index] = e.NewValue == CheckState.Checked;
            UpdateRenderableTemplate();
        }

        private void UpdateRenderableTemplate()
        {
            if (objectEditor is null)
            {
                return;
            }

            if (string.IsNullOrEmpty(comboBox1.SelectedItem.ToString()))
            {
                objectEditor.Template.Renderable = null;
            }
            else
            {
                objectEditor.Template.Renderable = new ModelEntitiesRenderableTemplate
                {
                    ModelID = comboBox1.SelectedItem.ToString(),
                    EnabledIndices = activeIndices,
                    Transform = currentTransform,
                };
            }
            objectEditor.TemplateChangedCallback?.Invoke();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (freeze)
            {
                return;
            }
            if (string.IsNullOrEmpty(comboBox1.SelectedItem?.ToString()))
            {
                InitialiseCheckedListBox(null);
                UpdateRenderableTemplate();
                return;
            }
            var modelInfo = objectEditor.LoadModelFunc(comboBox1.SelectedItem.ToString());
            if (modelInfo is null)
            {
                return;
            }
            ModelEntitiesRenderableTemplate template = new ModelEntitiesRenderableTemplate
            {
                ModelID = modelInfo.Value.ModelID,
                EnabledIndices = modelInfo.Value.Model.Entities.Select(x => true).ToArray(),
                Transform = currentTransform
            };
            InitialiseCheckedListBox(template);
            UpdateRenderableTemplate();
        }
    }
}
