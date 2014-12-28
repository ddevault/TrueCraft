using System;

namespace TrueCraft.API.Windows
{
    public interface IWindow
    {
        event EventHandler<WindowChangeEventArgs> WindowChange;

        IWindowArea[] WindowAreas { get; }
        int Length { get; }
        ItemStack this[int index] { get; set; }
        bool Empty { get; }

        /// <summary>
        /// Call this to "shift+click" an item from one area to another.
        /// </summary>
        void MoveToAlternateArea(int index);
        /// <summary>
        /// Gets an array of all slots in this window. Suitable for sending to clients over the network.
        /// </summary>
        ItemStack[] GetSlots();
    }
}