using Frinkahedron.Core.Template;

namespace Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels.Shapes
{
    internal sealed class SphereShapeViewModel(SphereTemplate sphereModel) : ShapeViewModelBase
    {
        public override string DisplayName => "Sphere";

        public override IShapeTemplate? Model => SphereModel;

        public SphereTemplate SphereModel => sphereModel;

        public float Radius
        {
            get => SphereModel.Radius;
            set => SetModelProperty(SphereModel.Radius, value, v => SphereModel.Radius = v);
        }
    }
}
