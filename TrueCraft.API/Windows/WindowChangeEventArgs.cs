using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API.Windows
{
    public class WindowChangeEventArgs : EventArgs
    {
        public int SlotIndex { get; set; }
        public ItemStack Value { get; set; }
        public bool Handled { get; set; }

        public WindowChangeEventArgs(int slotIndex, ItemStack value)
        {
            SlotIndex = slotIndex;
            Value = value;
            Handled = false;
        }
    }
}
