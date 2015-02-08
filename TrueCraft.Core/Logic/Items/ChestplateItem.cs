using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public abstract class ChestplateItem : ArmorItem, ICraftingRecipe
    {
        public override sbyte MaximumStack { get { return 1; } }
        
        public ItemStack[,] Pattern
        {
            get
            {
                short baseMaterial = 0;
                switch (Material)
                {
                    case ArmorMaterial.Diamond:
                        baseMaterial = DiamondItem.ItemID;
                        break;
                    case ArmorMaterial.Gold:
                        baseMaterial = GoldIngotItem.ItemID;
                        break;
                    case ArmorMaterial.Iron:
                        baseMaterial = IronIngotItem.ItemID;
                        break;
                    case ArmorMaterial.Leather:
                        baseMaterial = LeatherItem.ItemID;
                        break;
                }

                return new[,]
                {
                    { new ItemStack(baseMaterial), ItemStack.EmptyStack, new ItemStack(baseMaterial) },
                    { new ItemStack(baseMaterial), new ItemStack(baseMaterial), new ItemStack(baseMaterial) },
                    { new ItemStack(baseMaterial), new ItemStack(baseMaterial), new ItemStack(baseMaterial) }
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

    public class LeatherTunicItem : ChestplateItem
    {
        public static readonly short ItemID = 0x12B;

        public override short ID { get { return 0x12B; } }

        public override ArmorMaterial Material { get { return ArmorMaterial.Leather; } }

        public override short BaseDurability { get { return 49; } }

        public override float BaseArmor { get { return 4; } }

        public override string DisplayName { get { return "Leather Tunic"; } }
    }

    public class IronChestplateItem : ChestplateItem
    {
        public static readonly short ItemID = 0x133;

        public override short ID { get { return 0x133; } }

        public override ArmorMaterial Material { get { return ArmorMaterial.Iron; } }

        public override short BaseDurability { get { return 192; } }

        public override float BaseArmor { get { return 4; } }

        public override string DisplayName { get { return "Iron Chestplate"; } }
    }

    public class GoldenChestplateItem : ChestplateItem
    {
        public static readonly short ItemID = 0x13B;

        public override short ID { get { return 0x13B; } }

        public override ArmorMaterial Material { get { return ArmorMaterial.Gold; } }

        public override short BaseDurability { get { return 96; } }

        public override float BaseArmor { get { return 4; } }

        public override string DisplayName { get { return "Golden Chestplate"; } }
    }

    public class DiamondChestplateItem : ChestplateItem
    {
        public static readonly short ItemID = 0x137;

        public override short ID { get { return 0x137; } }

        public override ArmorMaterial Material { get { return ArmorMaterial.Diamond; } }

        public override short BaseDurability { get { return 384; } }

        public override float BaseArmor { get { return 4; } }

        public override string DisplayName { get { return "Diamond Chestplate"; } }
    }

    public class ChainChestplateItem : ArmorItem // Not HelmentItem because it can't inherit the recipe
    {
        public static readonly short ItemID = 0x12F;

        public override short ID { get { return 0x12F; } }

        public override ArmorMaterial Material { get { return ArmorMaterial.Chain; } }

        public override short BaseDurability { get { return 96; } }

        public override float BaseArmor { get { return 4; } }

        public override string DisplayName { get { return "Chain Chestplate"; } }
    }
}