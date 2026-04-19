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
using System.Text.Json;

namespace Frinkahedron.WinformsEditor
{
    public partial class Form1 : Form
    {
        private GameTemplateEditor gameEditor;
        private TreeNode modelsNode;
        private TreeNode objectsNode;
        private TreeNode levelsNode;
        private GraphicsService graphicsService;

        public Form1()
        {
            InitializeComponent();
            graphicsService = new GraphicsService();
            if (File.Exists($@"C:\tmp\tempgame.json"))
            {
                try
                {
                    using var fs = File.OpenRead($@"C:\tmp\tempgame.json");

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true, };
                    options.Converters.Add(new Vector3Converter());
                    var template = JsonSerializer.Deserialize<GameTemplate>(fs, options);
                    gameEditor = new GameTemplateEditor(template);
                }
                catch (Exception ex)
                {
                    gameEditor = new GameTemplateEditor();
                }
            }
            else
            {
                gameEditor = new GameTemplateEditor();
            }

            gameObjectEditorControl1.Initialise(gameEditor, graphicsService);
            levelEditorControl1.Initialise(gameEditor, graphicsService);

            modelsNode = treeView1.Nodes.Add("Models");
            objectsNode = treeView1.Nodes.Add("Objects");
            levelsNode = treeView1.Nodes.Add("Levels");

            if (gameEditor.Template.Models.Count > 0)
            {
                foreach (var model in gameEditor.Template.Models)
                {
                    var node = modelsNode.Nodes.Add(model.ModelID);
                    node.Tag = model;
                }
                modelsNode.Expand();
            }

            if (gameEditor.Template.GameObjects.Count > 0)
            {
                int index = 0;
                foreach (var obj in gameEditor.Template.GameObjects)
                {
                    var node = objectsNode.Nodes.Add($"Object {++index}");
                    node.Tag = obj;
                }
                objectsNode.Expand();
            }

            if (gameEditor.Template.Levels.Count == 0)
            {
                LevelTemplate levelTemplate = new LevelTemplate();
                var level = levelsNode.Nodes.Add("Level 1");
                level.Tag = levelTemplate;
                gameEditor.Template.Levels.Add(levelTemplate);
            }
            else
            {
                int index = 0;
                foreach (var levelTemplate in gameEditor.Template.Levels)
                {
                    var level = levelsNode.Nodes.Add($"Level {++index}");
                    level.Tag = levelTemplate;
                }
            }
            levelsNode.Expand();
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
                tabControl1.SelectedIndex = 0;
                gameObjectEditorControl1.SetNewTemplate(got);
            }
            else if (treeView1.SelectedNode?.Tag is LevelTemplate lt)
            {
                tabControl1.SelectedIndex = 1;
                levelEditorControl1.IsShown(lt);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IncludeFields = true, };
            options.Converters.Add(new Vector3Converter());

            using var fs = File.Create($@"C:\tmp\tempgame.json");
            JsonSerializer.Serialize<GameTemplate>(fs, gameEditor.Template, options);
        }
    }
}
