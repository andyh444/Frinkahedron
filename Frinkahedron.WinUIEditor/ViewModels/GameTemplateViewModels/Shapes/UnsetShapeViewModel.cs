using Frinkahedron.Core.Template;

namespace Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels.Shapes
{
    internal sealed class UnsetShapeViewModel : ShapeViewModelBase
    {
        public override string DisplayName => "None";

        public override IShapeTemplate? Model => null;
    }
}
