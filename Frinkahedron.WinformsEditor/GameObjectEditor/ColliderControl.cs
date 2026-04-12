using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Template;
using Frinkahedron.WinformsEditor.GameObjectEditor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frinkahedron.WinformsEditor
{
    public partial class ColliderControl : UserControl
    {
        private GameObjectTemplateEditor? editor;
        private bool freeze;

        public event EventHandler<IShapeTemplate>? ColliderChanged;

        public ColliderControl()
        {
            InitializeComponent();
        }

        public void Initialise(GameObjectTemplateEditor editor)
        {
            this.editor = editor;

            freeze = true;
            if (editor.Template.Collider is BoxTemplate bt)
            {
                xDimensionInput.Value = (decimal)bt.Dimensions.X;
                yDimensionInput.Value = (decimal)bt.Dimensions.Y;
                zDimensionInput.Value = (decimal)bt.Dimensions.Z;
            }
            freeze = false;
        }

        private void dimensionInput_ValueChanged(object sender, EventArgs e)
        {
            if (freeze)
            {
                return;
            }
            float xDim = (float)xDimensionInput.Value;
            float yDim = (float)yDimensionInput.Value;
            float zDim = (float)zDimensionInput.Value;

            BoxTemplate box = new BoxTemplate { Dimensions = new System.Numerics.Vector3(xDim, yDim, zDim) };
            editor.Template.Collider = box;
            editor.TemplateChangedCallback();
        }
    }
}
