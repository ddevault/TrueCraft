using System;
using TrueCraft.API.World;
using TrueCraft.API.Networking;
using TrueCraft.API.Server;
using fNbt;

namespace TrueCraft.API.Logic
{
    public interface IBlockProvider
    {
        byte ID { get; }
        double BlastResistance { get; }
        double Hardness { get; }
        byte Luminance { get; }
        bool Opaque { get; }
        bool RenderOpaque { get; }
        byte LightOpacity { get; }
        bool DiffuseSkyLight { get; }
        ToolMaterial EffectiveToolMaterials { get; }
        ToolType EffectiveTools { get; }
        string DisplayName { get; }
        BoundingBox? BoundingBox { get; } // NOTE: Will this eventually need to be metadata-aware?
        Tuple<int, int> GetTextureMap(byte metadata);
        void GenerateDropEntity(BlockDescriptor descriptor, IWorld world, IMultiplayerServer server, ItemStack heldItem);
        void BlockLeftClicked(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user);
        bool BlockRightClicked(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user);
        void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user);
        void BlockMined(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user);
        void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiplayerServer server, IWorld world);
        void BlockLoadedFromChunk(Coordinates3D coords, IMultiplayerServer server, IWorld world);
        void TileEntityLoadedForClient(BlockDescriptor descriptor, IWorld world, NbtCompound compound, IRemoteClient client);
    }
}