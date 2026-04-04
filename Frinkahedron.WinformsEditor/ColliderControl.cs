using Frinkahedron.Core.Colliders;
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
        public event EventHandler<IShape>? ColliderChanged;

        public ColliderControl()
        {
            InitializeComponent();
        }

        private void dimensionInput_ValueChanged(object sender, EventArgs e)
        {
            float xDim = (float)xDimensionInput.Value;
            float yDim = (float)yDimensionInput.Value;
            float zDim = (float)zDimensionInput.Value;

            Box box = new Box(new System.Numerics.Vector3(xDim, yDim, zDim));
            ColliderChanged?.Invoke(this, box);
        }
    }
}
