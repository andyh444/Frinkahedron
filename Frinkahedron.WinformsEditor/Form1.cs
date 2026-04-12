using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using Frinkahedron.Core;
using Frinkahedron.VeldridImplementation;
using System.Numerics;
using Veldrid;
using Frinkahedron.Core.Colliders;
using System.Runtime.InteropServices;
using Frinkahedron.Core.Template;
using Frinkahedron.WinformsEditor.GameObjectEditor;

namespace Frinkahedron.WinformsEditor
{
    public partial class Form1 : Form
    {
        private GameTemplateEditor gameEditor;
        private TreeNode modelsNode;
        private TreeNode objectsNode;

        public Form1()
        {
            InitializeComponent();
            gameEditor = new GameTemplateEditor();

            modelsNode = treeView1.Nodes.Add("Models");
            objectsNode = treeView1.Nodes.Add("Objects");
        }

        private void addObjectButton_Click(object sender, EventArgs e)
        {
            var template = new GameObjectTemplate();
            objectsNode.Expand();
            var node = objectsNode.Nodes.Add($"Object {objectsNode.Nodes.Count + 1}");
            node.Tag = template;
            gameEditor.Template.GameObjects.Add(template);
        }

        private void loadModelButton_Click(object sender, EventArgs e)
        {
            // TODO
            // Do model loading here, and change the game object editor to just pick one of the existing models
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode?.Tag is GameObjectTemplate got)
            {
                gameObjectEditorControl1.SetNewTemplate(got);
            }
        }
    }
}
