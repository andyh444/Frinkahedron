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
            zTranslationInput = new NumericUpDown();
            yTranslationInput = new NumericUpDown();
            xTranslationInput = new NumericUpDown();
            label7 = new Label();
            label5 = new Label();
            scaleInput = new NumericUpDown();
            zRotationInput = new NumericUpDown();
            yRotationInput = new NumericUpDown();
            label3 = new Label();
            label1 = new Label();
            label2 = new Label();
            label4 = new Label();
            xRotationInput = new NumericUpDown();
            label6 = new Label();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)zTranslationInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)yTranslationInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)xTranslationInput).BeginInit();
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
            tableLayoutPanel1.Controls.Add(zTranslationInput, 1, 2);
            tableLayoutPanel1.Controls.Add(yTranslationInput, 1, 1);
            tableLayoutPanel1.Controls.Add(xTranslationInput, 1, 0);
            tableLayoutPanel1.Controls.Add(label7, 0, 1);
            tableLayoutPanel1.Controls.Add(label5, 0, 0);
            tableLayoutPanel1.Controls.Add(scaleInput, 1, 6);
            tableLayoutPanel1.Controls.Add(zRotationInput, 1, 5);
            tableLayoutPanel1.Controls.Add(yRotationInput, 1, 4);
            tableLayoutPanel1.Controls.Add(label3, 0, 4);
            tableLayoutPanel1.Controls.Add(label1, 0, 3);
            tableLayoutPanel1.Controls.Add(label2, 0, 5);
            tableLayoutPanel1.Controls.Add(label4, 0, 6);
            tableLayoutPanel1.Controls.Add(xRotationInput, 1, 3);
            tableLayoutPanel1.Controls.Add(label6, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.Size = new Size(512, 233);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // zTranslationInput
            // 
            zTranslationInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            zTranslationInput.DecimalPlaces = 3;
            zTranslationInput.Location = new Point(259, 71);
            zTranslationInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            zTranslationInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            zTranslationInput.Name = "zTranslationInput";
            zTranslationInput.Size = new Size(250, 23);
            zTranslationInput.TabIndex = 13;
            zTranslationInput.ValueChanged += input_ValueChanged;
            // 
            // yTranslationInput
            // 
            yTranslationInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            yTranslationInput.DecimalPlaces = 3;
            yTranslationInput.Location = new Point(259, 38);
            yTranslationInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            yTranslationInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            yTranslationInput.Name = "yTranslationInput";
            yTranslationInput.Size = new Size(250, 23);
            yTranslationInput.TabIndex = 12;
            yTranslationInput.ValueChanged += input_ValueChanged;
            // 
            // xTranslationInput
            // 
            xTranslationInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            xTranslationInput.DecimalPlaces = 3;
            xTranslationInput.Location = new Point(259, 5);
            xTranslationInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            xTranslationInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            xTranslationInput.Name = "xTranslationInput";
            xTranslationInput.Size = new Size(250, 23);
            xTranslationInput.TabIndex = 11;
            xTranslationInput.ValueChanged += input_ValueChanged;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Left;
            label7.AutoSize = true;
            label7.Location = new Point(3, 42);
            label7.Name = "label7";
            label7.Size = new Size(74, 15);
            label7.TabIndex = 10;
            label7.Text = "Translation Y";
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Left;
            label5.AutoSize = true;
            label5.Location = new Point(3, 9);
            label5.Name = "label5";
            label5.Size = new Size(74, 15);
            label5.TabIndex = 8;
            label5.Text = "Translation X";
            // 
            // scaleInput
            // 
            scaleInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            scaleInput.DecimalPlaces = 3;
            scaleInput.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            scaleInput.Location = new Point(259, 204);
            scaleInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            scaleInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            scaleInput.Name = "scaleInput";
            scaleInput.Size = new Size(250, 23);
            scaleInput.TabIndex = 7;
            scaleInput.Value = new decimal(new int[] { 1, 0, 0, 0 });
            scaleInput.ValueChanged += input_ValueChanged;
            // 
            // zRotationInput
            // 
            zRotationInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            zRotationInput.DecimalPlaces = 3;
            zRotationInput.Location = new Point(259, 170);
            zRotationInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            zRotationInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            zRotationInput.Name = "zRotationInput";
            zRotationInput.Size = new Size(250, 23);
            zRotationInput.TabIndex = 6;
            zRotationInput.ValueChanged += input_ValueChanged;
            // 
            // yRotationInput
            // 
            yRotationInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            yRotationInput.DecimalPlaces = 3;
            yRotationInput.Location = new Point(259, 137);
            yRotationInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            yRotationInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            yRotationInput.Name = "yRotationInput";
            yRotationInput.Size = new Size(250, 23);
            yRotationInput.TabIndex = 5;
            yRotationInput.ValueChanged += input_ValueChanged;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Left;
            label3.AutoSize = true;
            label3.Location = new Point(3, 141);
            label3.Name = "label3";
            label3.Size = new Size(62, 15);
            label3.TabIndex = 2;
            label3.Text = "Rotation Y";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(3, 108);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 0;
            label1.Text = "Rotation X";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new Point(3, 174);
            label2.Name = "label2";
            label2.Size = new Size(62, 15);
            label2.TabIndex = 1;
            label2.Text = "Rotation Z";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Location = new Point(3, 208);
            label4.Name = "label4";
            label4.Size = new Size(34, 15);
            label4.TabIndex = 3;
            label4.Text = "Scale";
            // 
            // xRotationInput
            // 
            xRotationInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            xRotationInput.DecimalPlaces = 3;
            xRotationInput.Location = new Point(259, 104);
            xRotationInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            xRotationInput.Minimum = new decimal(new int[] { 1000, 0, 0, int.MinValue });
            xRotationInput.Name = "xRotationInput";
            xRotationInput.Size = new Size(250, 23);
            xRotationInput.TabIndex = 4;
            xRotationInput.ValueChanged += input_ValueChanged;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Left;
            label6.AutoSize = true;
            label6.Location = new Point(3, 75);
            label6.Name = "label6";
            label6.Size = new Size(74, 15);
            label6.TabIndex = 9;
            label6.Text = "Translation Z";
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
            ((System.ComponentModel.ISupportInitialize)zTranslationInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)yTranslationInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)xTranslationInput).EndInit();
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
        private NumericUpDown zTranslationInput;
        private NumericUpDown yTranslationInput;
        private NumericUpDown xTranslationInput;
        private Label label7;
        private Label label5;
        private Label label6;
    }
}
