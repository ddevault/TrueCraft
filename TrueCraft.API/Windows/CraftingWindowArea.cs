using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API.Windows
{
    public class CraftingWindowArea : WindowArea
    {
        public const int CraftingOutput = 0;

        public CraftingWindowArea(int startIndex) : base(startIndex, 5)
        {
        }

        protected override bool IsValid(ItemStack slot, int index)
        {
            if (index == CraftingOutput && !slot.Empty)
                return false;
            return base.IsValid(slot, index);
        }
    }
}
