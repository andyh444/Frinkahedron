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

        public float pitch = 0;
        public float yaw = 0;
        public float sensitivity = 0.5f;
        public float minPitch = -0.45f * MathF.PI;
        public float maxPitch = 0.45f * MathF.PI;

        public LevelViewerBehaviour(GameTemplateEditor gameEditor, LevelTemplateEditor levelEditor)
        {
            this.gameEditor = gameEditor;
            this.levelEditor = levelEditor;
        }

        public override void Update(GameObject self, GameState gameState)
        {
            base.Update(self, gameState);

            var currentPosition = gameState.Scene.Camera.Position;

            if (gameState.Input.IsMouseButtonDown(MouseButton.Right))
            {
                Vector2 delta = gameState.Input.GetMouseDelta();
                yaw -= delta.X * sensitivity * gameState.DeltaTime;
                pitch += delta.Y * sensitivity * gameState.DeltaTime;
                pitch = Math.Clamp(pitch, minPitch, maxPitch);
            }

            if (gameState.Input.IsKeyDown(Key.A))
            {
                currentPosition -= 5 * gameState.DeltaTime * gameState.Scene.Camera.GetRight();
            }
            if (gameState.Input.IsKeyDown(Key.D))
            {
                currentPosition += 5 * gameState.DeltaTime * gameState.Scene.Camera.GetRight();
            }
            if (gameState.Input.IsKeyDown(Key.W))
            {
                currentPosition += 5 * gameState.DeltaTime * gameState.Scene.Camera.LookDirection;
            }
            if (gameState.Input.IsKeyDown(Key.S))
            {
                currentPosition -= 5 * gameState.DeltaTime * gameState.Scene.Camera.LookDirection;
            }
            if (gameState.Input.IsKeyDown(Key.Q))
            {
                currentPosition -= 5 * gameState.DeltaTime * gameState.Scene.Camera.GetUp();
            }
            if (gameState.Input.IsKeyDown(Key.E))
            {
                currentPosition += 5 * gameState.DeltaTime * gameState.Scene.Camera.GetUp();
            }

            var direction = Vector3.Transform(Vector3.UnitZ, Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0));
            gameState.Scene.Camera.SetValues(currentPosition, direction);


            if (gameState.Input.IsMouseButtonPressed(MouseButton.Left))
            {
                (var rayPos, var rayDir) = gameState.Scene.Camera.GetRay(gameState.Input.GetMouseNdcPosition());

                float bestDistance = float.PositiveInfinity;
                Vector3 bestIntersection = default;
                foreach (var obj in gameState.Scene.Objects)
                {
                    
                    if (obj.Collider?.RayIntersection(obj.Position, rayPos, rayDir, out Vector3 result, out _) == true)
                    {
                        if (Vector3.DistanceSquared(result, rayPos) < bestDistance)
                        {
                            bestIntersection = result;
                            bestDistance = Vector3.DistanceSquared(result, rayPos);
                        }
                    }
                }

                if (!float.IsPositiveInfinity(bestDistance))
                {
                    levelEditor.Template.LevelObjects.Add(new LevelObjectTemplate
                    {
                        GameObjectIndex = 0, // TODO
                        WorldTransform = new TransformTemplate { Translation = bestIntersection } // TODO: This would need to be translated so it doesn't intersect with the existing object
                    });
                    levelEditor.TemplateChangedCallback?.Invoke();
                }

                else if (new Plane(Vector3.Zero, Vector3.UnitY).RayPlaneIntersection(rayPos, rayDir, out var intersection))
                {
                    levelEditor.Template.LevelObjects.Add(new LevelObjectTemplate
                    {
                        GameObjectIndex = 0, // TODO
                        WorldTransform = new TransformTemplate { Translation = intersection }
                    });
                    levelEditor.TemplateChangedCallback?.Invoke();
                }
            }
        }

        
    }
}
