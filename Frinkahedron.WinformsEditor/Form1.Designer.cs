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
            gameObjectEditorControl1 = new Frinkahedron.WinformsEditor.GameObjectEditor.GameObjectEditorControl();
            tableLayoutPanel1 = new TableLayoutPanel();
            treeView1 = new TreeView();
            flowLayoutPanel1 = new FlowLayoutPanel();
            addObjectButton = new Button();
            loadModelButton = new Button();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            levelEditorControl1 = new Frinkahedron.WinformsEditor.LevelEditor.LevelEditorControl();
            tableLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // gameObjectEditorControl1
            // 
            gameObjectEditorControl1.Dock = DockStyle.Fill;
            gameObjectEditorControl1.Location = new Point(3, 3);
            gameObjectEditorControl1.Name = "gameObjectEditorControl1";
            gameObjectEditorControl1.Size = new Size(812, 646);
            gameObjectEditorControl1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));
            tableLayoutPanel1.Controls.Add(treeView1, 0, 0);
            tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 1);
            tableLayoutPanel1.Controls.Add(tabControl1, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tableLayoutPanel1.Size = new Size(1109, 689);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // treeView1
            // 
            treeView1.Dock = DockStyle.Fill;
            treeView1.Location = new Point(3, 3);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(271, 651);
            treeView1.TabIndex = 1;
            treeView1.AfterSelect += treeView1_AfterSelect;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(addObjectButton);
            flowLayoutPanel1.Controls.Add(loadModelButton);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(0, 657);
            flowLayoutPanel1.Margin = new Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(277, 32);
            flowLayoutPanel1.TabIndex = 2;
            // 
            // addObjectButton
            // 
            addObjectButton.Location = new Point(3, 3);
            addObjectButton.Name = "addObjectButton";
            addObjectButton.Size = new Size(75, 23);
            addObjectButton.TabIndex = 0;
            addObjectButton.Text = "Add Object";
            addObjectButton.UseVisualStyleBackColor = true;
            addObjectButton.Click += addObjectButton_Click;
            // 
            // loadModelButton
            // 
            loadModelButton.Location = new Point(84, 3);
            loadModelButton.Name = "loadModelButton";
            loadModelButton.Size = new Size(83, 23);
            loadModelButton.TabIndex = 1;
            loadModelButton.Text = "Load Model";
            loadModelButton.UseVisualStyleBackColor = true;
            loadModelButton.Click += loadModelButton_Click;
            // 
            // tabControl1
            // 
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(280, 3);
            tabControl1.Name = "tabControl1";
            tableLayoutPanel1.SetRowSpan(tabControl1, 2);
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(826, 683);
            tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(gameObjectEditorControl1);
            tabPage1.Location = new Point(4, 27);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(818, 652);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Object";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(levelEditorControl1);
            tabPage2.Location = new Point(4, 27);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(818, 652);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Level";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // levelEditorControl1
            // 
            levelEditorControl1.Dock = DockStyle.Fill;
            levelEditorControl1.Location = new Point(3, 3);
            levelEditorControl1.Name = "levelEditorControl1";
            levelEditorControl1.Size = new Size(812, 646);
            levelEditorControl1.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1109, 689);
            Controls.Add(tableLayoutPanel1);
            Name = "Form1";
            Text = "Form1";
            tableLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GameObjectEditor.GameObjectEditorControl gameObjectEditorControl1;
        private TableLayoutPanel tableLayoutPanel1;
        private TreeView treeView1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button addObjectButton;
        private Button loadModelButton;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private LevelEditor.LevelEditorControl levelEditorControl1;
    }
}
