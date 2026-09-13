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

    internal sealed class BoxShapeViewModel : ShapeViewModelBase
    {
        private readonly BoxTemplate boxModel;
        private readonly IReadOnlyList<IGizmo> gizmos;

        public BoxShapeViewModel(BoxTemplate boxModel)
        {
            this.boxModel = boxModel;
            gizmos = [new BoxDimensionGizmo(this, Vector3.UnitX), new BoxDimensionGizmo(this, Vector3.UnitY), new BoxDimensionGizmo(this, Vector3.UnitZ)];
        }

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
            return gizmos;
        }
    }

    internal sealed class BoxDimensionGizmo(BoxShapeViewModel viewModel, Vector3 dir) : IGizmo
    {
        private const float POSITION_SCALE = 0.6F;

        public void Draw(bool mouseOver, bool mouseDragged, GameObject editableObject, GameState gameState, IRenderContext renderer)
        {
            var position = GetPosition(editableObject);

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

        private Position GetPosition(GameObject editableObject)
            => new Position(editableObject.Position.Centre + POSITION_SCALE * Vector3.Dot(viewModel.BoxModel.Dimensions, dir) * dir, Quaternion.Identity);

        public bool IsMouseOver(GameObject editableObject, GameState gameState)
        {
            Sphere sph = new Sphere(3f);

            (var rayPos, var rayDir) = gameState.Scene.Camera.GetRay(gameState.Input.GetMouseNdcPosition());
            var position = GetPosition(editableObject);
            return sph.RayIntersection(position, rayPos, rayDir, out _, out _);
        }

        public void OnDragged(GameObject editableObject, GameState gameState)
        {
            (var rayPos, var rayDir) = gameState.Scene.Camera.GetRay(gameState.Input.GetMouseNdcPosition());

            var line = new Line3(editableObject.Position.Centre, dir);
            var closest = line.ClosestPointTo(new Line3(rayPos, rayDir));

            // TODO This currently causes the screen to flicker because it creates a new scene each time
            Vector3 translation = (1f / POSITION_SCALE) * (closest - line.Origin);

            // TODO: This but better
            if (dir == Vector3.UnitX)
            {
                viewModel.DimX = translation.X;
            }
            else if (dir == Vector3.UnitY)
            {
                viewModel.DimY = translation.Y;
            }
            else if (dir == Vector3.UnitZ)
            {
                viewModel.DimZ = translation.Z;
            }
        }
    }
}
