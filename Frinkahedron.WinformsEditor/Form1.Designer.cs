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
            veldridControl1 = new VeldridControl();
            loadButton = new Button();
            transformControl1 = new TransformControl();
            colliderControl1 = new ColliderControl();
            checkedListBox1 = new CheckedListBox();
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
            // loadButton
            // 
            loadButton.Location = new Point(22, 18);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(94, 23);
            loadButton.TabIndex = 1;
            loadButton.Text = "Load Model";
            loadButton.UseVisualStyleBackColor = true;
            loadButton.Click += loadButton_Click;
            // 
            // transformControl1
            // 
            transformControl1.Location = new Point(12, 47);
            transformControl1.Name = "transformControl1";
            transformControl1.Size = new Size(145, 188);
            transformControl1.TabIndex = 2;
            transformControl1.TransformChanged += transformControl1_TransformChanged;
            // 
            // colliderControl1
            // 
            colliderControl1.Location = new Point(5, 286);
            colliderControl1.Name = "colliderControl1";
            colliderControl1.Size = new Size(161, 79);
            colliderControl1.TabIndex = 3;
            colliderControl1.ColliderChanged += colliderControl1_ColliderChanged;
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(5, 380);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(161, 292);
            checkedListBox1.TabIndex = 4;
            checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1109, 689);
            Controls.Add(checkedListBox1);
            Controls.Add(colliderControl1);
            Controls.Add(transformControl1);
            Controls.Add(loadButton);
            Controls.Add(veldridControl1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private VeldridControl veldridControl1;
        private Button loadButton;
        private TransformControl transformControl1;
        private ColliderControl colliderControl1;
        private CheckedListBox checkedListBox1;
    }
}
