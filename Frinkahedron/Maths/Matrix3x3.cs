using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Maths
{
    /// <summary>
    /// Row major 3x3 matrix
    /// </summary>
    public struct Matrix3x3 : IEquatable<Matrix3x3>
    {
        public Vector3 Row1;
        public Vector3 Row2;
        public Vector3 Row3;

        public Matrix3x3(Vector3 row1, Vector3 row2, Vector3 row3)
        {
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }

        public static Matrix3x3 Identity()
        {
            return new Matrix3x3(Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ);
        }

        public bool Equals(Matrix3x3 other)
        {
            return Row1.Equals(other.Row1)
                && Row2.Equals(other.Row2)
                && Row3.Equals(other.Row3);
        }

        public static Vector3 operator *(Matrix3x3 mat, Vector3 vec)
        {
            return new Vector3(
                Vector3.Dot(mat.Row1, vec),
                Vector3.Dot(mat.Row2, vec),
                Vector3.Dot(mat.Row3, vec)
            );
        }
    }
}
