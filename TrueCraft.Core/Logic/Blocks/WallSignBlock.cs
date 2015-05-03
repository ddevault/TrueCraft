using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.API.Networking;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class WallSignBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x44;
        
        public override byte ID { get { return 0x44; } }
        
        public override double BlastResistance { get { return 5; } }

        public override double Hardness { get { return 1; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return true; } } // This is weird. You can stack signs on signs in Minecraft.
        
        public override string DisplayName { get { return "Sign"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(4, 0);
        }

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            world.SetMetadata(descriptor.Coordinates, (byte)MathHelper.DirectionByRotationFlat(user.Entity.Yaw, true));
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor)
        {
            return new[] { new ItemStack(SignItem.ItemID) };
        }
    }
}