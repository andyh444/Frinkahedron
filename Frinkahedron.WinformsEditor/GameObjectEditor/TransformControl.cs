using Frinkahedron.Core.Template;
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
        public event EventHandler<TransformTemplate>? TransformChanged;

        private bool freeze;

        public TransformControl()
        {
            InitializeComponent();
        }

        public void Initialise(TransformTemplate transform)
        {
            freeze = true;
            xTranslationInput.Value = (decimal)transform.Translation.X;
            yTranslationInput.Value = (decimal)transform.Translation.Y;
            zTranslationInput.Value = (decimal)transform.Translation.Z;

            xRotationInput.Value = (decimal)(transform.RotationEulerAngles.X * 180 / MathF.PI);
            yRotationInput.Value = (decimal)(transform.RotationEulerAngles.Y * 180 / MathF.PI);
            zRotationInput.Value = (decimal)(transform.RotationEulerAngles.Z * 180 / MathF.PI);

            scaleInputX.Value = (decimal)transform.Scale.X;
            scaleInputY.Value = (decimal)transform.Scale.Y;
            scaleInputZ.Value = (decimal)transform.Scale.Z;
            freeze = false;
        }

        public TransformTemplate GetTransform()
        {
            float xRot = (float)xRotationInput.Value * MathF.PI / 180;
            float yRot = (float)yRotationInput.Value * MathF.PI / 180;
            float zRot = (float)zRotationInput.Value * MathF.PI / 180;

            float xTrans = (float)xTranslationInput.Value;
            float yTrans = (float)yTranslationInput.Value;
            float zTrans = (float)zTranslationInput.Value;

            float xScale = (float)scaleInputX.Value;
            float yScale = (float)scaleInputY.Value;
            float zScale = (float)scaleInputZ.Value;

            return new TransformTemplate
            {
                Translation = new Vector3(xTrans, yTrans, zTrans),
                RotationEulerAngles = new Vector3(xRot, yRot, zRot),
                Scale = new Vector3(xScale, yScale, zScale)
            };
        }

        private void input_ValueChanged(object sender, EventArgs e)
        {
            if (freeze)
            {
                return;
            }
            TransformChanged?.Invoke(this, GetTransform());
        }
    }
}
