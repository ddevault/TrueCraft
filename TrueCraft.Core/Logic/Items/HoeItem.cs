using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API.World;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Logic.Items
{
    public abstract class HoeItem : ToolItem, ICraftingRecipe
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
                    { ItemStack.EmptyStack, new ItemStack(StickItem.ItemID) },
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
                return ToolType.Hoe;
            }
        }

        public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world, IRemoteClient user)
        {
            var id = world.GetBlockID(coordinates);
            if (id == DirtBlock.BlockID || id == GrassBlock.BlockID)
            {
                world.SetBlockID(coordinates, FarmlandBlock.BlockID);
                user.Server.BlockRepository.GetBlockProvider(FarmlandBlock.BlockID).BlockPlaced(
                    new BlockDescriptor { Coordinates = coordinates }, face, world, user);
            }
        }
    }

    public class WoodenHoeItem : HoeItem
    {
        public static readonly short ItemID = 0x122;

        public override short ID { get { return 0x122; } }

        public override ToolMaterial Material { get { return ToolMaterial.Wood; } }

        public override short BaseDurability { get { return 60; } }

        public override string DisplayName { get { return "Wooden Hoe"; } }
    }

    public class StoneHoeItem : HoeItem
    {
        public static readonly short ItemID = 0x123;

        public override short ID { get { return 0x123; } }

        public override ToolMaterial Material { get { return ToolMaterial.Stone; } }

        public override short BaseDurability { get { return 132; } }

        public override string DisplayName { get { return "Stone Hoe"; } }
    }

    public class IronHoeItem : HoeItem
    {
        public static readonly short ItemID = 0x124;

        public override short ID { get { return 0x124; } }

        public override ToolMaterial Material { get { return ToolMaterial.Iron; } }

        public override short BaseDurability { get { return 251; } }

        public override string DisplayName { get { return "Iron Hoe"; } }
    }

    public class GoldenHoeItem : HoeItem
    {
        public static readonly short ItemID = 0x126;

        public override short ID { get { return 0x126; } }

        public override ToolMaterial Material { get { return ToolMaterial.Gold; } }

        public override short BaseDurability { get { return 33; } }

        public override string DisplayName { get { return "Golden Hoe"; } }
    }

    public class DiamondHoeItem : HoeItem
    {
        public static readonly short ItemID = 0x125;

        public override short ID { get { return 0x125; } }

        public override ToolMaterial Material { get { return ToolMaterial.Diamond; } }

        public override short BaseDurability { get { return 1562; } }

        public override string DisplayName { get { return "Diamond Hoe"; } }
    }
}