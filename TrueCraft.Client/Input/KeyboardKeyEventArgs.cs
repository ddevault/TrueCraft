using System;
using Microsoft.Xna.Framework.Input;

namespace TrueCraft.Client.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class KeyboardKeyEventArgs : KeyboardEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public Keys Key { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPressed { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isPressed"></param>
        public KeyboardKeyEventArgs(Keys key, bool isPressed)
        {
            Key = key;
            IsPressed = isPressed;
        }
    }
}
