using System;
using System.Numerics;

namespace Frinkahedron.Core
{
    public abstract class Behaviour
    {
        public virtual void Update(GameObject self, GameState gameState)
        {
        }
    }

    public class OrbitalCameraMouseBehaviour : Behaviour
    {
        public float yaw = 0;
        public float pitch = 0;
        public float distance = 50f;
        public float sensitivity = 1f;
        public float minPitch = -0.45f * MathF.PI;
        public float maxPitch = 0.45f * MathF.PI;

        public override void Update(GameObject self, GameState gameState)
        {
            base.Update(self, gameState);

            if (gameState.Input.IsMouseButtonDown(MouseButton.Right))
            {
                Vector2 delta = gameState.Input.GetMouseDelta();
                yaw += delta.X * sensitivity * gameState.DeltaTime;
                pitch -= delta.Y * sensitivity * gameState.DeltaTime;
                pitch = Math.Clamp(pitch, minPitch, maxPitch);
            }
            var scrollDelta = gameState.Input.GetMouseScrollDelta();
            distance -= 250 * scrollDelta * gameState.DeltaTime;
            distance = Math.Clamp(distance, 1, 100);

            var rotation = Quaternion.CreateFromYawPitchRoll(-yaw, -pitch, 0f);
            Vector3 offset = Vector3.Transform(new Vector3(0, 0, -distance), rotation);

            gameState.Scene.Camera.SetValues(self.Position.Centre + offset, -offset);
        }
    }

    public class CompositeBehaviour(IReadOnlyList<Behaviour> behaviours) : Behaviour
    {
        public override void Update(GameObject self, GameState gameState)
        {
            base.Update(self, gameState);
            foreach (var behaviour in behaviours)
            {
                behaviour.Update(self, gameState);
            }
        }
    }

    public class ContinuousRotationBehaviour(float xRot, float yRot, float zRot) : Behaviour
    {
        public override void Update(GameObject self, GameState gameState)
        {
            float dt = gameState.DeltaTime;
            var rot = Quaternion.CreateFromYawPitchRoll(dt * yRot, dt * xRot, dt * zRot);
            self.Position.Orientation *= rot;
        }
    }

    public class KeyboardCameraMoveBehaviour : Behaviour
    {
        public override void Update(GameObject self, GameState gameState)
        {
            Vector3 translation = new Vector3();
            float yawAngle = 0;
            float pitchAngle = 0;
            if (gameState.Input.IsKeyDown(Key.W))
            {
                translation += gameState.DeltaTime * 5 * gameState.Scene.Camera.LookDirection;
            }
            if (gameState.Input.IsKeyDown(Key.S))
            {
                translation -= gameState.DeltaTime * 5 * gameState.Scene.Camera.LookDirection;
            }
            Vector3 camRight = gameState.Scene.Camera.GetRight();
            if (gameState.Input.IsKeyDown(Key.A))
            {
                translation -= gameState.DeltaTime * 5 * camRight;
            }
            if (gameState.Input.IsKeyDown(Key.D))
            {
                translation += gameState.DeltaTime * 5 * camRight;
            }

            if (gameState.Input.IsKeyDown(Key.Left))
            {
                yawAngle += gameState.DeltaTime * MathF.PI;
            }
            if (gameState.Input.IsKeyDown(Key.Right))
            {
                yawAngle -= gameState.DeltaTime * MathF.PI;
            }
            
            if (gameState.Input.IsKeyDown(Key.Up))
            {
                pitchAngle += gameState.DeltaTime * MathF.PI;
            }
            if (gameState.Input.IsKeyDown(Key.Down))
            {
                pitchAngle -= gameState.DeltaTime * MathF.PI;
            }

            if (translation.X != 0 || translation.Y != 0 || translation.Z != 0)
            {
                gameState.Scene.Camera.Translate(translation);
            }
            if (yawAngle != 0)
            {
                gameState.Scene.Camera.RotateYaw(yawAngle);
            }
            if (pitchAngle != 0)
            {
                gameState.Scene.Camera.RotatePitch(pitchAngle);
            }
        }
    }
}