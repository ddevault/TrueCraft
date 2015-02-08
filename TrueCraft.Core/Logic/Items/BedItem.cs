using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.API.Networking;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
    public class BedItem : ItemProvider, ICraftingRecipe
    {
        public static readonly short ItemID = 0x163;

        public override short ID { get { return 0x163; } }

        public override sbyte MaximumStack { get { return 1; } }

        public override string DisplayName { get { return "Bed"; } }

        public override void ItemUsedOnBlock(Coordinates3D coordinates, ItemStack item, BlockFace face, IWorld world, IRemoteClient user)
        {
            coordinates += MathHelper.BlockFaceToCoordinates(face);
            var head = coordinates;
            var foot = coordinates;
            BedBlock.BedDirection direction = BedBlock.BedDirection.North;
            switch (MathHelper.DirectionByRotationFlat(user.Entity.Yaw))
            {
                case Direction.North:
                    head += Coordinates3D.North;
                    direction = BedBlock.BedDirection.North;
                    break;
                case Direction.South:
                    head += Coordinates3D.South;
                    direction = BedBlock.BedDirection.South;
                    break;
                case Direction.East:
                    head += Coordinates3D.East;
                    direction = BedBlock.BedDirection.East;
                    break;
                case Direction.West:
                    head += Coordinates3D.West;
                    direction = BedBlock.BedDirection.West;
                    break;
            }
            var bedProvider = (BedBlock)user.Server.BlockRepository.GetBlockProvider(BedBlock.BlockID);
            if (!bedProvider.ValidBedPosition(new BlockDescriptor { Coordinates = head },
                user.Server.BlockRepository, user.World, false, true) ||
                !bedProvider.ValidBedPosition(new BlockDescriptor { Coordinates = foot },
                user.Server.BlockRepository, user.World, false, true))
            {
                return;
            }
            user.Server.BlockUpdatesEnabled = false;
            world.SetBlockData(head, new BlockDescriptor
                { ID = BedBlock.BlockID, Metadata = (byte)((byte)direction | (byte)BedBlock.BedType.Head) });
            world.SetBlockData(foot, new BlockDescriptor
                { ID = BedBlock.BlockID, Metadata = (byte)((byte)direction | (byte)BedBlock.BedType.Foot) });
            user.Server.BlockUpdatesEnabled = true;
            item.Count--;
            user.Inventory[user.SelectedSlot] = item;
        }

        public ItemStack[,] Pattern
        {
            get
            {
                return new[,]
                {
                    {
                        new ItemStack(WoolBlock.BlockID),
                        new ItemStack(WoolBlock.BlockID),
                        new ItemStack(WoolBlock.BlockID),
                    },
                    {
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(WoodenPlanksBlock.BlockID),
                        new ItemStack(WoodenPlanksBlock.BlockID),
                    }
                };
            }
        }

        public ItemStack Output
        {
            get
            {
                return new ItemStack(ItemID);
            }
        }
    }
}