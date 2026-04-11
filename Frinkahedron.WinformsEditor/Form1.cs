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
        private GameObjectTemplateEditor editor;

        public Form1()
        {
            InitializeComponent();
            editor = new GameObjectTemplateEditor();
            veldridControl1.Initialise(editor);
            renderableTemplateControl1.Initialise(editor);
        }

        private void colliderControl1_ColliderChanged(object sender, IShapeTemplate e)
        {
            //TODO: Move this to collider control
            var template = editor.Template;
            template.Collider = e;
            editor.TemplateChangedCallback?.Invoke();
        }
    }
}
