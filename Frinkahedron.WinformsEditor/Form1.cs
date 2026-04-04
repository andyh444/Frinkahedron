using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using Frinkahedron.Core;
using Frinkahedron.VeldridImplementation;
using System.Numerics;
using Veldrid;
using Frinkahedron.Core.Colliders;
using System.Runtime.InteropServices;

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
            }
        }

        private void transformControl1_TransformChanged(object sender, Matrix4x4 e)
        {
            veldridControl1.UpdateTransform(e);
        }

        private void colliderControl1_ColliderChanged(object sender, IShape e)
        {
            veldridControl1.UpdateCollider(e);
        }
    }
}
