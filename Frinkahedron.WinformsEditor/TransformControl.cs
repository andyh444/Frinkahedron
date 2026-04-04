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

namespace Frinkahedron.WinformsEditor
{
    public partial class TransformControl : UserControl
    {
        public event EventHandler<Matrix4x4>? TransformChanged;

        public TransformControl()
        {
            InitializeComponent();
        }

        private Matrix4x4 CalculateTransform()
        {
            float xRot = (float)xRotationInput.Value * MathF.PI / 180;
            float yRot = (float)yRotationInput.Value * MathF.PI / 180;
            float zRot = (float)zRotationInput.Value * MathF.PI / 180;

            Quaternion rotation = Quaternion.CreateFromYawPitchRoll(yRot, xRot, zRot);
            return Matrix4x4.CreateScale((float)scaleInput.Value) * Matrix4x4.CreateFromQuaternion(rotation);
        }

        private void xRotationInput_ValueChanged(object sender, EventArgs e)
        {
            TransformChanged?.Invoke(this, CalculateTransform());
        }

        private void yRotationInput_ValueChanged(object sender, EventArgs e)
        {
            TransformChanged?.Invoke(this, CalculateTransform());
        }

        private void zRotationInput_ValueChanged(object sender, EventArgs e)
        {
            TransformChanged?.Invoke(this, CalculateTransform());
        }

        private void scaleInput_ValueChanged(object sender, EventArgs e)
        {
            TransformChanged?.Invoke(this, CalculateTransform());
        }
    }
}
