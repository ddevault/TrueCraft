using System;

namespace TrueCraft.Client.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class MouseMoveEventArgs : MouseEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public int DeltaX { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int DeltaY { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        public MouseMoveEventArgs(int x, int y, int deltaX, int deltaY)
            : base(x, y)
        {
            DeltaX = deltaX;
            DeltaY = DeltaY;
        }
    }
}
