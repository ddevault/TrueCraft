using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.Server;
using TrueCraft.API.Networking;
using TrueCraft.API.World;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class LeavesBlock : BlockProvider
    {
        public static readonly int MinDecayTime = 30;
        public static readonly int MaxDecayTime = 120;

        static LeavesBlock()
        {
            Adjacent = new Coordinates3D[5 * 5 * 5];
            int i = 0;
            for (int x = -2; x <= 2; x++)
            {
                for (int z = -2; z <= 2; z++)
                {
                    for (int y = -2; y <= 2; y++)
                    {
                        Adjacent[i++] = new Coordinates3D(x, y, z);
                    }
                }
            }
        }

        private static readonly Coordinates3D[] Adjacent;

        public static readonly byte BlockID = 0x12;

        public override byte ID { get { return 0x12; } }

        public override double BlastResistance { get { return 1; } }

        public override double Hardness { get { return 0.2; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override bool DiffuseSkyLight { get { return true; } }

        public override byte LightOpacity { get { return 2; } }

        public override string DisplayName { get { return "Leaves"; } }

        public override bool Flammable { get { return true; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(4, 3);
        }

        public void TryDecay(Coordinates3D coords, IMultiplayerServer server, IWorld world)
        {
            foreach (Coordinates3D a in Adjacent)
            {
                var c = a + coords;
                var id = world.GetBlockID(c);
                if (id == WoodBlock.BlockID)
                {
                    world.SetMetadata(coords, 0x0);
                    return;
                }
                else continue;
                }

            world.SetBlockID(coords, AirBlock.BlockID);
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
        {
            var provider = ItemRepository.GetItemProvider(item.ID);
            if (provider is ShearsItem)
                return base.GetDrop(descriptor, item);
            else
            {
                if (MathHelper.Random.Next(20) == 0) // 5% chance
                    return new[] { new ItemStack(SaplingBlock.BlockID, 1, descriptor.Metadata) };
                else
                    return new ItemStack[0];
            }
        }

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            world.SetMetadata(descriptor.Coordinates, 0x4);

            var chunk = world.FindChunk(descriptor.Coordinates);
            user.Server.Scheduler.ScheduleEvent("leaves", chunk,
                TimeSpan.FromSeconds(MathHelper.Random.Next(MinDecayTime, MaxDecayTime)),
                s => TryDecay(descriptor.Coordinates, user.Server, world));
        }

        public override void BlockLoadedFromChunk(Coordinates3D coords, IMultiplayerServer server, IWorld world)
        {
            if (world.GetMetadata(coords) == 0x4)
            {
                var chunk = world.FindChunk(coords);
                server.Scheduler.ScheduleEvent("leaves", chunk,
                    TimeSpan.FromSeconds(MathHelper.Random.Next(MinDecayTime, MaxDecayTime)),
                    s => TryDecay(coords, server, world));
            }
        }

        public override void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiplayerServer server, IWorld world)
        {
            var chunk = world.FindChunk(descriptor.Coordinates);
            server.Scheduler.ScheduleEvent("leaves", chunk,
                TimeSpan.FromSeconds(MathHelper.Random.Next(MinDecayTime, MaxDecayTime)),
                s => TryDecay(descriptor.Coordinates, server, world));  
        }
    }
}
