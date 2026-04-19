using System.Numerics;

namespace Frinkahedron
{
    public enum ProjectionType
    {
        Perspective,
        Orthographic
    }

    public interface IProjection
    {
        Matrix4x4 Matrix { get; }

        ProjectionType ProjectionType { get; }

        float AspectRatio { get; set; }

        float Near { get; }

        float Far { get; }
    }

    public class Camera
    {
        public Vector3 Position { get; private set; }

        public Vector3 LookDirection { get; private set; }

        public Matrix4x4 ViewMatrix { get; private set; }

        public IProjection Projection { get; private set; }

        public ProjectionType ProjectionType => Projection.ProjectionType;

        private Camera(Vector3 initialPosition, Vector3 initialDirection, IProjection projection)
        {
            Position = initialPosition;
            LookDirection = initialDirection;
            Projection = projection;
            ViewMatrix = CreateViewMatrix();
        }

        public static Camera CreatePerspectiveCamera(Vector3 initialPosition, Vector3 initialDirection, float screenAspectRatio)
        {
            return new Camera(
                initialPosition,
                initialDirection,
                new PerspectiveProjection(MathF.PI / 4, screenAspectRatio, 1.0f, 1000f));
        }

        public static Camera CreateOrthoCamera(Vector3 initialPosition, Vector3 initialDirection, float width, float screenAspectRatio)
        {
            return new Camera(
                initialPosition,
                initialDirection,
                new OrthographicProjection(width, screenAspectRatio, 1.0f, 1000f));
        }

        public void MakePerspective(float fov, float aspectRatio, float near, float far)
        {
            Projection = new PerspectiveProjection(fov, aspectRatio, near, far);
        }

        public void MakeOrtho(float width, float aspectRatio, float near, float far)
        {
            Projection = new OrthographicProjection(width, aspectRatio, near, far);
        }

        public void SetValues(Vector3 position, Vector3 direction)
        {
            Position = position;
            LookDirection = Vector3.Normalize(direction);
            ViewMatrix = CreateViewMatrix();
        }

        public void Translate(Vector3 translation)
        {
            Position += translation;
            ViewMatrix = CreateViewMatrix();
        }

        public Vector3 GetRight()
        {
            return Vector3.Cross(LookDirection, Vector3.UnitY);
        }

        public Vector3 GetUp() => Vector3.Cross(GetRight(), LookDirection);

        public void RotateYaw(float angle)
        {
            var rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, angle);
            LookDirection = Vector3.Transform(LookDirection, rotation);
            LookDirection = Vector3.Normalize(LookDirection);
            ViewMatrix = CreateViewMatrix();
        }

        public void RotatePitch(float angle)
        {
            var rotation = Quaternion.CreateFromAxisAngle(GetRight(), angle);
            LookDirection = Vector3.Transform(LookDirection, rotation);
            LookDirection = Vector3.Normalize(LookDirection);
            ViewMatrix = CreateViewMatrix();
        }

        public (Vector3 position, Vector3 direction) GetRay(Vector2 screenPosition)
        {
            // screen position should be in normalised device coordinates (ndc)
            Vector4 nearClip = new Vector4(screenPosition.X, screenPosition.Y, 0.0f, 1.0f);
            Vector4 farClip = new Vector4(screenPosition.X, screenPosition.Y, 1.0f, 1.0f);

            _ = Matrix4x4.Invert(Projection.Matrix, out var inverseProjection);
            _ = Matrix4x4.Invert(ViewMatrix, out var inverseView);

            Vector4 nearView = Vector4.Transform(nearClip, inverseProjection);
            Vector4 farView = Vector4.Transform(farClip, inverseProjection);

            // Perspective divide
            nearView /= nearView.W;
            farView /= farView.W;

            Vector4 nearWorld = Vector4.Transform(nearView, inverseView);
            Vector4 farWorld = Vector4.Transform(farView, inverseView);

            Vector3 rayPosition = nearWorld.AsVector3();
            Vector3 direction = Vector3.Normalize(farWorld.AsVector3() - rayPosition);
            return (rayPosition, direction);
        }

        public Vector3 Unproject(Vector2 screenPosition)
        {
            // screen position should be in normalised device coordinates (ndc)
            Vector4 nearClip = new Vector4(screenPosition.X, screenPosition.Y, 0.0f, 1.0f);

            _ = Matrix4x4.Invert(Projection.Matrix, out var inverseProjection);
            _ = Matrix4x4.Invert(ViewMatrix, out var inverseView);

            Vector4 nearView = Vector4.Transform(nearClip, inverseProjection);

            // Perspective divide
            nearView /= nearView.W;

            Vector4 nearWorld = Vector4.Transform(nearView, inverseView);

            Vector3 rayPosition = nearWorld.AsVector3();
            return rayPosition;
        }

        private Matrix4x4 CreateViewMatrix()
        {
            return Matrix4x4.CreateLookAt(Position, Position + LookDirection, Vector3.UnitY);
        }
    }
}
