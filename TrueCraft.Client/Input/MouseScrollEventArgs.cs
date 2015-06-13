using System;

namespace TrueCraft.Client.Input
{
    /// <summary>
    /// 
    /// </summary>
    public class MouseScrollEventArgs : MouseEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int DeltaValue { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        /// <param name="deltaValue"></param>
        public MouseScrollEventArgs(int x, int y, int value, int deltaValue)
            : base(x, y)
        {
            Value = value;
            DeltaValue = deltaValue;
        }
    }
}
