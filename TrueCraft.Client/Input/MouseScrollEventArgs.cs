using System;

namespace TrueCraft.Client.Input
{
    /// <summary>
    /// Provides the event data for mouse scroll events.
    /// </summary>
    public class MouseScrollEventArgs : MouseEventArgs
    {
        /// <summary>
        /// Gets the scroll value for the event.
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Gets the scroll value delta for the event.
        /// </summary>
        public int DeltaValue { get; private set; }

        /// <summary>
        /// Creates new mouse scroll event data.
        /// </summary>
        /// <param name="x">The X coordinate for the event.</param>
        /// <param name="y">The Y coordinate for the event.</param>
        /// <param name="value">The scroll value for the event.</param>
        /// <param name="deltaValue">The scroll value delta for the event.</param>
        public MouseScrollEventArgs(int x, int y, int value, int deltaValue)
            : base(x, y)
        {
            Value = value;
            DeltaValue = deltaValue;
        }
    }
}
