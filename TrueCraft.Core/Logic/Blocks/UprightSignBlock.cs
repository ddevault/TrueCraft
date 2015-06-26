using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.API.Networking;
using TrueCraft.Core.Logic.Items;
using fNbt;
using TrueCraft.Core.Networking.Packets;

namespace TrueCraft.Core.Logic.Blocks
{
    public class UprightSignBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x3F;
        
        public override byte ID { get { return 0x3F; } }
        
        public override double BlastResistance { get { return 5; } }

        public override double Hardness { get { return 1; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return true; } } // This is weird. You can stack signs on signs in Minecraft.
        
        public override string DisplayName { get { return "Sign"; } }

        public override BoundingBox? BoundingBox { get { return null; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(4, 0);
        }

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            double rotation = user.Entity.Yaw + 180 % 360;
            if (rotation < 0)
                rotation += 360;

            world.SetMetadata(descriptor.Coordinates, (byte)(rotation / 22.5));
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
        {
            return new[] { new ItemStack(SignItem.ItemID) };
        }

        public override void BlockMined(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            world.SetTileEntity(descriptor.Coordinates, null);
            base.BlockMined(descriptor, face, world, user);
        }

        public override void TileEntityLoadedForClient(BlockDescriptor descriptor, IWorld world, NbtCompound entity, IRemoteClient client)
        {
            client.QueuePacket(new UpdateSignPacket
            {
                X = descriptor.Coordinates.X,
                Y = (short)descriptor.Coordinates.Y,
                Z = descriptor.Coordinates.Z,
                Text = new[]
                {
                    entity["Text1"].StringValue,
                    entity["Text2"].StringValue,
                    entity["Text3"].StringValue,
                    entity["Text4"].StringValue
                }
            });
        }
    }
}
