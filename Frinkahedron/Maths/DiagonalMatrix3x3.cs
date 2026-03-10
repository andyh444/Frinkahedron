using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Frinkahedron.Core.Maths
{
    public struct DiagonalMatrix3x3
    {
        public Vector3 Diagonal;

        public DiagonalMatrix3x3(Vector3 diagonal)
        {
            Diagonal = diagonal;
        }

        public static DiagonalMatrix3x3 Identity() => new DiagonalMatrix3x3(new Vector3(1, 1, 1));

        public DiagonalMatrix3x3 GetInverse()
        {
            return new DiagonalMatrix3x3(
                new Vector3(
                    1 / Diagonal.X,
                    1 / Diagonal.Y,
                    1 / Diagonal.Z));
        }
    }
}
