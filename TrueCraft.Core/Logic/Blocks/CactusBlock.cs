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
        public static readonly int MinGrowthSeconds = 30;
        public static readonly int MaxGrowthSeconds = 60;
        public static readonly int MaxGrowHeight = 3;

        public static readonly byte BlockID = 0x51;
        
        public override byte ID { get { return 0x51; } }
        
        public override double BlastResistance { get { return 2; } }

        public override double Hardness { get { return 0.4; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Cactus"; } }

        public override SoundEffectClass SoundEffect
        {
            get
            {
                return SoundEffectClass.Cloth;
            }
        }

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

        private void TryGrowth(IMultiplayerServer server, Coordinates3D coords, IWorld world)
        {
            if (world.GetBlockID(coords) != BlockID)
                return;
            // Find current height of stalk
            int height = 0;
            for (int y = -MaxGrowHeight; y <= MaxGrowHeight; y++)
            {
                if (world.GetBlockID(coords + (Coordinates3D.Down * y)) == BlockID)
                    height++;
            }
            if (height < MaxGrowHeight)
            {
                var meta = world.GetMetadata(coords);
                meta++;
                world.SetMetadata(coords, meta);
                var chunk = world.FindChunk(coords);
                if (meta == 15)
                {
                    if (world.GetBlockID(coords + Coordinates3D.Up) == 0)
                    {
                        world.SetBlockID(coords + Coordinates3D.Up, BlockID);
                        server.Scheduler.ScheduleEvent("cactus", chunk,
                            TimeSpan.FromSeconds(MathHelper.Random.Next(MinGrowthSeconds, MaxGrowthSeconds)),
                            (_server) => TryGrowth(_server, coords + Coordinates3D.Up, world));
                    }
                }
                else
                {
                    server.Scheduler.ScheduleEvent("cactus", chunk,
                        TimeSpan.FromSeconds(MathHelper.Random.Next(MinGrowthSeconds, MaxGrowthSeconds)),
                        (_server) => TryGrowth(_server, coords, world));
                }
            }
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
            {
                world.SetBlockID(descriptor.Coordinates, AirBlock.BlockID);

                var manager = user.Server.GetEntityManagerForWorld(world);
                manager.SpawnEntity(
                    new ItemEntity(descriptor.Coordinates + Coordinates3D.Up,
                        new ItemStack(CactusBlock.BlockID, (sbyte)1)));
                // user.Inventory.PickUpStack() wasn't working?
            }

            var chunk = world.FindChunk(descriptor.Coordinates);
            user.Server.Scheduler.ScheduleEvent("cactus", chunk,
                TimeSpan.FromSeconds(MathHelper.Random.Next(MinGrowthSeconds, MaxGrowthSeconds)),
                (server) => TryGrowth(server, descriptor.Coordinates, world));
        }

        public override void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiplayerServer server, IWorld world)
        {
            if (!ValidCactusPosition(descriptor, server.BlockRepository, world))
                DestroyCactus(descriptor, server, world);
            base.BlockUpdate(descriptor, source, server, world);
        }

        public override void BlockLoadedFromChunk(Coordinates3D coords, IMultiplayerServer server, IWorld world)
        {
            var chunk = world.FindChunk(coords);
            server.Scheduler.ScheduleEvent("cactus", chunk,
                TimeSpan.FromSeconds(MathHelper.Random.Next(MinGrowthSeconds, MaxGrowthSeconds)),
                s => TryGrowth(s, coords, world));
        }
    }
}