using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;
using TrueCraft.API.Windows;

namespace TrueCraft.Core.Windows
{
    public class ArmorWindowArea : WindowArea
    {
        public const int Footwear = 3;
        public const int Pants = 2;
        public const int Chestplate = 1;
        public const int Headgear = 0;

        public ArmorWindowArea(int startIndex) : base(startIndex, 4, 1, 4)
        {
        }

        protected override bool IsValid(ItemStack slot, int index)
        {
            if (slot.Empty)
                return true;
            // TODO: Armor
            //var item = (IArmorItem)(Item)slot.Id;
            //if (item == null)
            //    return false;
            //if (index == Footwear && item.ArmorSlot != ArmorSlot.Footwear)
            //    return false;
            //if (index == Pants && item.ArmorSlot != ArmorSlot.Pants)
            //    return false;
            //if (index == Chestplate && item.ArmorSlot != ArmorSlot.Chestplate)
            //    return false;
            //if (index == Headgear && item.ArmorSlot != ArmorSlot.Headgear)
            //    return false;
            return base.IsValid(slot, index);
        }

        public override int MoveOrMergeItem(int index, ItemStack slot, IWindowArea from)
        {
            for (int i = 0; i < Length; i++)
            {
                if (IsValid(slot, i))
                {
                    if (this[i].Empty)
                    {
                        this[i] = slot;
                        from[index] = ItemStack.EmptyStack;
                        return i;
                    }
                }
            }
            return -1;
        }
    }
}
