using Frinkahedron.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frinkahedron.WinformsEditor
{
    internal sealed class UserControlInputListener : IDisposable
    {
        private readonly Control control;
        private List<MouseButtons> mouseButtonsDown; 
        private List<MouseButtons> mouseButtonsUp;
        private List<Keys> keysDown;
        private List<Keys> keysUp;
        private Vector2 currentMousePosition;
        private int currentMouseDelta;

        public UserControlInputListener(Control control)
        {
            mouseButtonsDown = new List<MouseButtons>();
            mouseButtonsUp = new List<MouseButtons>();

            keysDown = new List<Keys>();
            keysUp = new List<Keys>();

            control.MouseDown += Control_MouseDown;
            control.MouseUp += Control_MouseUp;
            control.MouseWheel += Control_MouseWheel;
            control.MouseMove += Control_MouseMove;

            control.KeyDown += Control_KeyDown;
            control.KeyUp += Control_KeyUp;

            this.control = control;
        }

        public void Dispose()
        {
            control.MouseDown -= Control_MouseDown;
            control.MouseUp -= Control_MouseUp;
            control.MouseWheel -= Control_MouseWheel;
            control.MouseMove -= Control_MouseMove;
            control.KeyDown -= Control_KeyDown;
            control.KeyUp -= Control_KeyUp;
        }

        private void Control_KeyUp(object? sender, KeyEventArgs e)
        {
            keysUp.Add(e.KeyCode);
        }

        private void Control_KeyDown(object? sender, KeyEventArgs e)
        {
            keysDown.Add(e.KeyCode);
        }

        public void UpdateInput(Input input)
        {
            foreach (var mouseButtonDown in mouseButtonsDown)
            {
                input.NewMouseButtonDown(GetFrinkMouseButton(mouseButtonDown));
            }
            foreach (var mouseButtonUp in mouseButtonsUp)
            {
                input.NewMouseButtonUp(GetFrinkMouseButton(mouseButtonUp));
            }
            foreach (var keyDown in keysDown)
            {
                input.NewKeyDown(GetFrinkKey(keyDown));
            }
            foreach (var keyUp in keysUp)
            {
                input.NewKeyUp(GetFrinkKey(keyUp));
            }

            input.SetMousePosition(currentMousePosition, new Vector2(control.Width, control.Height));
            input.SetScrollDelta(currentMouseDelta);

            mouseButtonsDown.Clear();
            mouseButtonsUp.Clear();
            keysDown.Clear();
            keysUp.Clear();
            currentMouseDelta = 0;
        }

        private void Control_MouseMove(object? sender, MouseEventArgs e)
        {
            currentMousePosition = new Vector2(e.X, e.Y);
        }

        private void Control_MouseWheel(object? sender, MouseEventArgs e)
        {
            currentMouseDelta += Math.Clamp(e.Delta, -12, 12);
        }

        private void Control_MouseUp(object? sender, MouseEventArgs e)
        {
            mouseButtonsUp.Add(e.Button);
        }

        private void Control_MouseDown(object? sender, MouseEventArgs e)
        {
            mouseButtonsDown.Add(e.Button);
        }

        private static MouseButton GetFrinkMouseButton(MouseButtons e)
        {
            return e switch
            {
                MouseButtons.Left => MouseButton.Left,
                MouseButtons.Right => MouseButton.Right,
                MouseButtons.Middle => MouseButton.Middle,
                _ => MouseButton.None
            };
        }

        private static Core.Key GetFrinkKey(Keys k)
        {
            return k switch
            {
                Keys.Up => Core.Key.Up,
                Keys.Down => Core.Key.Down,
                Keys.Left => Core.Key.Left,
                Keys.Right => Core.Key.Right,
                Keys.A => Core.Key.A,
                Keys.B => Core.Key.B,
                Keys.C => Core.Key.C,
                Keys.D => Core.Key.D,
                Keys.E => Core.Key.E,
                Keys.F => Core.Key.F,
                Keys.G => Core.Key.G,
                Keys.H => Core.Key.H,
                Keys.I => Core.Key.I,
                Keys.J => Core.Key.J,
                Keys.K => Core.Key.K,
                Keys.L => Core.Key.L,
                Keys.M => Core.Key.M,
                Keys.N => Core.Key.N,
                Keys.O => Core.Key.O,
                Keys.P => Core.Key.P,
                Keys.Q => Core.Key.Q,
                Keys.R => Core.Key.R,
                Keys.S => Core.Key.S,
                Keys.T => Core.Key.T,
                Keys.U => Core.Key.U,
                Keys.V => Core.Key.V,
                Keys.W => Core.Key.W,
                Keys.X => Core.Key.X,
                Keys.Y => Core.Key.Y,
                Keys.Z => Core.Key.Z,
                Keys.Space => Core.Key.Space,
                Keys.Enter => Core.Key.Enter,
                Keys.ShiftKey => Core.Key.ShiftKey,
                Keys.ControlKey => Core.Key.ControlKey,
                _ => Core.Key.None
            };
        }
    }
}
