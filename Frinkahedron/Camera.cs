using System.Numerics;

namespace Frinkahedron
{
    public class Camera
    {
        public Vector3 Position { get; private set; }

        public Vector3 LookDirection { get; private set; }

        public Matrix4x4 ViewMatrix { get; private set; }

        public Matrix4x4 ProjectionMatrix { get; private set; }

        private Camera(Vector3 initialPosition, Vector3 initialDirection, Matrix4x4 projectionMatrix)
        {
            Position = initialPosition;
            LookDirection = initialDirection;

            ProjectionMatrix = projectionMatrix;
            ViewMatrix = CreateViewMatrix();
        }

        public static Camera CreatePerspectiveCamera(Vector3 initialPosition, Vector3 initialDirection)
        {
            return new Camera(
                initialPosition,
                initialDirection,
                CreatePerspective(MathF.PI / 4, 1.777777f, 0.1f, 1000f));
        }

        public static Camera CreateOrthoCamera(Vector3 initialPosition, Vector3 initialDirection)
        {
            return new Camera(
                initialPosition,
                initialDirection,
                CreateOrtho(false, -100, 100, -100, 100, 0.1f, 2000f));
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

        private Matrix4x4 CreateViewMatrix()
        {
            return Matrix4x4.CreateLookAt(Position, Position + LookDirection, Vector3.UnitY);
        }

        //private static Matrix4x4 CreatePerspective(float fov, float aspectRatio, float near, float far)
        //{
        //    return Matrix4x4.CreatePerspectiveFieldOfView(fov, aspectRatio, near, far);
        //}

        private static Matrix4x4 CreatePerspective(float fov, float aspectRatio, float near, float far)
        {
            if (fov <= 0.0f || fov >= MathF.PI)
                throw new ArgumentOutOfRangeException(nameof(fov));

            if (near <= 0.0f)
                throw new ArgumentOutOfRangeException(nameof(near));

            if (far <= 0.0f)
                throw new ArgumentOutOfRangeException(nameof(far));

            float yScale = 1.0f / MathF.Tan(fov * 0.5f);
            float xScale = yScale / aspectRatio;

            Matrix4x4 result;

            result.M11 = xScale;
            result.M12 = result.M13 = result.M14 = 0.0f;

            result.M22 = yScale;
            result.M21 = result.M23 = result.M24 = 0.0f;

            result.M31 = result.M32 = 0.0f;
            var negFarRange = float.IsPositiveInfinity(far) ? -1.0f : far / (near - far);
            result.M33 = negFarRange;
            result.M34 = -1.0f;

            result.M41 = result.M42 = result.M44 = 0.0f;
            result.M43 = near * negFarRange;

            return result;
        }

        private static Matrix4x4 CreateOrtho(
            bool useReverseDepth,
            float left, float right,
            float bottom, float top,
            float near, float far)
        {
            Matrix4x4 ortho;
            if (useReverseDepth)
            {
                ortho = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, far, near);
            }
            else
            {
                ortho = Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, near, far);
            }
            bool isClipSpaceYInverted = false;
            if (isClipSpaceYInverted)
            {
                ortho *= new Matrix4x4(
                    1, 0, 0, 0,
                    0, -1, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1);
            }

            return ortho;
        }
    }
}
