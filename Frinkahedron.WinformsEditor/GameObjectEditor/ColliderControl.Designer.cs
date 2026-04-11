namespace Frinkahedron.WinformsEditor
{
    partial class ColliderControl
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
            zDimensionInput = new NumericUpDown();
            yDimensionInput = new NumericUpDown();
            label3 = new Label();
            label1 = new Label();
            label2 = new Label();
            xDimensionInput = new NumericUpDown();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)zDimensionInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)yDimensionInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)xDimensionInput).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(zDimensionInput, 1, 2);
            tableLayoutPanel1.Controls.Add(yDimensionInput, 1, 1);
            tableLayoutPanel1.Controls.Add(label3, 0, 1);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 2);
            tableLayoutPanel1.Controls.Add(xDimensionInput, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Size = new Size(297, 175);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // zDimensionInput
            // 
            zDimensionInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            zDimensionInput.DecimalPlaces = 3;
            zDimensionInput.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            zDimensionInput.Location = new Point(151, 134);
            zDimensionInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            zDimensionInput.Name = "zDimensionInput";
            zDimensionInput.Size = new Size(143, 23);
            zDimensionInput.TabIndex = 5;
            zDimensionInput.Value = new decimal(new int[] { 1, 0, 0, 0 });
            zDimensionInput.ValueChanged += dimensionInput_ValueChanged;
            // 
            // yDimensionInput
            // 
            yDimensionInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            yDimensionInput.DecimalPlaces = 3;
            yDimensionInput.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            yDimensionInput.Location = new Point(151, 75);
            yDimensionInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            yDimensionInput.Name = "yDimensionInput";
            yDimensionInput.Size = new Size(143, 23);
            yDimensionInput.TabIndex = 4;
            yDimensionInput.Value = new decimal(new int[] { 1, 0, 0, 0 });
            yDimensionInput.ValueChanged += dimensionInput_ValueChanged;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Left;
            label3.AutoSize = true;
            label3.Location = new Point(3, 79);
            label3.Name = "label3";
            label3.Size = new Size(74, 15);
            label3.TabIndex = 2;
            label3.Text = "Y Dimension";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(3, 21);
            label1.Name = "label1";
            label1.Size = new Size(74, 15);
            label1.TabIndex = 0;
            label1.Text = "X Dimension";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new Point(3, 138);
            label2.Name = "label2";
            label2.Size = new Size(74, 15);
            label2.TabIndex = 1;
            label2.Text = "Z Dimension";
            // 
            // xDimensionInput
            // 
            xDimensionInput.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            xDimensionInput.DecimalPlaces = 3;
            xDimensionInput.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            xDimensionInput.Location = new Point(151, 17);
            xDimensionInput.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            xDimensionInput.Name = "xDimensionInput";
            xDimensionInput.Size = new Size(143, 23);
            xDimensionInput.TabIndex = 3;
            xDimensionInput.Value = new decimal(new int[] { 1, 0, 0, 0 });
            xDimensionInput.ValueChanged += dimensionInput_ValueChanged;
            // 
            // ColliderControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "ColliderControl";
            Size = new Size(297, 175);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)zDimensionInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)yDimensionInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)xDimensionInput).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private NumericUpDown zDimensionInput;
        private NumericUpDown yDimensionInput;
        private Label label3;
        private Label label1;
        private Label label2;
        private NumericUpDown xDimensionInput;
    }
}
