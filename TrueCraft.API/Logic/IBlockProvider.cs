using System;
using TrueCraft.API.World;
using TrueCraft.API.Networking;
using TrueCraft.API.Server;

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
        BoundingBox? BoundingBox { get; } // NOTE: Will this eventually need to be metadata-aware?
        Tuple<int, int> GetTextureMap(byte metadata);
        void GenerateDropEntity(BlockDescriptor descriptor, IWorld world, IMultiplayerServer server);
        bool BlockRightClicked(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user);
        void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user);
        void BlockMined(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user);
        void BlockUpdate(BlockDescriptor descriptor, IMultiplayerServer server, IWorld world);
        void BlockScheduledEvent(BlockDescriptor descriptor, IWorld world, object data);
    }
}