using Frinkahedron.Core.Colliders;
using Frinkahedron.Core.Physics;
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
        public float pitch = -0.2f * MathF.PI;
        public float distance = 150f;
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
            distance = Math.Clamp(distance, 1, 500);

            var rotation = Quaternion.CreateFromYawPitchRoll(-yaw, -pitch, 0f);
            Vector3 offset = Vector3.Transform(new Vector3(0, 0, -distance), rotation);

            gameState.Scene.Camera.SetValues(self.Position.Centre + offset, -offset);

            
        }
    }

    public class CarCameraFollowBehaviour : Behaviour
    {
        public override void Update(GameObject self, GameState gameState)
        {
            Vector3 currentDirection = gameState.Scene.Camera.LookDirection;
            Vector3 targetDirection;
            if (MathF.Abs(self.RigidBody.Velocity.Length()) > 1)
            {
                targetDirection = Vector3.Normalize(self.RigidBody.Velocity);
            }
            else
            {
                targetDirection = Vector3.Transform(Vector3.UnitZ, self.Position.Orientation);
            }
            Vector3 right = Vector3.Cross(Vector3.UnitY, targetDirection);
            targetDirection = Vector3.Transform(targetDirection, Quaternion.CreateFromAxisAngle(right, MathF.PI / 16));

            Vector3 newDirection = Vector3.Lerp(currentDirection, targetDirection, 0.005f);
            gameState.Scene.Camera.SetValues(self.Position.Centre - 25 * newDirection + 1 * Vector3.UnitY, newDirection);
        }
    }

    public class CarBehaviour : Behaviour
    {
        const float ACCEL_FORCE = 40f;
        const float MAX_SPEED = 500f;
        const float DRAG = 0f;
        const float LATERAL_FRICTION = 8f;
        const float STEER_SPEED = 25.5f;

        public override void Update(GameObject self, GameState gameState)
        {
            float dt = gameState.DeltaTime;

            var rb = self.RigidBody;

            // Input
            int accelInput = 0;
            int steerInput = 0;

            if (gameState.Input.IsKeyDown(Key.W)) accelInput += 1;
            if (gameState.Input.IsKeyDown(Key.S)) accelInput -= 1;
            if (gameState.Input.IsKeyDown(Key.A)) steerInput -= 1;
            if (gameState.Input.IsKeyDown(Key.D)) steerInput += 1;

            if (accelInput == 0 && steerInput == 0)
            {
                return;
            }

            Vector3? groundNormal = GetGroundNormal(self, gameState.Scene);
            if (groundNormal is null)
            {
                // not in contact with ground
                return;
            }

            // Directions
            Vector3 forward = Vector3.Transform(Vector3.UnitZ, self.Position.Orientation);
            Vector3 up = groundNormal.Value;
            forward -= groundNormal.Value * Vector3.Dot(forward, up);
            forward = Vector3.Normalize(forward);
            Vector3 right = Vector3.Cross(up, forward);

            Vector3 velocity = rb.Velocity;

            // --- Decompose velocity ---
            float forwardSpeed = Vector3.Dot(velocity, forward);
            float lateralSpeed = Vector3.Dot(velocity, right);
            float verticalSpeed = Vector3.Dot(velocity, groundNormal.Value);

            // --- Apply acceleration ---
            forwardSpeed += accelInput * ACCEL_FORCE * dt;

            // Clamp max speed
            forwardSpeed = Math.Clamp(forwardSpeed, -MAX_SPEED, MAX_SPEED);

            // --- Apply lateral friction (kills sideways sliding) ---
            lateralSpeed *= MathF.Exp(-LATERAL_FRICTION * dt);

            // --- Rebuild velocity ---
            Vector3 planarVelocity = forward * forwardSpeed + right * lateralSpeed;
            velocity = planarVelocity + groundNormal.Value * verticalSpeed;

            // --- Apply drag ---
            velocity *= MathF.Exp(-DRAG * dt);

            rb.Velocity = velocity;

            // --- Steering (based on forward speed) ---
            float speedFactor = forwardSpeed / MAX_SPEED;
            float steerAmount = steerInput * STEER_SPEED * speedFactor * dt;

            if (Math.Abs(forwardSpeed) > 0.1f)
            {
                var steerQuat = Quaternion.CreateFromAxisAngle(groundNormal.Value, steerAmount);
                self.Position.Orientation = Quaternion.Normalize(self.Position.Orientation * steerQuat);
            }

            /*Vector3 currentUp = Vector3.Transform(Vector3.UnitY, self.Position.Orientation);

            Quaternion align = Quaternion.CreateFromAxisAngle(
                Vector3.Normalize(Vector3.Cross(currentUp, groundNormal.Value)),
                MathF.Acos(Math.Clamp(Vector3.Dot(currentUp, groundNormal.Value), -1f, 1f)) * 0.1f // smoothing
            );

            self.Position.Orientation = Quaternion.Normalize(align * self.Position.Orientation);*/
        }

        private Vector3? GetGroundNormal(GameObject self, Scene scene)
        {
            const float MAX_DIST = 1f;

            var dimensions = (self.Collider as Box).Dimensions;
            Vector3 point1 = self.Position.ToWorld(new Vector3(0, 0, dimensions.Z / 2));
            Vector3 point2 = self.Position.ToWorld(new Vector3(dimensions.X / 2, 0, -dimensions.Z / 2));
            Vector3 point3 = self.Position.ToWorld(new Vector3(-dimensions.X / 2, 0, -dimensions.Z / 2));

            Vector3 down = Vector3.Transform(-Vector3.UnitY, self.Position.Orientation);

            foreach (var obj in scene.Objects)
            {
                if (obj == self
                    || obj.Collider is null)
                {
                    continue;
                }
                bool r1 = obj.Collider.RayIntersection(obj.Position, point1, down, out Vector3 hitPoint1);
                bool r2 = obj.Collider.RayIntersection(obj.Position, point2, down, out Vector3 hitPoint2);
                bool r3 = obj.Collider.RayIntersection(obj.Position, point3, down, out Vector3 hitPoint3);

                if (r1
                    && r2
                    && r3
                    && Vector3.Distance(point1, hitPoint1) < MAX_DIST
                    && Vector3.Distance(point2, hitPoint2) < MAX_DIST
                    && Vector3.Distance(point3, hitPoint3) < MAX_DIST)
                {
                    return Vector3.Normalize(Vector3.Cross(hitPoint3 - hitPoint2, hitPoint3 - hitPoint1));
                }
            }
            return null;
        }
    }

    public class ImpulseOnClickBehaviour : Behaviour
    {
        public override void Update(GameObject self, GameState gameState)
        {
            if (gameState.Input.IsMouseButtonPressed(MouseButton.Left))
            {
                // doesn't belong here: move
                (var rayPosition, var rayDirection) = gameState.Scene.Camera.GetRay(gameState.Input.GetMouseNdcPosition());

                List<(GameObject, Vector3)> intersections = new List<(GameObject, Vector3)>();

                foreach (var obj in gameState.Scene.Objects)
                {
                    if (obj.Collider?.RayIntersection(obj.Position, rayPosition, rayDirection, out Vector3 hitPoint) == true)
                    {
                        intersections.Add((obj, hitPoint));
                        //obj.RigidBody?.ApplyImpulse(1 * rayDirection, hitPoint, obj.Position);

                        /*Sphere sph = new Sphere(0.5f);
                        float sphMass = 1 * sph.CalculateVolume();
                        GameObject sphObj = new GameObject(hitPoint - rayDirection * sph.Radius * 2,
                            null,
                            sph,
                            new RigidBody
                            {
                                Mass = sphMass,
                                InverseInertia = sph.CalculateFilledInertia(sphMass).GetInverse(),
                                Gravity = true,
                                Velocity = new Vector3(),
                                AngularVelocity = new Vector3(),
                            });
                        sphObj.Position.Orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI);
                        gameState.Scene.AddObject(sphObj);*/

                        //break;
                    }
                }
                if (intersections.Count > 0)
                {
                    (var closest, var closestHitPoint) = intersections.MinBy(x => Vector3.DistanceSquared(x.Item2, rayPosition));

                    closest.RigidBody?.ApplyImpulse(20 * rayDirection, closestHitPoint - closest.Position.Centre, closest.Position);
                }
            }
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

    public class CapsuleControlBehaviour : Behaviour
    {
        public override void Update(GameObject self, GameState gameState)
        {
            base.Update(self, gameState);
            if (gameState.Input.IsKeyDown(Key.W))
            {
                Vector3 capsuleAxis = Vector3.Transform(Vector3.UnitY, self.Position.Orientation);
                self.RigidBody.ApplyTorque(1000 * capsuleAxis, gameState.DeltaTime, self.Position);
            }
            if (gameState.Input.IsKeyDown(Key.S))
            {
                Vector3 capsuleAxis = Vector3.Transform(Vector3.UnitY, self.Position.Orientation);
                self.RigidBody.ApplyTorque(-1000 * capsuleAxis, gameState.DeltaTime, self.Position);
            }
            if (gameState.Input.IsKeyDown(Key.A))
            {
                Vector3 capsuleAxis = Vector3.UnitY;// Vector3.Transform(Vector3.UnitY, self.Position.Orientation);
                self.RigidBody.ApplyTorque(-1000 * capsuleAxis, gameState.DeltaTime, self.Position);
            }
            if (gameState.Input.IsKeyDown(Key.D))
            {
                Vector3 capsuleAxis = Vector3.UnitY;// Vector3.Transform(Vector3.UnitY, self.Position.Orientation);
                self.RigidBody.ApplyTorque(1000 * capsuleAxis, gameState.DeltaTime, self.Position);
            }
        }
    }

    public class SphereControlBehaviour : Behaviour
    {
        public override void Update(GameObject self, GameState gameState)
        {
            base.Update(self, gameState);
            if (gameState.Input.IsKeyDown(Key.W))
            {
                Vector3 axis = gameState.Scene.Camera.GetRight();
                self.RigidBody.ApplyTorque(-10000 * axis, gameState.DeltaTime, self.Position);
            }
            if (gameState.Input.IsKeyDown(Key.S))
            {
                Vector3 axis = gameState.Scene.Camera.GetRight();
                self.RigidBody.ApplyTorque(10000 * axis, gameState.DeltaTime, self.Position);
            }
            if (gameState.Input.IsKeyDown(Key.A))
            {
                Vector3 axis = gameState.Scene.Camera.LookDirection;
                self.RigidBody.ApplyTorque(-10000 * axis, gameState.DeltaTime, self.Position);
            }
            if (gameState.Input.IsKeyDown(Key.D))
            {
                Vector3 axis = gameState.Scene.Camera.LookDirection;
                self.RigidBody.ApplyTorque(10000 * axis, gameState.DeltaTime, self.Position);
            }
        }
    }
}