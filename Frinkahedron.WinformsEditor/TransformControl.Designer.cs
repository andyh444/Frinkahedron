namespace Frinkahedron.WinformsEditor
{
    partial class TransformControl
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
            scaleInput = new NumericUpDown();
            zRotationInput = new NumericUpDown();
            yRotationInput = new NumericUpDown();
            label3 = new Label();
            label1 = new Label();
            label2 = new Label();
            label4 = new Label();
            xRotationInput = new NumericUpDown();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)scaleInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)zRotationInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)yRotationInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)xRotationInput).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(scaleInput, 1, 3);
            tableLayoutPanel1.Controls.Add(zRotationInput, 1, 2);
            tableLayoutPanel1.Controls.Add(yRotationInput, 1, 1);
            tableLayoutPanel1.Controls.Add(label3, 0, 1);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 2);
            tableLayoutPanel1.Controls.Add(label4, 0, 3);
            tableLayoutPanel1.Controls.Add(xRotationInput, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Size = new Size(512, 233);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // scaleInput
            // 
            scaleInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            scaleInput.DecimalPlaces = 3;
            scaleInput.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            scaleInput.Location = new Point(259, 192);
            scaleInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            scaleInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            scaleInput.Name = "scaleInput";
            scaleInput.Size = new Size(250, 23);
            scaleInput.TabIndex = 7;
            scaleInput.Value = new decimal(new int[] { 1, 0, 0, 0 });
            scaleInput.ValueChanged += scaleInput_ValueChanged;
            // 
            // zRotationInput
            // 
            zRotationInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            zRotationInput.DecimalPlaces = 3;
            zRotationInput.Location = new Point(259, 133);
            zRotationInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            zRotationInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            zRotationInput.Name = "zRotationInput";
            zRotationInput.Size = new Size(250, 23);
            zRotationInput.TabIndex = 6;
            zRotationInput.ValueChanged += zRotationInput_ValueChanged;
            // 
            // yRotationInput
            // 
            yRotationInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            yRotationInput.DecimalPlaces = 3;
            yRotationInput.Location = new Point(259, 75);
            yRotationInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            yRotationInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            yRotationInput.Name = "yRotationInput";
            yRotationInput.Size = new Size(250, 23);
            yRotationInput.TabIndex = 5;
            yRotationInput.ValueChanged += yRotationInput_ValueChanged;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Left;
            label3.AutoSize = true;
            label3.Location = new Point(3, 79);
            label3.Name = "label3";
            label3.Size = new Size(62, 15);
            label3.TabIndex = 2;
            label3.Text = "Rotation Y";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(3, 21);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 0;
            label1.Text = "Rotation X";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new Point(3, 137);
            label2.Name = "label2";
            label2.Size = new Size(62, 15);
            label2.TabIndex = 1;
            label2.Text = "Rotation Z";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Location = new Point(3, 196);
            label4.Name = "label4";
            label4.Size = new Size(34, 15);
            label4.TabIndex = 3;
            label4.Text = "Scale";
            // 
            // xRotationInput
            // 
            xRotationInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            xRotationInput.DecimalPlaces = 3;
            xRotationInput.Location = new Point(259, 17);
            xRotationInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            xRotationInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            xRotationInput.Name = "xRotationInput";
            xRotationInput.Size = new Size(250, 23);
            xRotationInput.TabIndex = 4;
            xRotationInput.ValueChanged += xRotationInput_ValueChanged;
            // 
            // TransformControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "TransformControl";
            Size = new Size(512, 233);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)scaleInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)zRotationInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)yRotationInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)xRotationInput).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label label3;
        private Label label1;
        private Label label2;
        private Label label4;
        private NumericUpDown xRotationInput;
        private NumericUpDown scaleInput;
        private NumericUpDown zRotationInput;
        private NumericUpDown yRotationInput;
    }
}
