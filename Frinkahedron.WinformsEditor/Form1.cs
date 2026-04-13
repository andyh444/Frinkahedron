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
        private GraphicsService graphicsService;

        public Form1()
        {
            InitializeComponent();
            graphicsService = new GraphicsService();
            gameEditor = new GameTemplateEditor();

            gameObjectEditorControl1.Initialise(gameEditor, graphicsService);

            modelsNode = treeView1.Nodes.Add("Models");
            objectsNode = treeView1.Nodes.Add("Objects");
        }

        private void addObjectButton_Click(object sender, EventArgs e)
        {
            var template = new GameObjectTemplate();
            var node = objectsNode.Nodes.Add($"Object {objectsNode.Nodes.Count + 1}");
            node.Tag = template;
            gameEditor.Template.GameObjects.Add(template);
            objectsNode.Expand();
        }

        private void loadModelButton_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = $"gltf files (*.gltf)|*.gltf",
            };

            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                string fileName = ofd.FileName;
                string modelID = new DirectoryInfo(Path.GetDirectoryName(fileName)!).Name;
                ModelTemplate modelTemplate = new ModelTemplate(modelID, fileName);
                var node = modelsNode.Nodes.Add(modelID);
                node.Tag = modelTemplate;
                gameEditor.Template.Models.Add(modelTemplate);
                modelsNode.Expand();
            }
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
