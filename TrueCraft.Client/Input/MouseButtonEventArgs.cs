using System;

namespace TrueCraft.Client.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class MouseButtonEventArgs : MouseEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public MouseButton Button { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPressed { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="button"></param>
        /// <param name="isPressed"></param>
        public MouseButtonEventArgs(int x, int y, MouseButton button, bool isPressed)
            : base(x, y)
        {
            Button = button;
            IsPressed = isPressed;
        }
    }
}
