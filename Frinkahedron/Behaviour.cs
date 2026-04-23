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
            
            distance -= 10 * distance * scrollDelta * gameState.DeltaTime;
            distance = Math.Clamp(distance, 1, 500);

            if (gameState.Scene.Camera.ProjectionType is ProjectionType.Orthographic
                && gameState.Scene.Camera.Projection is OrthographicProjection op)
            {
                op.Width -= 10 * op.Width * scrollDelta * gameState.DeltaTime;
            }

            var rotation = Quaternion.CreateFromYawPitchRoll(-yaw, -pitch, 0f);
            Vector3 offset = Vector3.Transform(new Vector3(0, 0, -distance), rotation);

            gameState.Scene.Camera.SetValues(self.Position.Centre + offset, -offset);

            
        }
    }

    public class CarCameraFollowBehaviour : Behaviour
    {
        public float yawOverride = 0;
        public float pitchOverride = 0;
        public float sensitivity = 1f;
        public float minPitch = -0.45f * MathF.PI;
        public float maxPitch = 0.45f * MathF.PI;

        private float? _smoothedYaw = null;
        private float _smoothedPitch = 0f;
        private float _baseDistance = 5f;
        private float _maxDistance = 6f;
        private float _heightOffset = 0.2f;
        private float _tiltAngle = MathF.PI / 16f;
        private float _speedThreshold = 3f;
        private float _reverseTransitionSpeed = 8f;

        public override void Update(GameObject self, GameState gameState)
        {
            float dt = gameState.DeltaTime;
            Vector3 carForward = Vector3.Transform(Vector3.UnitZ, self.Position.Orientation);
            float currentSpeed = self.RigidBody.Velocity.Length();
            float forwardSpeed = Vector3.Dot(self.RigidBody.Velocity, carForward);

            // --- Determine target direction ---
            Vector3 targetDirection;
            if (currentSpeed > _speedThreshold)
            {
                // Use velocity direction, but only flip to reverse at a higher threshold
                // to avoid jittery transitions
                Vector3 velocityDir = Vector3.Normalize(self.RigidBody.Velocity);

                if (forwardSpeed < -_reverseTransitionSpeed)
                {
                    // Clearly reversing: look backward
                    targetDirection = velocityDir;
                }
                else if (forwardSpeed < 0)
                {
                    // Transitioning to reverse: stick with car forward to avoid sudden flip
                    targetDirection = carForward;
                }
                else
                {
                    targetDirection = velocityDir;
                }
            }
            else
            {
                // At low speed, use car's forward orientation
                targetDirection = carForward;
            }

            // --- Decompose target into yaw and pitch ---
            float targetYaw = MathF.Atan2(targetDirection.X, targetDirection.Z);
            float targetPitch = -_tiltAngle; // subtle downward tilt

            // --- Mouse override ---
            if (gameState.Input.IsMouseButtonDown(MouseButton.Right))
            {
                Vector2 delta = gameState.Input.GetMouseDelta();
                yawOverride += delta.X * sensitivity * dt;
                pitchOverride -= delta.Y * sensitivity * dt;
                pitchOverride = Math.Clamp(pitchOverride, minPitch, maxPitch);
                targetYaw -= yawOverride;
                targetPitch -= pitchOverride;
            }
            else
            {
                // Smoothly decay overrides back to zero
                yawOverride *= MathF.Exp(-5f * dt);
                pitchOverride *= MathF.Exp(-5f * dt);
                if (MathF.Abs(yawOverride) < 0.001f) yawOverride = 0f;
                if (MathF.Abs(pitchOverride) < 0.001f) pitchOverride = 0f;
            }

            // --- Compute stiffness using a smoother curve ---
            float stiffness;
            if (gameState.Input.IsMouseButtonDown(MouseButton.Right))
            {
                stiffness = 1f;
            }
            else
            {
                // Smooth ramp: speed / (speed + k), reaches ~0.5 at speed=k
                float normalizedSpeed = currentSpeed / (currentSpeed + 10f);
                stiffness = Math.Clamp(normalizedSpeed * 4f * dt, 0.02f * dt, 1f);
            }

            // --- Initialize on first frame ---
            if (_smoothedYaw is null)
            {
                _smoothedYaw = targetYaw;
                _smoothedPitch = targetPitch;
            }

            // --- Interpolate yaw using angular shortest-path ---
            float yawDiff = targetYaw - _smoothedYaw.Value;
            // Wrap to [-PI, PI] so the camera always takes the short way around
            yawDiff = MathF.IEEERemainder(yawDiff, 2f * MathF.PI);
            _smoothedYaw += yawDiff * Math.Clamp(stiffness, 0f, 1f);

            // --- Interpolate pitch linearly ---
            _smoothedPitch += (targetPitch - _smoothedPitch) * Math.Clamp(stiffness, 0f, 1f);

            // --- Reconstruct direction from yaw/pitch ---
            Vector3 newDirection = new Vector3(
                MathF.Cos(_smoothedPitch) * MathF.Sin(_smoothedYaw.Value),
                MathF.Sin(_smoothedPitch),
                MathF.Cos(_smoothedPitch) * MathF.Cos(_smoothedYaw.Value)
            );
            newDirection = Vector3.Normalize(newDirection);

            // --- Distance varies with speed ---
            float speedRatio = Math.Clamp(currentSpeed / 100f, 0f, 1f);
            float distance = float.Lerp(_baseDistance, _maxDistance, speedRatio);

            gameState.Scene.Camera.SetValues(
                self.Position.Centre - distance * newDirection + _heightOffset * Vector3.UnitY,
                newDirection);
        }
    }

    public class CarBehaviour : Behaviour
    {
        const float ACCEL_FORCE = 80f;
        const float MAX_SPEED = 800f;
        const float LATERAL_FRICTION = 8f;
        const float HANDBRAKE_LATERAL_FRICTION = 1.5f;
        const float HANDBRAKE_FORWARD_DRAG = 3f;
        const float HANDBRAKE_STEER_SPEED = 75f;
        const float STEER_SPEED = 25.5f;

        public override void Update(GameObject self, GameState gameState)
        {
            float dt = gameState.DeltaTime;

            var rb = self.RigidBody as DynamicBody;

            // Input
            int accelInput = 0;
            int steerInput = 0;
            bool handbrakeOn = false;

            if (gameState.Input.IsKeyDown(Key.W)) accelInput += 1;
            if (gameState.Input.IsKeyDown(Key.S)) accelInput -= 1;
            if (gameState.Input.IsKeyDown(Key.A)) steerInput -= 1;
            if (gameState.Input.IsKeyDown(Key.D)) steerInput += 1;
            handbrakeOn = gameState.Input.IsKeyDown(Key.Space);

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
            if (!handbrakeOn)
            {
                forwardSpeed += accelInput * ACCEL_FORCE * dt;
            }
            else
            {
                // Handbrake locks rear wheels: apply forward drag to simulate braking
                forwardSpeed *= MathF.Exp(-HANDBRAKE_FORWARD_DRAG * dt);

                // Still allow a small amount of throttle input to power through drifts
                forwardSpeed += accelInput * ACCEL_FORCE * 0.3f * dt;
            }

            // Clamp max speed
            forwardSpeed = Math.Clamp(forwardSpeed, -MAX_SPEED, MAX_SPEED);

            // --- Apply lateral friction ---
            // Handbrake reduces grip, allowing the car to slide sideways (drift)
            float lateralFriction = handbrakeOn ? HANDBRAKE_LATERAL_FRICTION : LATERAL_FRICTION;
            lateralSpeed *= MathF.Exp(-lateralFriction * dt);

            // --- Rebuild velocity ---
            rb.Velocity = forward * forwardSpeed
                + right * lateralSpeed
                + groundNormal.Value * verticalSpeed;

            // --- Steering (based on forward speed) ---
            float speedFactor = forwardSpeed / MAX_SPEED;
            var steerSpeed = handbrakeOn ? HANDBRAKE_STEER_SPEED : STEER_SPEED;
            float steerAmount = steerInput * steerSpeed * speedFactor * dt;

            if (Math.Abs(forwardSpeed) > 0.1f)
            {
                var steerQuat = Quaternion.CreateFromAxisAngle(groundNormal.Value, steerAmount);
                self.Position.Orientation = Quaternion.Normalize(self.Position.Orientation * steerQuat);
            }
        }

        private Vector3? GetGroundNormal(GameObject self, Scene scene)
        {
            const float MAX_DIST = 2f;

            var dimensions = (self.Collider as Box).Dimensions;
            Vector3 point1 = self.Position.ToWorld(new Vector3(0, 0, dimensions.Z / 2));
            Vector3 point2 = self.Position.ToWorld(new Vector3(dimensions.X / 2, 0, -dimensions.Z / 2));
            Vector3 point3 = self.Position.ToWorld(new Vector3(-dimensions.X / 2, 0, -dimensions.Z / 2));

            Vector3 down = Vector3.Transform(-Vector3.UnitY, self.Position.Orientation);

            bool r1 = false;
            bool r2 = false;
            bool r3 = false;
            Vector3 hitPoint1 = default;
            Vector3 hitPoint2 = default;
            Vector3 hitPoint3 = default;

            foreach (var obj in scene.Objects)
            {
                if (obj == self
                    || obj.Collider is null)
                {
                    continue;
                }
                r1 = r1 || obj.Collider.RayIntersection(obj.Position, point1, down, out hitPoint1, out _);
                r2 = r2 || obj.Collider.RayIntersection(obj.Position, point2, down, out hitPoint2, out _);
                r3 = r3 || obj.Collider.RayIntersection(obj.Position, point3, down, out hitPoint3, out _);
            }
            if (r1
                && r2
                && r3
                && Vector3.Distance(point1, hitPoint1) < MAX_DIST
                && Vector3.Distance(point2, hitPoint2) < MAX_DIST
                && Vector3.Distance(point3, hitPoint3) < MAX_DIST)
            {
                return Vector3.Normalize(Vector3.Cross(hitPoint3 - hitPoint2, hitPoint3 - hitPoint1));
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
                    if (obj.Collider?.RayIntersection(obj.Position, rayPosition, rayDirection, out Vector3 hitPoint, out _) == true)
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