using System;
using TrueCraft.API.Networking;

namespace TrueCraft.API.Windows
{
    public interface IWindow : IDisposable
    {
        event EventHandler<WindowChangeEventArgs> WindowChange;

        IRemoteClient Client { get; set; }
        IWindowArea[] WindowAreas { get; }
        sbyte ID { get; set; }
        string Name { get; }
        sbyte Type { get; }
        int Length { get; }
        int MinecraftWasWrittenByFuckingIdiotsLength { get; }
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
        void SetSlots(ItemStack[] slots);
        /// <summary>
        /// Adds the specified item stack to this window, merging with established slots as neccessary.
        /// </summary>
        bool PickUpStack(ItemStack slot);
        /// <summary>
        /// Copy the contents of this window back into an inventory window after changes have been made.
        /// </summary>
        void CopyToInventory(IWindow inventoryWindow);
    }
}
