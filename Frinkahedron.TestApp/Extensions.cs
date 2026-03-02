using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Frinkahedron.TestApp
{
    internal static class Extensions
    {
        public static Vector4 ToVector4(this RgbaFloat colour)
        {
            return new Vector4(
                colour.R,
                colour.G,
                colour.B,
                colour.A);
        }
    }
}
