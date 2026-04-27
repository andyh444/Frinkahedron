using Frinkahedron.Core.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frinkahedron.WinformsEditor.GameObjectEditor
{
    public partial class RigidBodyControl : UserControl
    {
        private GameObjectTemplateEditor? editor;
        private bool freeze;

        public RigidBodyControl()
        {
            InitializeComponent();
        }

        public void Initialise(GameObjectTemplateEditor editor)
        {
            this.editor = editor;

            this.comboBox1.Items.Clear();
            this.comboBox1.Items.Add($"None");
            this.comboBox1.Items.Add($"Dynamic");
            this.comboBox1.Items.Add($"Static");

            freeze = true;

            if (editor.Template.RigidBody is DynamicBodyTemplate dbt)
            {
                comboBox1.SelectedItem = "Dynamic";
                densityInput.Enabled = true;
                densityInput.Enabled = true;
                elasticityInput.Enabled = true;
                densityInput.Value = (decimal)dbt.Density;
                elasticityInput.Value = (decimal)dbt.Elasticity;
                frictionInput.Value = (decimal)dbt.CoefficientOfFriction;
            }
            else if (editor.Template.RigidBody is StaticBodyTemplate sbt)
            {
                comboBox1.SelectedItem = "Static";
                densityInput.Enabled = false;
                frictionInput.Enabled = true;
                elasticityInput.Enabled = true;
                elasticityInput.Value = (decimal)sbt.Elasticity;
                frictionInput.Value = (decimal)sbt.CoefficientOfFriction;
            }
            else if (editor.Template.RigidBody is null)
            {
                comboBox1.SelectedItem = "None";
                densityInput.Enabled = false;
                elasticityInput.Enabled = false;
                frictionInput.Enabled = false;
            }
            else
            {
                Debugger.Break();
            }

            freeze = false;
        }


        private void input_ValueChanged(object sender, EventArgs e)
        {
            if (freeze || editor is null)
            {
                return;
            }
            editor.Template.RigidBody = GetTemplate();
            editor.TemplateChangedCallback();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (freeze || editor is null)
            {
                return;
            }
            editor.Template.RigidBody = GetTemplate();
            editor.TemplateChangedCallback();

            densityInput.Enabled = comboBox1.SelectedItem?.ToString() == "Dynamic";
            elasticityInput.Enabled = comboBox1.SelectedItem?.ToString() != "None";
            frictionInput.Enabled = comboBox1.SelectedItem?.ToString() != "None";
        }

        private IRigidBodyTemplate? GetTemplate()
        {
            if (comboBox1.SelectedItem is null)
            {
                return null;
            }
            return comboBox1.SelectedItem.ToString() switch
            {
                "None" => null,
                "Dynamic" => new DynamicBodyTemplate { Density = (float)densityInput.Value, Elasticity = (float)elasticityInput.Value, CoefficientOfFriction = (float)frictionInput.Value },
                "Static" => new StaticBodyTemplate { Elasticity = (float)elasticityInput.Value, CoefficientOfFriction = (float)frictionInput.Value },
                _ => throw new InvalidOperationException($"Unexpected rigid body type")
            };
        }
    }
}
