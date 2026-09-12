using Frinkahedron.Core;
using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using Frinkahedron.Core.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            set => SetModelProperty(BoxModel.Dimensions.X, value, v => BoxModel.Dimensions = BoxModel.Dimensions with { X = v }, "DimX");
        }

        public float DimY
        {
            get => BoxModel.Dimensions.Y;
            set => SetModelProperty(BoxModel.Dimensions.Y, value, v => BoxModel.Dimensions = BoxModel.Dimensions with { Y = v }, "DimY");
        }

        public float DimZ
        {
            get => BoxModel.Dimensions.Z;
            set => SetModelProperty(BoxModel.Dimensions.Z, value, v => BoxModel.Dimensions = BoxModel.Dimensions with { Z = v }, "DimZ");
        }

        public override IReadOnlyList<IGizmo> GetGizmos()
        {
            return [new BoxDimensionGizmo(this)];
        }
    }

    internal sealed class BoxDimensionGizmo(BoxShapeViewModel viewModel) : IGizmo
    {
        private const float POSITION_SCALE = 0.6F;

        public void Draw(bool mouseOver, bool mouseDragged, GameObject editableObject, IRenderContext renderer)
        {
            var position = new Position(editableObject.Position.Centre
                        + new Vector3(POSITION_SCALE * viewModel.DimX, 0, 0), Quaternion.Identity);

            float radius = 3f;
            if (mouseOver)
            {
                radius = 5f;
            }
            if (mouseDragged)
            {
                radius = 7f;
            }
            var transform = Matrix4x4.CreateScale(radius) * position.ToMatrix();

            renderer.DrawPrimitiveWireframe(Primitive.Ellipsoid, transform);
        }

        public bool IsMouseOver(GameObject editableObject, GameState gameState)
        {
            Sphere sph = new Sphere(3f);

            (var rayPos, var rayDir) = gameState.Scene.Camera.GetRay(gameState.Input.GetMouseNdcPosition());
            var position = new Position(editableObject.Position.Centre + new Vector3(POSITION_SCALE * viewModel.DimX, 0, 0), Quaternion.Identity);
            return sph.RayIntersection(position, rayPos, rayDir, out _, out _);
        }

        public void OnDragged(GameObject editableObject, GameState gameState)
        {
            (var rayPos, var rayDir) = gameState.Scene.Camera.GetRay(gameState.Input.GetMouseNdcPosition());
            var plane = new Frinkahedron.Core.Maths.Plane(editableObject.Position.Centre, Vector3.UnitZ);

            var line = new Line3(editableObject.Position.Centre, Vector3.UnitX);
            var closest = line.ClosestPointTo(new Line3(rayPos, rayDir));

                // TODO This currently causes the screen to flicker because it creates a new scene each time
                viewModel.DimX = (1f / POSITION_SCALE) * (closest - plane.Point).X;
        }
    }
}
