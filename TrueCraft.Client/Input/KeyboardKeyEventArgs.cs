using System;
using Microsoft.Xna.Framework.Input;

namespace TrueCraft.Client.Input
{
    /// <summary>
    /// Provides the event data for keyboard key events.
    /// </summary>
    public class KeyboardKeyEventArgs : KeyboardEventArgs
    {
        /// <summary>
        /// Gets the key for the event.
        /// </summary>
        public Keys Key { get; private set; }

        /// <summary>
        /// Gets whether the key was pressed or released.
        /// </summary>
        public bool IsPressed { get; private set; }

        /// <summary>
        /// Creates new keyboard key event data.
        /// </summary>
        /// <param name="key">The key for the event.</param>
        /// <param name="isPressed">Whether the key was pressed or released.</param>
        public KeyboardKeyEventArgs(Keys key, bool isPressed)
        {
            Key = key;
            IsPressed = isPressed;
        }
    }
}
