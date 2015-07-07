using System;
using TrueCraft.API.Logic;
using TrueCraft.API.Server;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;
using TrueCraft.API.Networking;

namespace TrueCraft.Core.Logic.Blocks
{
    public class SugarcaneBlock : BlockProvider
    {
        public static readonly int MinGrowthSeconds = 30;
        public static readonly int MaxGrowthSeconds = 120;

        public static readonly byte BlockID = 0x53;
        
        public override byte ID { get { return 0x53; } }
        
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Sugar cane"; } }

        public override BoundingBox? BoundingBox
        {
            get
            {
                return null;
            }
        }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(9, 4);
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
        {
            return new[] { new ItemStack(SugarCanesItem.ItemID) };
        }

        public static bool ValidPlacement(BlockDescriptor descriptor, IWorld world)
        {
            var below = world.GetBlockID(descriptor.Coordinates + Coordinates3D.Down);
            if (below != SugarcaneBlock.BlockID && below != GrassBlock.BlockID && below != DirtBlock.BlockID)
                return false;
            var toCheck = new[]
            {
                Coordinates3D.Down + Coordinates3D.Left,
                Coordinates3D.Down + Coordinates3D.Right,
                Coordinates3D.Down + Coordinates3D.Backwards,
                Coordinates3D.Down + Coordinates3D.Forwards
            };
            if (below != BlockID)
            {
                bool foundWater = false;
                for (int i = 0; i < toCheck.Length; i++)
                {
                    var id = world.GetBlockID(descriptor.Coordinates + toCheck[i]);
                    if (id == WaterBlock.BlockID || id == StationaryWaterBlock.BlockID)
                    {
                        foundWater = true;
                        break;
                    }
                }
                return foundWater;
            }
            return true;
        }

        public override void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiplayerServer server, IWorld world)
        {
            if (!ValidPlacement(descriptor, world))
            {
                // Destroy self
                world.SetBlockID(descriptor.Coordinates, 0);
                GenerateDropEntity(descriptor, world, server, ItemStack.EmptyStack);
            }
        }

        private void TryGrowth(IMultiplayerServer server, Coordinates3D coords, IWorld world)
        {
            if (world.GetBlockID(coords) != BlockID)
                return;
            // Find current height of stalk
            int height = 0;
            for (int y = -3; y <= 3; y++)
            {
                if (world.GetBlockID(coords + (Coordinates3D.Down * y)) == BlockID)
                    height++;
            }
            if (height < 3)
            {
                var meta = world.GetMetadata(coords);
                meta++;
                world.SetMetadata(coords, meta);
                var chunk = world.FindChunk(coords);
                if (meta == 15)
                {
                    world.SetBlockID(coords + Coordinates3D.Up, BlockID);
                    server.Scheduler.ScheduleEvent(chunk,
                        DateTime.UtcNow.AddSeconds(MathHelper.Random.Next(MinGrowthSeconds, MaxGrowthSeconds)),
                        (_server) => TryGrowth(_server, coords + Coordinates3D.Up, world));
                }
                else
                {
                    server.Scheduler.ScheduleEvent(chunk,
                        DateTime.UtcNow.AddSeconds(MathHelper.Random.Next(MinGrowthSeconds, MaxGrowthSeconds)),
                        (_server) => TryGrowth(_server, coords, world));
                }
            }
        }

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            var chunk = world.FindChunk(descriptor.Coordinates);
            user.Server.Scheduler.ScheduleEvent(chunk,
                DateTime.UtcNow.AddSeconds(MathHelper.Random.Next(MinGrowthSeconds, MaxGrowthSeconds)),
                (server) => TryGrowth(server, descriptor.Coordinates, world));
        }

        public override void BlockLoadedFromChunk(Coordinates3D coords, IMultiplayerServer server, IWorld world)
        {
            var chunk = world.FindChunk(coords);
            server.Scheduler.ScheduleEvent(chunk,
                DateTime.UtcNow.AddSeconds(MathHelper.Random.Next(MinGrowthSeconds, MaxGrowthSeconds)),
                s => TryGrowth(s, coords, world));
        }
    }
}