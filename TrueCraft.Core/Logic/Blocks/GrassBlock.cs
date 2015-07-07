using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.API.Server;

namespace TrueCraft.Core.Logic.Blocks
{
    public class GrassBlock : BlockProvider
    {
        static GrassBlock()
        {
            GrowthCandidates = new Coordinates3D[3 * 3 * 5];
            int i = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    for (int y = -3; y <= 1; y++)
                    {
                        GrowthCandidates[i++] = new Coordinates3D(x, y, z);
                    }
                }
            }
        }

        private static readonly Coordinates3D[] GrowthCandidates;

        public static readonly int MaxDecayTime = 60 * 10;
        public static readonly int MinDecayTime = 60 * 2;

        public static readonly byte BlockID = 0x02;
        
        public override byte ID { get { return 0x02; } }
        
        public override double BlastResistance { get { return 3; } }

        public override double Hardness { get { return 0.6; } }

        public override byte Luminance { get { return 0; } }

        public override string DisplayName { get { return "Grass"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(0, 0);
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
        {
            return new[] { new ItemStack(DirtBlock.BlockID, 1) };
        }

        private void ScheduledUpdate(IWorld world, Coordinates3D coords)
        {
            if (world.IsValidPosition(coords + Coordinates3D.Up))
            {
                var id = world.GetBlockID(coords + Coordinates3D.Up);
                var provider = world.BlockRepository.GetBlockProvider(id);
                if (provider.Opaque)
                    world.SetBlockID(coords, DirtBlock.BlockID);
            }
        }

        public override void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiplayerServer server, IWorld world)
        {
            if (source.Coordinates == descriptor.Coordinates + Coordinates3D.Up)
            {
                var provider = world.BlockRepository.GetBlockProvider(source.ID);
                if (provider.Opaque)
                {
                    var chunk = world.FindChunk(descriptor.Coordinates, generate: false);
                    server.Scheduler.ScheduleEvent(chunk,
                    DateTime.UtcNow.AddSeconds(MathHelper.Random.Next(MinDecayTime, MaxDecayTime)), s =>
                    {
                        ScheduledUpdate(world, descriptor.Coordinates);
                    });
                }
            }
        }
    }
}