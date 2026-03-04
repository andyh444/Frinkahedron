using System.Numerics;

namespace Frinkahedron.Core
{
    public abstract class Behaviour
    {
        public virtual void Update(GameObject self, GameState gameState)
        {
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
            self.RotateX += xRot * gameState.DeltaTime;
            self.RotateY += yRot * gameState.DeltaTime;
            self.RotateZ += zRot * gameState.DeltaTime;
        }
    }

    public class KeyboardCameraMoveBehaviour : Behaviour
    {
        public override void Update(GameObject self, GameState gameState)
        {
            Vector3 translation = new Vector3();
            float yawAngle = 0;
            float pitchAngle = 0;
            if (gameState.KeysDown.Contains(Key.W))
            {
                translation += gameState.DeltaTime * 5 * gameState.Scene.Camera.LookDirection;
            }
            if (gameState.KeysDown.Contains(Key.S))
            {
                translation -= gameState.DeltaTime * 5 * gameState.Scene.Camera.LookDirection;
            }
            Vector3 camRight = gameState.Scene.Camera.GetRight();
            if (gameState.KeysDown.Contains(Key.A))
            {
                translation -= gameState.DeltaTime * 5 * camRight;
            }
            if (gameState.KeysDown.Contains(Key.D))
            {
                translation += gameState.DeltaTime * 5 * camRight;
            }

            if (gameState.KeysDown.Contains(Key.Left))
            {
                yawAngle += gameState.DeltaTime * MathF.PI;
            }
            if (gameState.KeysDown.Contains(Key.Right))
            {
                yawAngle -= gameState.DeltaTime * MathF.PI;
            }
            
            if (gameState.KeysDown.Contains(Key.Up))
            {
                pitchAngle += gameState.DeltaTime * MathF.PI;
            }
            if (gameState.KeysDown.Contains(Key.Down))
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