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
        private HashSet<int> activeIndices;
        private Matrix4x4 currentTransform;

        public RenderableTemplateControl()
        {
            InitializeComponent();
            activeIndices = new HashSet<int>();
            currentTransform = Matrix4x4.Identity;
        }

        public void Initialise(GameObjectTemplateEditor editor)
        {
            this.editor = editor;
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

                checkedListBox1.ItemCheck -= checkedListBox1_ItemCheck;
                checkedListBox1.Items.Clear();
                for (int i = 0; i < modelInfo.Value.Model.Entities.Count; i++)
                {
                    var index = checkedListBox1.Items.Add($"Entity {i + 1}");
                    checkedListBox1.SetItemChecked(index, true);
                    activeIndices.Add(index);
                }
                checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
                UpdateRenderableTemplate();
            }
        }

        private void transformControl1_TransformChanged(object sender, System.Numerics.Matrix4x4 e)
        {
            currentTransform = e;
            UpdateRenderableTemplate();
        }

        private void checkedListBox1_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                activeIndices.Add(e.Index);
            }
            else
            {
                activeIndices.Remove(e.Index);
            }
            UpdateRenderableTemplate();
        }

        private void UpdateRenderableTemplate()
        {
            if (modelInfo is null || editor is null)
            {
                return;
            }

            editor.Template.Renderable = new CompositeRenderableTemplate
            {
                Children = activeIndices
                .Select<int, IRenderableTemplate>(x => new ModelEntityRenderableTemplate
                {
                    ModelID = modelInfo.Value.ModelID,
                    Index = x,
                    Transform = currentTransform
                })
                .ToList()
            };

            editor.TemplateChangedCallback?.Invoke();
        }
    }
}
