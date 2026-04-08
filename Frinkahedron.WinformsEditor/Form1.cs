using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using Frinkahedron.Core;
using Frinkahedron.VeldridImplementation;
using System.Numerics;
using Veldrid;
using Frinkahedron.Core.Colliders;
using System.Runtime.InteropServices;
using Frinkahedron.Core.Template;

namespace Frinkahedron.WinformsEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = $"gltf files (*.gltf)|*.gltf",
            };

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                veldridControl1.LoadModel(ofd.FileName);
                var template = veldridControl1.GetGameObjectTemplate();
                if (template.Renderable is ModelRenderableTemplate mrt)
                {
                    mrt.ModelID = Path.GetFileNameWithoutExtension(ofd.FileName);
                }
                else
                {
                    template.Renderable = new ModelRenderableTemplate { ModelID = Path.GetFileNameWithoutExtension(ofd.FileName) };
                }
                veldridControl1.GameObjectTemplateUpdated();
            }
        }

        private void transformControl1_TransformChanged(object sender, Matrix4x4 e)
        {
            var template = veldridControl1.GetGameObjectTemplate();
            template.Renderable?.TrySetTransform(e);
            veldridControl1.GameObjectTemplateUpdated();
        }

        private void colliderControl1_ColliderChanged(object sender, IShapeTemplate e)
        {
            var template = veldridControl1.GetGameObjectTemplate();
            template.Collider = e;
            veldridControl1.GameObjectTemplateUpdated();
        }
    }
}
