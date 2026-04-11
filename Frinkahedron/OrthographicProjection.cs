using System.Numerics;

namespace Frinkahedron
{
    public class OrthographicProjection(float width, float aspectRatio, float near, float far) : IProjection
    {
        public Matrix4x4 Matrix { get; private set; }

        public ProjectionType ProjectionType => ProjectionType.Orthographic;

        public float Width
        {
            get;
            set
            {
                field = value;
                UpdateMatrix();
            }
        } = width;

        public float AspectRatio
        {
            get;
            set
            {
                field = value;
                UpdateMatrix();
            }
        } = aspectRatio;

        public float Near
        {
            get;
            set
            {
                field = value;
                UpdateMatrix();
            }
        } = near;

        public float Far
        {
            get;
            set
            {
                field = value;
                UpdateMatrix();
            }
        } = far;

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
