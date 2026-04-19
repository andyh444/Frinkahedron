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
            xTranslationInput = new NumericUpDown();
            label5 = new Label();
            yTranslationInput = new NumericUpDown();
            zTranslationInput = new NumericUpDown();
            label1 = new Label();
            xRotationInput = new NumericUpDown();
            yRotationInput = new NumericUpDown();
            zRotationInput = new NumericUpDown();
            label4 = new Label();
            scaleInputX = new NumericUpDown();
            scaleInputY = new NumericUpDown();
            scaleInputZ = new NumericUpDown();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)xTranslationInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)yTranslationInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)zTranslationInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)xRotationInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)yRotationInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)zRotationInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)scaleInputX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)scaleInputY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)scaleInputZ).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Controls.Add(scaleInputZ, 3, 2);
            tableLayoutPanel1.Controls.Add(scaleInputY, 2, 2);
            tableLayoutPanel1.Controls.Add(xTranslationInput, 1, 0);
            tableLayoutPanel1.Controls.Add(label5, 0, 0);
            tableLayoutPanel1.Controls.Add(yTranslationInput, 2, 0);
            tableLayoutPanel1.Controls.Add(zTranslationInput, 3, 0);
            tableLayoutPanel1.Controls.Add(label1, 0, 1);
            tableLayoutPanel1.Controls.Add(xRotationInput, 1, 1);
            tableLayoutPanel1.Controls.Add(yRotationInput, 2, 1);
            tableLayoutPanel1.Controls.Add(zRotationInput, 3, 1);
            tableLayoutPanel1.Controls.Add(label4, 0, 2);
            tableLayoutPanel1.Controls.Add(scaleInputX, 1, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33334F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
            tableLayoutPanel1.Size = new Size(246, 105);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // xTranslationInput
            // 
            xTranslationInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            xTranslationInput.DecimalPlaces = 3;
            xTranslationInput.Location = new Point(73, 5);
            xTranslationInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            xTranslationInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            xTranslationInput.Name = "xTranslationInput";
            xTranslationInput.Size = new Size(52, 23);
            xTranslationInput.TabIndex = 11;
            xTranslationInput.ValueChanged += input_ValueChanged;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Left;
            label5.AutoSize = true;
            label5.Location = new Point(3, 9);
            label5.Name = "label5";
            label5.Size = new Size(64, 15);
            label5.TabIndex = 8;
            label5.Text = "Translation";
            // 
            // yTranslationInput
            // 
            yTranslationInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            yTranslationInput.DecimalPlaces = 3;
            yTranslationInput.Location = new Point(131, 5);
            yTranslationInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            yTranslationInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            yTranslationInput.Name = "yTranslationInput";
            yTranslationInput.Size = new Size(52, 23);
            yTranslationInput.TabIndex = 12;
            yTranslationInput.ValueChanged += input_ValueChanged;
            // 
            // zTranslationInput
            // 
            zTranslationInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            zTranslationInput.DecimalPlaces = 3;
            zTranslationInput.Location = new Point(189, 5);
            zTranslationInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            zTranslationInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            zTranslationInput.Name = "zTranslationInput";
            zTranslationInput.Size = new Size(54, 23);
            zTranslationInput.TabIndex = 13;
            zTranslationInput.ValueChanged += input_ValueChanged;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(3, 44);
            label1.Name = "label1";
            label1.Size = new Size(52, 15);
            label1.TabIndex = 0;
            label1.Text = "Rotation";
            // 
            // xRotationInput
            // 
            xRotationInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            xRotationInput.DecimalPlaces = 3;
            xRotationInput.Location = new Point(73, 40);
            xRotationInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            xRotationInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            xRotationInput.Name = "xRotationInput";
            xRotationInput.Size = new Size(52, 23);
            xRotationInput.TabIndex = 4;
            xRotationInput.ValueChanged += input_ValueChanged;
            // 
            // yRotationInput
            // 
            yRotationInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            yRotationInput.DecimalPlaces = 3;
            yRotationInput.Location = new Point(131, 40);
            yRotationInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            yRotationInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            yRotationInput.Name = "yRotationInput";
            yRotationInput.Size = new Size(52, 23);
            yRotationInput.TabIndex = 5;
            yRotationInput.ValueChanged += input_ValueChanged;
            // 
            // zRotationInput
            // 
            zRotationInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            zRotationInput.DecimalPlaces = 3;
            zRotationInput.Location = new Point(189, 40);
            zRotationInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            zRotationInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            zRotationInput.Name = "zRotationInput";
            zRotationInput.Size = new Size(54, 23);
            zRotationInput.TabIndex = 6;
            zRotationInput.ValueChanged += input_ValueChanged;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Location = new Point(3, 79);
            label4.Name = "label4";
            label4.Size = new Size(34, 15);
            label4.TabIndex = 3;
            label4.Text = "Scale";
            // 
            // scaleInputX
            // 
            scaleInputX.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            scaleInputX.DecimalPlaces = 3;
            scaleInputX.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            scaleInputX.Location = new Point(73, 75);
            scaleInputX.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            scaleInputX.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            scaleInputX.Name = "scaleInputX";
            scaleInputX.Size = new Size(52, 23);
            scaleInputX.TabIndex = 7;
            scaleInputX.Value = new decimal(new int[] { 1, 0, 0, 0 });
            scaleInputX.ValueChanged += input_ValueChanged;
            // 
            // scaleInputY
            // 
            scaleInputY.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            scaleInputY.DecimalPlaces = 3;
            scaleInputY.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            scaleInputY.Location = new Point(131, 75);
            scaleInputY.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            scaleInputY.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            scaleInputY.Name = "scaleInputY";
            scaleInputY.Size = new Size(52, 23);
            scaleInputY.TabIndex = 14;
            scaleInputY.Value = new decimal(new int[] { 1, 0, 0, 0 });
            scaleInputY.ValueChanged += input_ValueChanged;
            // 
            // scaleInputZ
            // 
            scaleInputZ.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            scaleInputZ.DecimalPlaces = 3;
            scaleInputZ.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            scaleInputZ.Location = new Point(189, 75);
            scaleInputZ.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            scaleInputZ.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            scaleInputZ.Name = "scaleInputZ";
            scaleInputZ.Size = new Size(54, 23);
            scaleInputZ.TabIndex = 15;
            scaleInputZ.Value = new decimal(new int[] { 1, 0, 0, 0 });
            scaleInputZ.ValueChanged += input_ValueChanged;
            // 
            // TransformControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "TransformControl";
            Size = new Size(246, 105);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)xTranslationInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)yTranslationInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)zTranslationInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)xRotationInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)yRotationInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)zRotationInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)scaleInputX).EndInit();
            ((System.ComponentModel.ISupportInitialize)scaleInputY).EndInit();
            ((System.ComponentModel.ISupportInitialize)scaleInputZ).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label label1;
        private Label label4;
        private NumericUpDown xRotationInput;
        private NumericUpDown scaleInputX;
        private NumericUpDown zRotationInput;
        private NumericUpDown yRotationInput;
        private NumericUpDown zTranslationInput;
        private NumericUpDown yTranslationInput;
        private NumericUpDown xTranslationInput;
        private Label label5;
        private NumericUpDown scaleInputZ;
        private NumericUpDown scaleInputY;
    }
}
