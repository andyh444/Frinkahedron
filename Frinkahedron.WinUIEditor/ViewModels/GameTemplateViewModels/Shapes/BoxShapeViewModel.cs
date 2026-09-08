using Frinkahedron.Core;
using Frinkahedron.Core.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels.Shapes
{

    internal sealed class BoxShapeViewModel(BoxTemplate boxModel) : ShapeViewModelBase
    {
        public override string DisplayName => "Box";

        public override IShapeTemplate? Model => BoxModel;

        public BoxTemplate BoxModel => boxModel;

        public float DimX
        {
            get => BoxModel.Dimensions.X;
            set => SetModelProperty(BoxModel.Dimensions.X, value, v => BoxModel.Dimensions = new Vector3(v, BoxModel.Dimensions.Y, BoxModel.Dimensions.Z), "DimX");
        }

        public float DimY
        {
            get => BoxModel.Dimensions.Y;
            set => SetModelProperty(BoxModel.Dimensions.Y, value, v => BoxModel.Dimensions = new Vector3(BoxModel.Dimensions.X, v, BoxModel.Dimensions.Z), "DimY");
        }

        public float DimZ
        {
            get => BoxModel.Dimensions.Z;
            set => SetModelProperty(BoxModel.Dimensions.Z, value, v => BoxModel.Dimensions = new Vector3(BoxModel.Dimensions.X, BoxModel.Dimensions.Y, v), "DimZ");
        }
    }

    internal sealed class BoxDimensionGizmo : IGizmo
    {
        public void Draw(bool mouseOver, bool mouseDragged, GameObject editableObject, IRenderContext renderer)
        {
            throw new NotImplementedException();
        }

        public bool IsMouseOver(GameObject editableObject, Input input)
        {
            throw new NotImplementedException();
        }

        public void OnDragged(GameObject editableObject, Input input)
        {
            throw new NotImplementedException();
        }
    }
}
