namespace Frinkahedron.WinformsEditor
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            veldridControl1 = new Frinkahedron.WinformsEditor.GameObjectEditor.VeldridControl();
            colliderControl1 = new ColliderControl();
            renderableTemplateControl1 = new Frinkahedron.WinformsEditor.GameObjectEditor.RenderableTemplateControl();
            SuspendLayout();
            // 
            // veldridControl1
            // 
            veldridControl1.BackColor = Color.Black;
            veldridControl1.Location = new Point(172, 12);
            veldridControl1.Name = "veldridControl1";
            veldridControl1.Size = new Size(925, 665);
            veldridControl1.TabIndex = 0;
            // 
            // colliderControl1
            // 
            colliderControl1.Location = new Point(5, 586);
            colliderControl1.Name = "colliderControl1";
            colliderControl1.Size = new Size(161, 79);
            colliderControl1.TabIndex = 3;
            colliderControl1.ColliderChanged += colliderControl1_ColliderChanged;
            // 
            // renderableTemplateControl1
            // 
            renderableTemplateControl1.Location = new Point(5, 12);
            renderableTemplateControl1.Name = "renderableTemplateControl1";
            renderableTemplateControl1.Size = new Size(161, 414);
            renderableTemplateControl1.TabIndex = 4;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1109, 689);
            Controls.Add(renderableTemplateControl1);
            Controls.Add(colliderControl1);
            Controls.Add(veldridControl1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private GameObjectEditor.VeldridControl veldridControl1;
        private ColliderControl colliderControl1;
        private GameObjectEditor.RenderableTemplateControl renderableTemplateControl1;
    }
}
