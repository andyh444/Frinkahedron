using Frinkahedron.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frinkahedron.WinformsEditor
{
    internal sealed class UserControlInputListener : IDisposable
    {
        private readonly UserControl control;
        private List<MouseButtons> mouseButtonsDown; 
        private List<MouseButtons> mouseButtonsUp;
        private Vector2 currentMousePosition;
        private int currentMouseDelta;

        public UserControlInputListener(UserControl control)
        {
            mouseButtonsDown = new List<MouseButtons>();
            mouseButtonsUp = new List<MouseButtons>();

            control.MouseDown += Control_MouseDown;
            control.MouseUp += Control_MouseUp;
            control.MouseWheel += Control_MouseWheel;
            control.MouseMove += Control_MouseMove;
            this.control = control;
        }

        public void UpdateInput(Input input)
        {
            foreach (var mouseButtonDown in mouseButtonsDown)
            {
                input.NewMouseButtonDown(GetFrinkMouseButton(mouseButtonDown));
            }
            foreach (var mouseButtonDown in mouseButtonsUp)
            {
                input.NewMouseButtonUp(GetFrinkMouseButton(mouseButtonDown));
            }

            input.SetMousePosition(currentMousePosition, new Vector2(control.Width, control.Height));
            input.SetScrollDelta(currentMouseDelta);

            mouseButtonsDown.Clear();
            mouseButtonsUp.Clear();
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

        public void Dispose()
        {
            control.MouseDown -= Control_MouseDown;
            control.MouseUp -= Control_MouseUp;
            control.MouseWheel -= Control_MouseWheel;
            control.MouseMove -= Control_MouseMove;
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
    }
}
