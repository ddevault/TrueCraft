using System;
using TrueCraft.API.World;
using TrueCraft.API.Networking;

namespace TrueCraft.API.Logic
{
    public interface IBlockProvider
    {
        byte ID { get; }
        double BlastResistance { get; }
        double Hardness { get; }
        byte Luminance { get; }
        bool Opaque { get; }
        byte LightModifier { get; }
        string DisplayName { get; }
        Tuple<int, int> GetTextureMap(byte metadata);
        bool BlockRightClicked(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user);
        bool BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user);
        void BlockMined(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user);
        void BlockUpdate(BlockDescriptor descriptor, IWorld world);
        void BlockScheduledEvent(BlockDescriptor descriptor, IWorld world, object data);
    }
}