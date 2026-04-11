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

        public event EventHandler<IShapeTemplate>? ColliderChanged;

        public ColliderControl()
        {
            InitializeComponent();
        }

        public void Initialise(GameObjectTemplateEditor editor)
        {
            this.editor = editor;
        }

        private void dimensionInput_ValueChanged(object sender, EventArgs e)
        {
            float xDim = (float)xDimensionInput.Value;
            float yDim = (float)yDimensionInput.Value;
            float zDim = (float)zDimensionInput.Value;

            BoxTemplate box = new BoxTemplate { Dimensions = new System.Numerics.Vector3(xDim, yDim, zDim) };
            editor.Template.Collider = box;
            editor.TemplateChangedCallback();
        }
    }
}
