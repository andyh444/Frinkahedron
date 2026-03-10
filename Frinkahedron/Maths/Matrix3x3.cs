using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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

        public static Matrix3x3 CreateFromQuaternion(Quaternion q)
        {
            float x = q.X;
            float y = q.Y;
            float z = q.Z;
            float w = q.W;

            float x2 = x + x;
            float y2 = y + y;
            float z2 = z + z;

            float xx = x * x2;
            float yy = y * y2;
            float zz = z * z2;
            float xy = x * y2;
            float xz = x * z2;
            float yz = y * z2;
            float wx = w * x2;
            float wy = w * y2;
            float wz = w * z2;

            return new Matrix3x3(
                new Vector3(1f - (yy + zz), xy - wz, xz + wy),
                new Vector3(xy + wz, 1f - (xx + zz), yz - wx),
                new Vector3(xz - wy, yz + wx, 1f - (xx + yy))
            );
        }

        private Vector3 Column1() => new Vector3(Row1.X, Row2.X, Row3.X);
        private Vector3 Column2() => new Vector3(Row1.Y, Row2.Y, Row3.Y);
        private Vector3 Column3() => new Vector3(Row1.Z, Row2.Z, Row3.Z);

        public Matrix3x3 Transpose()
        {
            return new Matrix3x3(
                Column1(),
                Column2(),
                Column3());
        }

        public bool Equals(Matrix3x3 other)
        {
            return Row1.Equals(other.Row1)
                && Row2.Equals(other.Row2)
                && Row3.Equals(other.Row3);
        }

        public Matrix3x3 GetInverse()
        {
            // Calculate the determinant of the matrix
            float determinant = Row1.X * (Row2.Y * Row3.Z - Row2.Z * Row3.Y)
                              - Row1.Y * (Row2.X * Row3.Z - Row2.Z * Row3.X)
                              + Row1.Z * (Row2.X * Row3.Y - Row2.Y * Row3.X);

            if (Math.Abs(determinant) < float.Epsilon)
            {
                throw new InvalidOperationException("Matrix is not invertible.");
            }

            // Calculate the inverse determinant
            float invDet = 1.0f / determinant;

            // Calculate the adjugate matrix
            Vector3 col1 = new Vector3(
                (Row2.Y * Row3.Z - Row2.Z * Row3.Y) * invDet,
                (Row1.Z * Row3.Y - Row1.Y * Row3.Z) * invDet,
                (Row1.Y * Row2.Z - Row1.Z * Row2.Y) * invDet
            );

            Vector3 col2 = new Vector3(
                (Row2.Z * Row3.X - Row2.X * Row3.Z) * invDet,
                (Row1.X * Row3.Z - Row1.Z * Row3.X) * invDet,
                (Row1.Z * Row2.X - Row1.X * Row2.Z) * invDet
            );

            Vector3 col3 = new Vector3(
                (Row2.X * Row3.Y - Row2.Y * Row3.X) * invDet,
                (Row1.Y * Row3.X - Row1.X * Row3.Y) * invDet,
                (Row1.X * Row2.Y - Row1.Y * Row2.X) * invDet
            );

            // Return the transposed adjugate matrix as the inverse
            return new Matrix3x3(col1, col2, col3);
        }

        public static Matrix3x3 operator *(Matrix3x3 mat1, Matrix3x3 mat2)
        {
            Vector3 r1 = mat1.Row1;
            Vector3 r2 = mat1.Row2;
            Vector3 r3 = mat1.Row3;
            Vector3 c1 = mat2.Column1();
            Vector3 c2 = mat2.Column2();
            Vector3 c3 = mat2.Column3();
            return new Matrix3x3(
                new Vector3(Vector3.Dot(r1, c1), Vector3.Dot(r1, c2), Vector3.Dot(r1, c3)),
                new Vector3(Vector3.Dot(r2, c1), Vector3.Dot(r2, c2), Vector3.Dot(r2, c3)),
                new Vector3(Vector3.Dot(r3, c1), Vector3.Dot(r3, c2), Vector3.Dot(r3, c3)));
        }

        public static Matrix3x3 operator *(Matrix3x3 mat1, DiagonalMatrix3x3 mat2)
        {
            Vector3 r1 = mat1.Row1;
            Vector3 r2 = mat1.Row2;
            Vector3 r3 = mat1.Row3;
            Vector3 d1 = mat2.Diagonal;

            return new Matrix3x3(
                ElementwiseMultiply(r1, d1),
                ElementwiseMultiply(r2, d1),
                ElementwiseMultiply(r3, d1));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3 ElementwiseMultiply(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
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
