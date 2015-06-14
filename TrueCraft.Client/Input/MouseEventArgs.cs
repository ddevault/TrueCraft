using System;

namespace TrueCraft.Client.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class MouseEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public MouseEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
