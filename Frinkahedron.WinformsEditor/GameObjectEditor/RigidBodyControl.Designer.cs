namespace Frinkahedron.WinformsEditor.GameObjectEditor
{
    partial class RigidBodyControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            label4 = new Label();
            frictionInput = new NumericUpDown();
            elasticityInput = new NumericUpDown();
            label1 = new Label();
            label3 = new Label();
            label2 = new Label();
            densityInput = new NumericUpDown();
            comboBox1 = new ComboBox();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)frictionInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)elasticityInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)densityInput).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(label4, 0, 0);
            tableLayoutPanel1.Controls.Add(frictionInput, 1, 3);
            tableLayoutPanel1.Controls.Add(elasticityInput, 1, 2);
            tableLayoutPanel1.Controls.Add(label1, 0, 1);
            tableLayoutPanel1.Controls.Add(label3, 0, 3);
            tableLayoutPanel1.Controls.Add(label2, 0, 2);
            tableLayoutPanel1.Controls.Add(densityInput, 1, 1);
            tableLayoutPanel1.Controls.Add(comboBox1, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 24.998127F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25.00062F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25.0006256F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25.0006256F));
            tableLayoutPanel1.Size = new Size(174, 120);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Location = new Point(3, 7);
            label4.Name = "label4";
            label4.Size = new Size(31, 15);
            label4.TabIndex = 6;
            label4.Text = "Type";
            // 
            // frictionInput
            // 
            frictionInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            frictionInput.DecimalPlaces = 3;
            frictionInput.Location = new Point(62, 93);
            frictionInput.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            frictionInput.Name = "frictionInput";
            frictionInput.Size = new Size(109, 23);
            frictionInput.TabIndex = 5;
            frictionInput.ValueChanged += input_ValueChanged;
            // 
            // elasticityInput
            // 
            elasticityInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            elasticityInput.DecimalPlaces = 3;
            elasticityInput.Location = new Point(62, 62);
            elasticityInput.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            elasticityInput.Name = "elasticityInput";
            elasticityInput.Size = new Size(109, 23);
            elasticityInput.TabIndex = 4;
            elasticityInput.ValueChanged += input_ValueChanged;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(3, 36);
            label1.Name = "label1";
            label1.Size = new Size(46, 15);
            label1.TabIndex = 0;
            label1.Text = "Density";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Left;
            label3.AutoSize = true;
            label3.Location = new Point(3, 97);
            label3.Name = "label3";
            label3.Size = new Size(47, 15);
            label3.TabIndex = 2;
            label3.Text = "Friction";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new Point(3, 66);
            label2.Name = "label2";
            label2.Size = new Size(53, 15);
            label2.TabIndex = 1;
            label2.Text = "Elasticity";
            // 
            // densityInput
            // 
            densityInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            densityInput.DecimalPlaces = 3;
            densityInput.Location = new Point(62, 32);
            densityInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            densityInput.Name = "densityInput";
            densityInput.Size = new Size(109, 23);
            densityInput.TabIndex = 3;
            densityInput.ValueChanged += input_ValueChanged;
            // 
            // comboBox1
            // 
            comboBox1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(62, 3);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(109, 23);
            comboBox1.TabIndex = 7;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // RigidBodyControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "RigidBodyControl";
            Size = new Size(174, 120);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)frictionInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)elasticityInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)densityInput).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private NumericUpDown frictionInput;
        private NumericUpDown elasticityInput;
        private Label label1;
        private Label label3;
        private Label label2;
        private NumericUpDown densityInput;
        private Label label4;
        private ComboBox comboBox1;
    }
}
