using System.Numerics;
using System.Runtime.InteropServices;

namespace Frinkahedron.VeldridImplementation
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Model2MatrixInfo
    {
        public Vector2 XAxis;
        public Vector2 YAxis;
        public Vector2 Translation;
        public float Pad0;
        public float Pad1;

        public static Model2MatrixInfo FromMatrix3x2(Matrix3x2 matrix)
        {
            return new Model2MatrixInfo
            {
                XAxis = new Vector2(matrix.M11, matrix.M12),
                YAxis = new Vector2(matrix.M21, matrix.M22),
                Translation = new Vector2(matrix.M31, matrix.M32)
            };
        }
    }

    public struct ModelMatrixInfo
    {
        public Matrix4x4 Model;
    }

    public struct CameraMatrixInfo
    {
        public Matrix4x4 View;
        public Matrix4x4 Projection;
    }

    public struct PointLightInfo
    {
        public Vector3 Position;
        public float _pad0;
        public Vector3 Colour;
        public float _pad1;
        public float Range;
        public float _pad2;
        public float _pad3;
        public float _pad4;
    }

    public struct DirectionalLightInfo
    {
        public int Enabled;
        public float _pad0;
        public float _pad1;
        public float _pad2;
        public Vector3 Direction;
        public float _pad3;
        public Vector3 Colour;
        public float _pad4;
    }

    public struct PointLightsInfo
    {
        public PointLightInfo PointLights0;
        public PointLightInfo PointLights1;
        public PointLightInfo PointLights2;
        public PointLightInfo PointLights3;
        public int NumActiveLights;
        public float _padding0;
        public float _padding1;
        public float _padding2;
    }

    public struct CameraInfo
    {
        public Vector3 WorldPosition;
        public float _padding1;
        public Vector3 LookDirection;
        public float _padding2;
    }

    public struct HighlightParams
    {
        // Colour stored as RGBA
        public Vector4 Color;
        // Params.X = OutlineWidth, other components unused
        public Vector4 Params;
    }
}