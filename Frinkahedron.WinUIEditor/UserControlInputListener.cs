using Frinkahedron.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace Frinkahedron.WinUIEditor
{
    internal sealed class UserControlInputListener : IDisposable
    {
        private readonly UIElement control;
        private List<MouseButton> mouseButtonsDown; 
        private List<MouseButton> mouseButtonsUp;
        private List<VirtualKey> keysDown;
        private List<VirtualKey> keysUp;
        private Vector2 currentMousePosition;
        private int currentMouseDelta;

        public UserControlInputListener(UIElement control)
        {
            mouseButtonsDown = new List<MouseButton>();
            mouseButtonsUp = new List<MouseButton>();

            keysDown = new List<VirtualKey>();
            keysUp = new List<VirtualKey>();

            control.PointerPressed += Control_MouseDown;
            control.PointerReleased += Control_MouseUp;
            control.PointerWheelChanged += Control_MouseWheel;
            control.PointerMoved += Control_MouseMove;

            control.KeyDown += Control_KeyDown;
            control.KeyUp += Control_KeyUp;

            //control.PreviewKeyDown += Control_PreviewKeyDown;

            this.control = control;
        }

        public void Dispose()
        {
            control.PointerPressed -= Control_MouseDown;
            control.PointerReleased -= Control_MouseUp;
            control.PointerWheelChanged -= Control_MouseWheel;
            control.PointerMoved -= Control_MouseMove;
            control.KeyDown -= Control_KeyDown;
            control.KeyUp -= Control_KeyUp;
            //control.PreviewKeyDown -= Control_PreviewKeyDown;
        }

        /*private void Control_PreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Left
                || e.KeyCode == Keys.Right
                || e.KeyCode == Keys.Up
                || e.KeyCode == Keys.Down)
            {
                keysDown.Add(e.KeyCode);
            }
        }*/

        private void Control_KeyUp(object? sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            keysUp.Add(e.Key);
        }

        private void Control_KeyDown(object? sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            keysDown.Add(e.Key);
        }

        public void UpdateInput(Input input)
        {
            foreach (var mouseButtonDown in mouseButtonsDown)
            {
                input.NewMouseButtonDown(mouseButtonDown);
            }
            foreach (var mouseButtonUp in mouseButtonsUp)
            {
                input.NewMouseButtonUp(mouseButtonUp);
            }
            foreach (var keyDown in keysDown)
            {
                input.NewKeyDown(GetFrinkKey(keyDown));
            }
            foreach (var keyUp in keysUp)
            {
                input.NewKeyUp(GetFrinkKey(keyUp));
            }

            //input.SetMousePosition(currentMousePosition, new Vector2(control.Width, control.Height));
            input.SetScrollDelta(currentMouseDelta);

            mouseButtonsDown.Clear();
            mouseButtonsUp.Clear();
            keysDown.Clear();
            keysUp.Clear();
            currentMouseDelta = 0;
        }

        private void Control_MouseMove(object? sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(control);
            currentMousePosition = new Vector2((float)point.Position.X, (float)point.Position.Y);
        }

        private void Control_MouseWheel(object? sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(control);
            currentMouseDelta += Math.Clamp(point.Properties.MouseWheelDelta, -12, 12);
        }

        private void Control_MouseUp(object? sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(control);
            var properties = point.Properties;
            var mouseButton = properties.PointerUpdateKind switch
            {
                Microsoft.UI.Input.PointerUpdateKind.LeftButtonReleased => MouseButton.Left,
                Microsoft.UI.Input.PointerUpdateKind.RightButtonReleased => MouseButton.Right,
                Microsoft.UI.Input.PointerUpdateKind.MiddleButtonReleased => MouseButton.Middle,
                _ => MouseButton.None,
            };

            mouseButtonsUp.Add(mouseButton);

            control.Focus(Microsoft.UI.Xaml.FocusState.Pointer);
        }

        private void Control_MouseDown(object? sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(control);
            var properties = point.Properties;
            var mouseButton = properties.PointerUpdateKind switch
            {
                Microsoft.UI.Input.PointerUpdateKind.LeftButtonPressed => MouseButton.Left,
                Microsoft.UI.Input.PointerUpdateKind.RightButtonPressed => MouseButton.Right,
                Microsoft.UI.Input.PointerUpdateKind.MiddleButtonPressed => MouseButton.Middle,
                _ => MouseButton.None,
            };

            mouseButtonsDown.Add(mouseButton);

            control.Focus(Microsoft.UI.Xaml.FocusState.Pointer);
        }

        private static Core.Key GetFrinkKey(VirtualKey k)
        {
            return k switch
            {
                VirtualKey.Up => Core.Key.Up,
                VirtualKey.Down => Core.Key.Down,
                VirtualKey.Left => Core.Key.Left,
                VirtualKey.Right => Core.Key.Right,
                VirtualKey.A => Core.Key.A,
                VirtualKey.B => Core.Key.B,
                VirtualKey.C => Core.Key.C,
                VirtualKey.D => Core.Key.D,
                VirtualKey.E => Core.Key.E,
                VirtualKey.F => Core.Key.F,
                VirtualKey.G => Core.Key.G,
                VirtualKey.H => Core.Key.H,
                VirtualKey.I => Core.Key.I,
                VirtualKey.J => Core.Key.J,
                VirtualKey.K => Core.Key.K,
                VirtualKey.L => Core.Key.L,
                VirtualKey.M => Core.Key.M,
                VirtualKey.N => Core.Key.N,
                VirtualKey.O => Core.Key.O,
                VirtualKey.P => Core.Key.P,
                VirtualKey.Q => Core.Key.Q,
                VirtualKey.R => Core.Key.R,
                VirtualKey.S => Core.Key.S,
                VirtualKey.T => Core.Key.T,
                VirtualKey.U => Core.Key.U,
                VirtualKey.V => Core.Key.V,
                VirtualKey.W => Core.Key.W,
                VirtualKey.X => Core.Key.X,
                VirtualKey.Y => Core.Key.Y,
                VirtualKey.Z => Core.Key.Z,
                VirtualKey.Space => Core.Key.Space,
                VirtualKey.Enter => Core.Key.Enter,
                VirtualKey.Shift => Core.Key.ShiftKey,
                VirtualKey.Control => Core.Key.ControlKey,
                _ => Core.Key.None
            };
        }
    }
}
