using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public abstract class LeggingsItem : ArmorItem, ICraftingRecipe
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
                    { new ItemStack(baseMaterial), new ItemStack(baseMaterial), new ItemStack(baseMaterial) },
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

    public class LeatherPantsItem : LeggingsItem
    {
        public static readonly short ItemID = 0x12C;

        public override short ID { get { return 0x12C; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(0, 2);
        }

        public override ArmorMaterial Material { get { return ArmorMaterial.Leather; } }

        public override short BaseDurability { get { return 46; } }

        public override float BaseArmor { get { return 3; } }

        public override string DisplayName { get { return "Leather Pants"; } }
    }

    public class IronLeggingsItem : LeggingsItem
    {
        public static readonly short ItemID = 0x134;

        public override short ID { get { return 0x134; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(2, 2);
        }

        public override ArmorMaterial Material { get { return ArmorMaterial.Iron; } }

        public override short BaseDurability { get { return 184; } }

        public override float BaseArmor { get { return 3; } }

        public override string DisplayName { get { return "Iron Leggings"; } }
    }

    public class GoldenLeggingsItem : LeggingsItem
    {
        public static readonly short ItemID = 0x13C;

        public override short ID { get { return 0x13C; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(4, 2);
        }

        public override ArmorMaterial Material { get { return ArmorMaterial.Gold; } }

        public override short BaseDurability { get { return 92; } }

        public override float BaseArmor { get { return 3; } }

        public override string DisplayName { get { return "Golden Leggings"; } }
    }

    public class DiamondLeggingsItem : LeggingsItem
    {
        public static readonly short ItemID = 0x138;

        public override short ID { get { return 0x138; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(3, 2);
        }

        public override ArmorMaterial Material { get { return ArmorMaterial.Diamond; } }

        public override short BaseDurability { get { return 368; } }

        public override float BaseArmor { get { return 3; } }

        public override string DisplayName { get { return "Diamond Leggings"; } }
    }

    public class ChainLeggingsItem : ArmorItem // Not HelmentItem because it can't inherit the recipe
    {
        public static readonly short ItemID = 0x130;

        public override short ID { get { return 0x130; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(1, 2);
        }

        public override ArmorMaterial Material { get { return ArmorMaterial.Chain; } }

        public override short BaseDurability { get { return 92; } }

        public override float BaseArmor { get { return 3; } }

        public override string DisplayName { get { return "Chain Leggings"; } }
    }
}