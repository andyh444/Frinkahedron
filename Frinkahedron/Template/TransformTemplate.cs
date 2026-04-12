using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Template
{
    public sealed class TransformTemplate
    {
        public Vector3 Translation { get; set; }

        public Vector3 RotationEulerAngles { get; set; }

        public float Scale { get; set; } = 1;

        public Matrix4x4 ToMatrix()
        {
            Quaternion rotation = Quaternion.CreateFromYawPitchRoll(
                RotationEulerAngles.Y,
                RotationEulerAngles.X,
                RotationEulerAngles.Z);
            return Matrix4x4.CreateScale(Scale)
                * Matrix4x4.CreateFromQuaternion(rotation)
                * Matrix4x4.CreateTranslation(Translation);
        }
    }
}
