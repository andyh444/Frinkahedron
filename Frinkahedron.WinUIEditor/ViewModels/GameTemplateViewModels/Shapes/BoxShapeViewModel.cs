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

            float radius = GetRadius(gameState.Scene.Camera, position.Centre, 0.01f);
            if (mouseOver)
            {
                radius = GetRadius(gameState.Scene.Camera, position.Centre, 0.015f);
            }
            if (mouseDragged)
            {
                radius = GetRadius(gameState.Scene.Camera, position.Centre, 0.02f);
            }
            var transform = Matrix4x4.CreateScale(radius) * position.ToMatrix();

            renderer.DrawPrimitiveWireframe(Primitive.Ellipsoid, transform);
        }

        private Position GetPosition(GameObject editableObject)
            => new Position(editableObject.Position.Centre + POSITION_SCALE * Vector3.Dot(viewModel.BoxModel.Dimensions, dir) * dir, Quaternion.Identity);

        public bool IsMouseOver(GameObject editableObject, GameState gameState)
        {
            var position = GetPosition(editableObject);
            Sphere sph = new Sphere(GetRadius(gameState.Scene.Camera, position.Centre, 0.01f));

            (var rayPos, var rayDir) = gameState.Scene.Camera.GetRay(gameState.Input.GetMouseNdcPosition());
            return sph.RayIntersection(position, rayPos, rayDir, out _, out _);
        }

        public void OnDragged(GameObject editableObject, GameState gameState)
        {
            (var rayPos, var rayDir) = gameState.Scene.Camera.GetRay(gameState.Input.GetMouseNdcPosition());

            var line = new Line3(editableObject.Position.Centre, dir);
            var closest = line.ClosestPointTo(new Line3(rayPos, rayDir));

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

        private float GetRadius(Camera camera, Vector3 position, float screenFraction)
        {
            if (camera.ProjectionType == ProjectionType.Orthographic)
            {
                throw new NotImplementedException();
            }
            // Camera-space position
            Vector4 viewPosition = Vector4.Transform(
                new Vector4(position, 1.0f),
                camera.ViewMatrix);

            float distance = -viewPosition.Z;

            // World-space radius required to occupy the desired
            // fraction of the screen height.
            float requiredRadius =
                distance *
                MathF.Tan((camera.Projection as PerspectiveProjection).FoV * 0.5f) *
                screenFraction;

            return requiredRadius / 0.5f;
        }
    }
}
