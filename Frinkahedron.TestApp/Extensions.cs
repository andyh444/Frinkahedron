using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;
using Vulkan.Xlib;

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

        public static Core.Key ToCoreKey(this Key key)
        {
            return key switch
            {
                Key.Up => Core.Key.Up,
                Key.Down => Core.Key.Down,
                Key.Left => Core.Key.Left,
                Key.Right => Core.Key.Right,
                Key.A => Core.Key.A,
                Key.B => Core.Key.B,
                Key.C => Core.Key.C,
                Key.D => Core.Key.D,
                Key.E => Core.Key.E,
                Key.F => Core.Key.F,
                Key.G => Core.Key.G,
                Key.H => Core.Key.H,
                Key.I => Core.Key.I,
                Key.J => Core.Key.J,
                Key.K => Core.Key.K,
                Key.L => Core.Key.L,
                Key.M => Core.Key.M,
                Key.N => Core.Key.N,
                Key.O => Core.Key.O,
                Key.P => Core.Key.P,
                Key.Q => Core.Key.Q,
                Key.R => Core.Key.R,
                Key.S => Core.Key.S,
                Key.T => Core.Key.T,
                Key.U => Core.Key.U,
                Key.V => Core.Key.V,
                Key.W => Core.Key.W,
                Key.X => Core.Key.X,
                Key.Y => Core.Key.Y,
                Key.Z => Core.Key.Z,
                Key.Space => Core.Key.Space,
                Key.Enter => Core.Key.Enter,
                Key.ShiftLeft or Key.ShiftRight => Core.Key.ShiftKey,
                Key.ControlLeft or Key.ControlRight => Core.Key.ControlKey,
                _ => Core.Key.None
            };
        }
    }
}
