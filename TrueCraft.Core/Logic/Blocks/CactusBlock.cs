using System;
using System.Linq;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.API.World;
using TrueCraft.API.Networking;
using TrueCraft.Core.Entities;
using TrueCraft.Core.Logic.Items;
using TrueCraft.API.Server;

namespace TrueCraft.Core.Logic.Blocks
{
    public class CactusBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x51;
        
        public override byte ID { get { return 0x51; } }
        
        public override double BlastResistance { get { return 2; } }

        public override double Hardness { get { return 0.4; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Cactus"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(6, 4);
        }

        public bool ValidCactusPosition(BlockDescriptor descriptor, IBlockRepository repository, IWorld world, bool checkNeighbor = true, bool checkSupport = true)
        {
            if (checkNeighbor)
            {
                var adjacent = new Coordinates3D[]
                {
                    descriptor.Coordinates + Coordinates3D.North,
                    descriptor.Coordinates + Coordinates3D.East,
                    descriptor.Coordinates + Coordinates3D.South,
                    descriptor.Coordinates + Coordinates3D.West,
                };

                foreach (var coords in adjacent)
                    if (world.GetBlockID(coords) != AirBlock.BlockID)
                        return false;
            }

            if (checkSupport)
            {
                var supportingBlock = repository.GetBlockProvider(world.GetBlockID(descriptor.Coordinates + Coordinates3D.Down));
                if ((supportingBlock.ID != CactusBlock.BlockID) && (supportingBlock.ID != SandBlock.BlockID))
                    return false;
            }

            return true;
        }

        public void DestroyCactus(BlockDescriptor descriptor, IMultiplayerServer server, IWorld world)
        {
            var toDrop = 0;

            // Search upwards
            for (int y = descriptor.Coordinates.Y; y < 127; y++)
            {
                var coordinates = new Coordinates3D(descriptor.Coordinates.X, y, descriptor.Coordinates.Z);
                if (world.GetBlockID(coordinates) == CactusBlock.BlockID)
                {
                    world.SetBlockID(coordinates, AirBlock.BlockID);
                    toDrop++;
                }
            }

            // Search downwards.
            for (int y = descriptor.Coordinates.Y - 1; y > 0; y--)
            {
                var coordinates = new Coordinates3D(descriptor.Coordinates.X, y, descriptor.Coordinates.Z);
                if (world.GetBlockID(coordinates) == CactusBlock.BlockID)
                {
                    world.SetBlockID(coordinates, AirBlock.BlockID);
                    toDrop++;
                }
            }

            var manager = server.GetEntityManagerForWorld(world);
            manager.SpawnEntity(
                new ItemEntity(descriptor.Coordinates + Coordinates3D.Up,
                    new ItemStack(CactusBlock.BlockID, (sbyte)toDrop)));
        }

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            if (ValidCactusPosition(descriptor, user.Server.BlockRepository, world))
                base.BlockPlaced(descriptor, face, world, user);
            else
                user.Inventory.PickUpStack(new ItemStack(CactusBlock.BlockID, (sbyte)1));
        }

        public override void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiplayerServer server, IWorld world)
        {
            if (!ValidCactusPosition(descriptor, server.BlockRepository, world))
                DestroyCactus(descriptor, server, world);
            base.BlockUpdate(descriptor, source, server, world);
        }
    }
}