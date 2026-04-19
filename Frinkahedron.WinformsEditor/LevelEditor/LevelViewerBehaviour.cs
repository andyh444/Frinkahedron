using Frinkahedron.Core;
using Frinkahedron.Core.Maths;
using Frinkahedron.Core.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Plane = Frinkahedron.Core.Maths.Plane;

namespace Frinkahedron.WinformsEditor.LevelEditor
{
    internal sealed class LevelViewerBehaviour : Behaviour
    {
        private readonly GameTemplateEditor gameEditor;
        private readonly LevelTemplateEditor levelEditor;
        private readonly Func<int> getObjIndex;

        public float pitch = 0;
        public float yaw = 0;
        public float sensitivity = 0.5f;
        public float minPitch = -0.45f * MathF.PI;
        public float maxPitch = 0.45f * MathF.PI;

        private bool isOrbiting = false;
        private float orbitDist;
        private Vector3 orbitCentre;
        private Vector3 pivotDiff;

        public LevelViewerBehaviour(GameTemplateEditor gameEditor, LevelTemplateEditor levelEditor, Func<int> getObjIndex)
        {
            this.gameEditor = gameEditor;
            this.levelEditor = levelEditor;
            this.getObjIndex = getObjIndex;
        }

        public override void Update(GameObject self, GameState gameState)
        {
            base.Update(self, gameState);

            var cam = gameState.Scene.Camera;

            if (gameState.Input.IsMouseButtonDown(MouseButton.Right))
            {
                if (gameState.Input.IsKeyDown(Key.ShiftKey))
                {
                    float panSensitivity = 0.001f;
                    if (SceneRayCast(gameState, out var rayPos, out var intersect, out _))
                    {
                        panSensitivity = 0.001f * Vector3.Distance(rayPos, intersect);
                    }
                    isOrbiting = false;
                    Vector2 delta = gameState.Input.GetMouseDelta();
                    var camPos = cam.Position;
                    var delta3 = panSensitivity * (-delta.X * cam.GetRight() + delta.Y * cam.GetUp());
                    cam.SetValues(camPos + delta3, cam.LookDirection);
                }
                else
                {
                    if (!isOrbiting)
                    {
                        if (SceneRayCast(gameState, out var rayPos, out var centreOfRotation, out _))
                        {
                            isOrbiting = true;
                            orbitDist = Vector3.Distance(rayPos, centreOfRotation);
                            orbitCentre = centreOfRotation;

                            pivotDiff = cam.Position - orbitCentre;
                        }
                    }

                    if (isOrbiting)
                    {
                        var oldQuat = Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0);

                        Vector2 delta = gameState.Input.GetMouseDelta();
                        yaw -= delta.X * sensitivity * gameState.DeltaTime;
                        pitch += delta.Y * sensitivity * gameState.DeltaTime;
                        pitch = Math.Clamp(pitch, minPitch, maxPitch);

                        var newQuat = Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0);
                        var direction = Vector3.Transform(Vector3.UnitZ, newQuat);

                        var rotationDelta = newQuat * Quaternion.Inverse(oldQuat);
                        pivotDiff = Vector3.Transform(pivotDiff, rotationDelta);

                        var camPos = orbitCentre + pivotDiff;
                        cam.SetValues(camPos, direction);
                    }
                }
            }
            else
            {
                isOrbiting = false;
                if (gameState.Input.IsKeyDown(Key.ShiftKey))
                {
                    // move cam up/down
                    float scrollSensitivity = 0.05f;
                    int delta = gameState.Input.GetMouseScrollDelta();
                    cam.SetValues(cam.Position + scrollSensitivity * delta * cam.GetUp(), cam.LookDirection);
                }
                else
                {
                    // zoom in/out
                    // TODO: Zoom amount should change based on distance to scene
                    if (cam.ProjectionType is ProjectionType.Perspective)
                    {
                        (_, var rayDir) = gameState.Scene.Camera.GetRay(gameState.Input.GetMouseNdcPosition());

                        float scrollSensitivity = 0.05f;
                        int delta = gameState.Input.GetMouseScrollDelta();
                        cam.SetValues(cam.Position + scrollSensitivity * delta * rayDir, cam.LookDirection);
                    }
                    else if (cam.ProjectionType is ProjectionType.Orthographic)
                    {
                        // TODO Orthographic zoom
                    }
                }
            }
            


            if (gameState.Input.IsMouseButtonPressed(MouseButton.Left))
            {
                if (SceneRayCast(gameState, out _, out var intersect, out var intersectNormal))
                {
                    levelEditor.Template.LevelObjects.Add(new LevelObjectTemplate
                    {
                        GameObjectIndex = getObjIndex(),
                        WorldTransform = new TransformTemplate { Translation = intersect + 0.5f * intersectNormal } // TODO: translate based on AABB rather than just 0.5
                    });
                    levelEditor.TemplateChangedCallback?.Invoke();
                }

            }
        }

        private bool SceneRayCast(GameState gameState, out Vector3 rayPos, out Vector3 intersect, out Vector3 normal)
        {
            (rayPos, var rayDir) = gameState.Scene.Camera.GetRay(gameState.Input.GetMouseNdcPosition());

            float bestDistance = float.PositiveInfinity;
            Vector3 bestIntersection = default;
            Vector3 bestNormal = default;
            foreach (var obj in gameState.Scene.Objects)
            {

                if (obj.Collider?.RayIntersection(obj.Position, rayPos, rayDir, out Vector3 result, out Vector3 n) == true)
                {
                    if (Vector3.DistanceSquared(result, rayPos) < bestDistance)
                    {
                        bestIntersection = result;
                        bestNormal = n;
                        bestDistance = Vector3.DistanceSquared(result, rayPos);
                    }
                }
            }

            if (!float.IsPositiveInfinity(bestDistance))
            {
                intersect = bestIntersection;
                normal = bestNormal;
                return true;
            }

            else if (new Plane(Vector3.Zero, Vector3.UnitY).RayPlaneIntersection(rayPos, rayDir, out var intersection))
            {
                intersect = intersection;
                normal = Vector3.UnitY;
                return true;
            }
            intersect = default;
            normal = default;
            return false;
        }
    }
}
