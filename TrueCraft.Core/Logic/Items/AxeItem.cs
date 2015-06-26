using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public abstract class AxeItem : ToolItem, ICraftingRecipe
    {
        public ItemStack[,] Pattern
        {
            get
            {
                short baseMaterial = 0;
                switch (Material)
                {
                    case ToolMaterial.Diamond:
                        baseMaterial = DiamondItem.ItemID;
                        break;
                    case ToolMaterial.Gold:
                        baseMaterial = GoldIngotItem.ItemID;
                        break;
                    case ToolMaterial.Iron:
                        baseMaterial = IronIngotItem.ItemID;
                        break;
                    case ToolMaterial.Stone:
                        baseMaterial = CobblestoneBlock.BlockID;
                        break;
                    case ToolMaterial.Wood:
                        baseMaterial = WoodenPlanksBlock.BlockID;
                        break;
                }

                return new[,]
                {
                    { new ItemStack(baseMaterial), new ItemStack(baseMaterial) },
                    { new ItemStack(baseMaterial), new ItemStack(StickItem.ItemID) },
                    { ItemStack.EmptyStack, new ItemStack(StickItem.ItemID) }
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

        public override ToolType ToolType
        {
            get
            {
                return ToolType.Axe;
            }
        }
    }

    public class WoodenAxeItem : AxeItem
    {
        public static readonly short ItemID = 0x10F;

        public override short ID { get { return 0x10F; } }

        public override ToolMaterial Material { get { return ToolMaterial.Wood; } }

        public override short BaseDurability { get { return 60; } }

        public override string DisplayName { get { return "Wooden Axe"; } }
    }

    public class StoneAxeItem : AxeItem
    {
        public static readonly short ItemID = 0x113;

        public override short ID { get { return 0x113; } }

        public override ToolMaterial Material { get { return ToolMaterial.Stone; } }

        public override short BaseDurability { get { return 132; } }

        public override string DisplayName { get { return "Stone Axe"; } }
    }

    public class IronAxeItem : AxeItem
    {
        public static readonly short ItemID = 0x102;

        public override short ID { get { return 0x102; } }

        public override ToolMaterial Material { get { return ToolMaterial.Iron; } }

        public override short BaseDurability { get { return 251; } }

        public override string DisplayName { get { return "Iron Axe"; } }
    }

    public class GoldenAxeItem : AxeItem
    {
        public static readonly short ItemID = 0x11E;

        public override short ID { get { return 0x11E; } }

        public override ToolMaterial Material { get { return ToolMaterial.Gold; } }

        public override short BaseDurability { get { return 33; } }

        public override string DisplayName { get { return "Golden Axe"; } }
    }

    public class DiamondAxeItem : AxeItem
    {
        public static readonly short ItemID = 0x117;

        public override short ID { get { return 0x117; } }

        public override ToolMaterial Material { get { return ToolMaterial.Diamond; } }

        public override short BaseDurability { get { return 1562; } }

        public override string DisplayName { get { return "Diamond Axe"; } }
    }
}