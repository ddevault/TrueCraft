using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public abstract class BootsItem : ArmorItem, ICraftingRecipe
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
                    { new ItemStack(baseMaterial), ItemStack.EmptyStack, new ItemStack(baseMaterial) }
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

    public class LeatherBootsItem : BootsItem
    {
        public static readonly short ItemID = 0x12D;

        public override short ID { get { return 0x12D; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(0, 3);
        }

        public override ArmorMaterial Material { get { return ArmorMaterial.Leather; } }

        public override short BaseDurability { get { return 40; } }

        public override float BaseArmor { get { return 1.5f; } }

        public override string DisplayName { get { return "Leather Boots"; } }
    }

    public class IronBootsItem : BootsItem
    {
        public static readonly short ItemID = 0x135;

        public override short ID { get { return 0x135; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(2, 3);
        }

        public override ArmorMaterial Material { get { return ArmorMaterial.Iron; } }

        public override short BaseDurability { get { return 160; } }

        public override float BaseArmor { get { return 1.5f; } }

        public override string DisplayName { get { return "Iron Boots"; } }
    }

    public class GoldenBootsItem : BootsItem
    {
        public static readonly short ItemID = 0x13D;

        public override short ID { get { return 0x13D; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(4, 3);
        }

        public override ArmorMaterial Material { get { return ArmorMaterial.Gold; } }

        public override short BaseDurability { get { return 80; } }

        public override float BaseArmor { get { return 1.5f; } }

        public override string DisplayName { get { return "Golden Boots"; } }
    }

    public class DiamondBootsItem : BootsItem
    {
        public static readonly short ItemID = 0x139;

        public override short ID { get { return 0x139; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(3, 3);
        }

        public override ArmorMaterial Material { get { return ArmorMaterial.Diamond; } }

        public override short BaseDurability { get { return 320; } }

        public override float BaseArmor { get { return 1.5f; } }

        public override string DisplayName { get { return "Diamond Boots"; } }
    }

    public class ChainBootsItem : ArmorItem // Not HelmentItem because it can't inherit the recipe
    {
        public static readonly short ItemID = 0x131;

        public override short ID { get { return 0x131; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(1, 3);
        }

        public override ArmorMaterial Material { get { return ArmorMaterial.Chain; } }

        public override short BaseDurability { get { return 79; } }

        public override float BaseArmor { get { return 1.5f; } }

        public override string DisplayName { get { return "Chain Boots"; } }
    }
}