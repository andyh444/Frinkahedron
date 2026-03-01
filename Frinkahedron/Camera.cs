using System.Numerics;

namespace Frinkahedron
{
    public class Camera
    {
        public Vector3 Position { get; private set; }

        public Vector3 LookDirection { get; private set; }

        public Matrix4x4 ViewMatrix { get; private set; }

        public Matrix4x4 ProjectionMatrix { get; private set; }

        public Camera(Vector3 initialPosition, Vector3 initialDirection)
        {
            Position = initialPosition;
            LookDirection = initialDirection;

            ProjectionMatrix = CreatePerspective(MathF.PI / 2, 1.777777f, 0.1f, 1000f);
            ViewMatrix = CreateViewMatrix();
        }

        public void Translate(Vector3 translation)
        {
            Position += translation;
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
    }
}
