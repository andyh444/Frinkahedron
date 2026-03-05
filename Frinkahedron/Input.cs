using Frinkahedron.Core;
using System.Numerics;

namespace Frinkahedron.Core
{
    public interface IInput
    {
        /// <summary>
        /// returns true if the specified key is currently held down
        /// </summary>
        bool IsKeyDown(Key key);

        /// <summary>
        /// return true if the specified key was pressed between the previous frame and this one
        /// </summary>
        bool IsKeyPressed(Key key);

        /// <summary>
        /// returns true if the specified key was released between the previous frame and this one
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsKeyReleased(Key key);

        /// <summary>
        /// returns true if the specified mouse button is currently held down
        /// </summary>
        bool IsMouseButtonDown(MouseButton button);

        /// <summary>
        /// return true if the specified mouse button was pressed between the previous frame and this one
        /// </summary>
        bool IsMouseButtonPressed(MouseButton button);

        /// <summary>
        /// return true if the specified mouse button was pressed between the previous frame and this one
        /// </summary>
        bool IsMouseButtonReleased(MouseButton button);

        /// <summary>
        /// Gets the position of the mouse in normalised screen coordinates (between 0 and 1)
        /// </summary>
        /// <returns></returns>
        Vector2 GetMouseScreenPosition();

        Vector2 GetMouseDelta();

        int GetMouseScrollDelta();
    }

    public class Input : IInput
    {
        private HashSet<Key> keysDown = new HashSet<Key>();
        private HashSet<Key> keysPressed = new HashSet<Key>();
        private HashSet<Key> keysReleased = new HashSet<Key>();
        private HashSet<MouseButton> mouseButtonsDown = new HashSet<MouseButton>();
        private HashSet<MouseButton> mouseButtonsPressed = new HashSet<MouseButton>();
        private HashSet<MouseButton> mouseButtonsReleased = new HashSet<MouseButton>();
        private Vector2 mouseDelta;
        private Vector2 mouseScreenPosition;
        private int mouseScrollDelta;

        public bool IsKeyDown(Key key) => keysDown.Contains(key);

        public bool IsKeyPressed(Key key) => keysPressed.Contains(key);

        public bool IsKeyReleased(Key key) => keysReleased.Contains(key);

        public bool IsMouseButtonDown(MouseButton button) => mouseButtonsDown.Contains(button);

        public bool IsMouseButtonPressed(MouseButton button) => mouseButtonsPressed.Contains(button);

        public bool IsMouseButtonReleased(MouseButton button) => mouseButtonsReleased.Contains(button);

        public void NewKeyDown(Key key)
        {
            keysDown.Add(key);
            keysPressed.Add(key);
        }

        public void NewKeyUp(Key key)
        {
            keysDown.Remove(key);
            keysReleased.Add(key);
        }

        public void NewMouseButtonDown(MouseButton mouseButton)
        {
            mouseButtonsDown.Add(mouseButton);
            mouseButtonsPressed.Add(mouseButton);
        }

        public void NewMouseButtonUp(MouseButton mouseButton)
        {
            mouseButtonsDown.Remove(mouseButton);
            mouseButtonsReleased.Add(mouseButton);
        }

        public void Clear()
        {
            keysPressed.Clear();
            keysReleased.Clear();
            mouseButtonsPressed.Clear();
            mouseButtonsReleased.Clear();
            //mouseScrollDelta = 0;
        }

        public Vector2 GetMouseScreenPosition() => mouseScreenPosition;

        public void SetMousePosition(Vector2 screenPosition)
        {
            var old = mouseScreenPosition;
            mouseScreenPosition = screenPosition;
            mouseDelta = mouseScreenPosition - old;
        }

        public void SetScrollDelta(int delta) => mouseScrollDelta = delta;

        public int GetMouseScrollDelta() => mouseScrollDelta;

        public Vector2 GetMouseDelta() => mouseDelta;
    }
}
