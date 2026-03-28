using System.Numerics;

namespace Frinkahedron.VeldridImplementation
{
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
}