using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Physics
{
    public sealed class Position
    {
        public Vector3 Centre { get; set; }

        public Quaternion Orientation { get; set; }

        public Position(Vector3 initialCentre, Quaternion initialOrientation)
        {
            Centre = initialCentre;
            Orientation = initialOrientation;
        }

        public Matrix4x4 ToMatrix()
        {
            return Matrix4x4.CreateFromQuaternion(Orientation) * Matrix4x4.CreateTranslation(Centre);
        }

        public Vector3 ToWorld(Vector3 local)
        {
            return Vector3.Transform(local, Orientation) + Centre;
        }

        public Vector3 ToLocal(Vector3 world)
        {
            return Vector3.Transform(world - Centre, Quaternion.Conjugate(Orientation));
        }
    }
}
