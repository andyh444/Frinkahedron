namespace Frinkahedron.WinformsEditor.GameObjectEditor
{
    partial class GameObjectEditorControl
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
            veldridControl1 = new GameObjectViewerControl();
            renderableTemplateControl1 = new RenderableTemplateControl();
            colliderControl1 = new ColliderControl();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(veldridControl1, 1, 0);
            tableLayoutPanel1.Controls.Add(renderableTemplateControl1, 0, 0);
            tableLayoutPanel1.Controls.Add(colliderControl1, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(970, 687);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // veldridControl1
            // 
            veldridControl1.BackColor = Color.Black;
            veldridControl1.Dock = DockStyle.Fill;
            veldridControl1.Location = new Point(277, 3);
            veldridControl1.Name = "veldridControl1";
            tableLayoutPanel1.SetRowSpan(veldridControl1, 2);
            veldridControl1.Size = new Size(690, 681);
            veldridControl1.TabIndex = 7;
            // 
            // renderableTemplateControl1
            // 
            renderableTemplateControl1.Dock = DockStyle.Fill;
            renderableTemplateControl1.Location = new Point(3, 3);
            renderableTemplateControl1.Name = "renderableTemplateControl1";
            renderableTemplateControl1.Size = new Size(268, 596);
            renderableTemplateControl1.TabIndex = 5;
            // 
            // colliderControl1
            // 
            colliderControl1.Location = new Point(3, 605);
            colliderControl1.Name = "colliderControl1";
            colliderControl1.Size = new Size(268, 79);
            colliderControl1.TabIndex = 6;
            // 
            // GameObjectEditorControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "GameObjectEditorControl";
            Size = new Size(970, 687);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private RenderableTemplateControl renderableTemplateControl1;
        private ColliderControl colliderControl1;
        private GameObjectViewerControl veldridControl1;
    }
}
