using Frinkahedron.Core;
using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Physics;
using Frinkahedron.Core.Template;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Frinkahedron.WinUIEditor.ViewModels.GameTemplateViewModels.Shapes
{
    internal sealed class SphereShapeViewModel : ShapeViewModelBase
    {
        private readonly SphereTemplate sphereModel;
        private readonly SphereRadiusGizmo gizmo;

        public SphereShapeViewModel(SphereTemplate sphereModel)
        {
            this.sphereModel = sphereModel;
            gizmo = new SphereRadiusGizmo(this);
        }

        public override string DisplayName => "Sphere";

        public override IShapeTemplate? Model => SphereModel;

        public SphereTemplate SphereModel => sphereModel;

        public float Radius
        {
            get => SphereModel.Radius;
            set => SetModelProperty(SphereModel.Radius, value, v => SphereModel.Radius = v, "Radius");
        }

        public override IReadOnlyList<IGizmo> GetGizmos()
        {
            return [gizmo];
        }
    }

    internal sealed class SphereRadiusGizmo(SphereShapeViewModel viewModel) : IGizmo
    {
        private const float POSITION_SCALE = 1.1F;

        public void Draw(bool mouseOver, bool mouseDragged, GameObject editableObject, GameState gameState, IRenderContext renderer)
        {
            var position = GetPosition(editableObject, gameState);

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
            var position = GetPosition(editableObject, gameState);
            return sph.RayIntersection(position, rayPos, rayDir, out _, out _);
        }

        public void OnDragged(GameObject editableObject, GameState gameState)
        {
            (var rayPos, var rayDir) = gameState.Scene.Camera.GetRay(gameState.Input.GetMouseNdcPosition());

            var line = new Line3(editableObject.Position.Centre, gameState.Scene.Camera.GetUp());

            var plane = new Frinkahedron.Core.Maths.Plane(editableObject.Position.Centre, gameState.Scene.Camera.LookDirection);
            var ray = new Line3(rayPos, rayDir);

            if (plane.RayPlaneIntersection(ray.Origin, ray.Direction, out var intersection))
            {
                viewModel.Radius = (1f / POSITION_SCALE) * MathF.Abs(Vector3.Dot(intersection, gameState.Scene.Camera.GetRight()));
            }
        }

        private Position GetPosition(GameObject editableObject, GameState gameState)
        {
            Vector3 camRight = gameState.Scene.Camera.GetRight();
            Vector3 centre = editableObject.Position.Centre + POSITION_SCALE * viewModel.Radius * camRight;
            return new Position(centre, Quaternion.Identity);
        }
    }
}
