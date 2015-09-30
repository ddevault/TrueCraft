using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Items
{
    public abstract class HelmentItem : ArmorItem, ICraftingRecipe
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

    public class LeatherCapItem : HelmentItem
    {
        public static readonly short ItemID = 0x12A;

        public override short ID { get { return 0x12A; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(0, 0);
        }

        public override ArmorMaterial Material { get { return ArmorMaterial.Leather; } }

        public override short BaseDurability { get { return 34; } }

        public override float BaseArmor { get { return 1.5f; } }

        public override string DisplayName { get { return "Leather Cap"; } }
    }

    public class IronHelmetItem : HelmentItem
    {
        public static readonly short ItemID = 0x132;

        public override short ID { get { return 0x132; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(2, 0);
        }

        public override ArmorMaterial Material { get { return ArmorMaterial.Iron; } }

        public override short BaseDurability { get { return 136; } }

        public override float BaseArmor { get { return 1.5f; } }

        public override string DisplayName { get { return "Iron Helmet"; } }
    }

    public class GoldenHelmetItem : HelmentItem
    {
        public static readonly short ItemID = 0x13A;

        public override short ID { get { return 0x13A; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(4, 0);
        }

        public override ArmorMaterial Material { get { return ArmorMaterial.Gold; } }

        public override short BaseDurability { get { return 68; } }

        public override float BaseArmor { get { return 1.5f; } }

        public override string DisplayName { get { return "Golden Helmet"; } }
    }

    public class DiamondHelmetItem : HelmentItem
    {
        public static readonly short ItemID = 0x136;

        public override short ID { get { return 0x136; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(3, 0);
        }

        public override ArmorMaterial Material { get { return ArmorMaterial.Diamond; } }

        public override short BaseDurability { get { return 272; } }

        public override float BaseArmor { get { return 1.5f; } }

        public override string DisplayName { get { return "Diamond Helmet"; } }
    }

    public class ChainHelmetItem : ArmorItem // Not HelmentItem because it can't inherit the recipe
    {
        public static readonly short ItemID = 0x12E;

        public override short ID { get { return 0x12E; } }

        public override Tuple<int, int> GetIconTexture(byte metadata)
        {
            return new Tuple<int, int>(1, 0);
        }

        public override ArmorMaterial Material { get { return ArmorMaterial.Chain; } }

        public override short BaseDurability { get { return 67; } }

        public override float BaseArmor { get { return 1.5f; } }

        public override string DisplayName { get { return "Chain Helmet"; } }
    }
}