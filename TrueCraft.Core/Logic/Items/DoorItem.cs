using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public abstract class DoorItem : ItemProvider, ICraftingRecipe
    {
        public override sbyte MaximumStack { get { return 1; } }
    
        public ItemStack[,] Pattern
        {
            get
            {
                var baseMaterial = (this is IronDoorItem) ? IronIngotItem.ItemID : WoodenPlanksBlock.BlockID;
                return new[,]
                {
                    { new ItemStack(baseMaterial), new ItemStack(baseMaterial) },
                    { new ItemStack(baseMaterial), new ItemStack(baseMaterial) },
                    { new ItemStack(baseMaterial), new ItemStack(baseMaterial) }
                };
            }
        }

        public ItemStack Output
        {
            get
            {
                return new ItemStack(ID);
            }
        }

        public bool SignificantMetadata
        {
            get
            {
                return false;
            }
        }
    }

    public class IronDoorItem : DoorItem
    {
        public static readonly short ItemID = 0x14A;

        public override short ID { get { return 0x14A; } }

        public override string DisplayName { get { return "Iron Door"; } }
    }

    public class WoodenDoorItem : DoorItem
    {
        public static readonly short ItemID = 0x144;

        public override short ID { get { return 0x144; } }

        public override string DisplayName { get { return "Wooden Door"; } }
    }
}