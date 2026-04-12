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
        private GameObjectTemplateEditor? editor;
        private ModelInfo? modelInfo;
        private bool[] activeIndices;
        private TransformTemplate currentTransform;
        private bool freeze;

        public RenderableTemplateControl()
        {
            InitializeComponent();
            activeIndices = [];
            currentTransform = new TransformTemplate();
        }

        public void Initialise(GameObjectTemplateEditor editor)
        {
            this.editor = editor;
            transformControl1.Initialise(editor.Template.Renderable?.Transform ?? new TransformTemplate());
            InitialiseCheckedListBox(editor.Template.Renderable);
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

        private void loadButton_Click(object sender, EventArgs e)
        {
            if (editor?.LoadModelFunc is null)
            {
                return;
            }

            using OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = $"gltf files (*.gltf)|*.gltf",
            };

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                modelInfo = editor.LoadModelFunc(ofd.FileName);
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
            if (modelInfo is null || editor is null)
            {
                return;
            }

            editor.Template.Renderable = new ModelEntitiesRenderableTemplate
            {
                ModelID = modelInfo.Value.ModelID,
                EnabledIndices = activeIndices,
                Transform = currentTransform,
            };

            editor.TemplateChangedCallback?.Invoke();
        }
    }
}
