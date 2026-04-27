using Frinkahedron.Core.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frinkahedron.WinformsEditor.GameObjectEditor
{
    public partial class GameObjectEditorControl : UserControl
    {
        private GameObjectTemplateEditor objectEditor;
        private GameTemplateEditor? gameEditor;

        public GameObjectEditorControl()
        {
            InitializeComponent();
            objectEditor = new GameObjectTemplateEditor();
        }

        public void Initialise(GameTemplateEditor gameEditor, GraphicsService graphicsService)
        {
            this.gameEditor = gameEditor;
            veldridControl1.Initialise(gameEditor, objectEditor, graphicsService);
            renderableTemplateControl1.Initialise(gameEditor, objectEditor);
            colliderControl1.Initialise(objectEditor);
            rigidBodyControl1.Initialise(objectEditor);
        }

        public void SetNewTemplate(GameObjectTemplate template)
        {
            objectEditor.Template = template;

            renderableTemplateControl1.Initialise(gameEditor, objectEditor);
            colliderControl1.Initialise(objectEditor);
            rigidBodyControl1.Initialise(objectEditor);
        }

        public GameObjectTemplateEditor GetObjectEditor() => objectEditor;
    }
}
