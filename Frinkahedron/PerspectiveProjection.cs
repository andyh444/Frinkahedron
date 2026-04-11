using System.Numerics;

namespace Frinkahedron
{
    public class PerspectiveProjection : IProjection
    {
        private float foV;
        private float aspectRatio;
        private float near;
        private float far;

        public Matrix4x4 Matrix { get; private set; }

        public ProjectionType ProjectionType => ProjectionType.Perspective;

        public float FoV
        {
            get => foV;
            set
            {
                foV = value;
                UpdateMatrix();
            }
        }

        public float AspectRatio
        {
            get => aspectRatio;
            set
            {
                aspectRatio = value;
                UpdateMatrix();
            }
        }

        public float Near
        {
            get => near;
            set
            {
                near = value;
                UpdateMatrix();
            }
        }

        public float Far
        {
            get => far;
            set
            {
                far = value;
                UpdateMatrix();
            }
        }

        public PerspectiveProjection(float fov, float aspectRatio, float near, float far)
        {
            this.foV = fov;
            this.aspectRatio = aspectRatio;
            this.near = near;
            this.far = far;
            UpdateMatrix();
        }

        private void UpdateMatrix()
        {
            Matrix = CreatePerspective(FoV, AspectRatio, Near, Far);
        }

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
