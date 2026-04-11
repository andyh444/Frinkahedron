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
            loadButton = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            checkedListBox1 = new CheckedListBox();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // transformControl1
            // 
            transformControl1.Dock = DockStyle.Fill;
            transformControl1.Location = new Point(3, 33);
            transformControl1.Name = "transformControl1";
            transformControl1.Size = new Size(254, 108);
            transformControl1.TabIndex = 6;
            transformControl1.TransformChanged += transformControl1_TransformChanged;
            // 
            // loadButton
            // 
            loadButton.Location = new Point(3, 3);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(94, 24);
            loadButton.TabIndex = 5;
            loadButton.Text = "Load Model";
            loadButton.UseVisualStyleBackColor = true;
            loadButton.Click += loadButton_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(loadButton, 0, 0);
            tableLayoutPanel1.Controls.Add(checkedListBox1, 0, 2);
            tableLayoutPanel1.Controls.Add(transformControl1, 0, 1);
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
            checkedListBox1.Location = new Point(3, 147);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(254, 267);
            checkedListBox1.TabIndex = 7;
            checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
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
        private Button loadButton;
        private TableLayoutPanel tableLayoutPanel1;
        private CheckedListBox checkedListBox1;
    }
}
