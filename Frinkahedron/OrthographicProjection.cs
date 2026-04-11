using System.Numerics;

namespace Frinkahedron
{
    public class OrthographicProjection : IProjection
    {
        private float width;
        private float aspectRatio;
        private float near;
        private float far;

        public Matrix4x4 Matrix { get; private set; }

        public ProjectionType ProjectionType => ProjectionType.Orthographic;

        public float Width
        {
            get => width;
            set
            {
                width = value;
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

        public OrthographicProjection(float width, float aspectRatio, float near, float far)
        {
            this.width = width;
            this.aspectRatio = aspectRatio;
            this.near = near;
            this.far = far;
            UpdateMatrix();
        }

        private void UpdateMatrix()
        {
            float halfWidth = 0.5f * Width;
            float halfHeight = 0.5f * (Width / AspectRatio);
            Matrix = CreateOrtho(false, -halfWidth, halfWidth, -halfHeight, halfHeight, Near, Far);
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
