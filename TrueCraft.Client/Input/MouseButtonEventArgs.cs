using System;

namespace TrueCraft.Client.Input
{
    /// <summary>
    /// Provides the event data for mouse button events.
    /// </summary>
    public class MouseButtonEventArgs : MouseEventArgs
    {
        /// <summary>
        /// Gets the mouse button for the event.
        /// </summary>
        public MouseButton Button { get; private set; }

        /// <summary>
        /// Gets whether the button was pressed or released.
        /// </summary>
        public bool IsPressed { get; private set; }

        /// <summary>
        /// Creates new mouse button event data.
        /// </summary>
        /// <param name="x">The X coordinate for the event.</param>
        /// <param name="y">The Y coordinate for the event.</param>
        /// <param name="button">The mouse button for the event.</param>
        /// <param name="isPressed">Whether the button was pressed or released.</param>
        public MouseButtonEventArgs(int x, int y, MouseButton button, bool isPressed)
            : base(x, y)
        {
            Button = button;
            IsPressed = isPressed;
        }
    }
}
