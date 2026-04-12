namespace Frinkahedron.WinformsEditor.GameObjectEditor
{
    partial class RenderableTemplateControl
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
            transformControl1 = new TransformControl();
            tableLayoutPanel1 = new TableLayoutPanel();
            checkedListBox1 = new CheckedListBox();
            comboBox1 = new ComboBox();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // transformControl1
            // 
            transformControl1.Dock = DockStyle.Fill;
            transformControl1.Location = new Point(3, 32);
            transformControl1.Name = "transformControl1";
            transformControl1.Size = new Size(254, 108);
            transformControl1.TabIndex = 6;
            transformControl1.TransformChanged += transformControl1_TransformChanged;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(checkedListBox1, 0, 2);
            tableLayoutPanel1.Controls.Add(transformControl1, 0, 1);
            tableLayoutPanel1.Controls.Add(comboBox1, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(260, 417);
            tableLayoutPanel1.TabIndex = 8;
            // 
            // checkedListBox1
            // 
            checkedListBox1.Dock = DockStyle.Fill;
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(3, 146);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(254, 268);
            checkedListBox1.TabIndex = 7;
            checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
            // 
            // comboBox1
            // 
            comboBox1.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(3, 3);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(254, 23);
            comboBox1.TabIndex = 8;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // RenderableTemplateControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "RenderableTemplateControl";
            Size = new Size(260, 417);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TransformControl transformControl1;
        private TableLayoutPanel tableLayoutPanel1;
        private CheckedListBox checkedListBox1;
        private ComboBox comboBox1;
    }
}
