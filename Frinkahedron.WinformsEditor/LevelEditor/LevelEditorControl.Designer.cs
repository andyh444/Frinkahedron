namespace Frinkahedron.WinformsEditor.LevelEditor
{
    partial class LevelEditorControl
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
            levelViewerControl1 = new LevelViewerControl();
            tableLayoutPanel2 = new TableLayoutPanel();
            objectSelectionBox = new ComboBox();
            button1 = new Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));
            tableLayoutPanel1.Controls.Add(levelViewerControl1, 1, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(952, 612);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // levelViewerControl1
            // 
            levelViewerControl1.BackColor = Color.Black;
            levelViewerControl1.Dock = DockStyle.Fill;
            levelViewerControl1.Location = new Point(241, 3);
            levelViewerControl1.Name = "levelViewerControl1";
            levelViewerControl1.Size = new Size(708, 606);
            levelViewerControl1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(objectSelectionBox, 0, 0);
            tableLayoutPanel2.Controls.Add(button1, 0, 1);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(232, 606);
            tableLayoutPanel2.TabIndex = 1;
            // 
            // objectSelectionBox
            // 
            objectSelectionBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            objectSelectionBox.FormattingEnabled = true;
            objectSelectionBox.Location = new Point(3, 3);
            objectSelectionBox.Name = "objectSelectionBox";
            objectSelectionBox.Size = new Size(226, 23);
            objectSelectionBox.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(3, 32);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 1;
            button1.Text = "Play/Stop";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // LevelEditorControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "LevelEditorControl";
            Size = new Size(952, 612);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private LevelViewerControl levelViewerControl1;
        private TableLayoutPanel tableLayoutPanel2;
        private ComboBox objectSelectionBox;
        private Button button1;
    }
}
