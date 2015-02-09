using System;
using TrueCraft.API.Logic;
using TrueCraft.API.Networking;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.API.Server;

namespace TrueCraft.Core.Logic.Blocks
{
    public class FarmlandBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x3C;
        
        public override byte ID { get { return 0x3C; } }
        
        public override double BlastResistance { get { return 3; } }

        public override double Hardness { get { return 0.6; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }

        public override byte LightModifier { get { return 255; } }
        
        public override string DisplayName { get { return "Farmland"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(7, 5);
        }

        public bool IsHydrated(Coordinates3D coordinates, IWorld world)
        {
            var min = new Coordinates3D(-6 + coordinates.X, coordinates.Y, -6 + coordinates.Z);
            var max = new Coordinates3D(6 + coordinates.X, coordinates.Y + 1, 6 + coordinates.Z);
            for (int x = min.X; x < max.X; x++)
            {
                for (int y = min.Y; y < max.Y; y++) // TODO: This does not check one above the farmland block for some reason
                {
                    for (int z = min.Z; z < max.Z; z++)
                    {
                        var id = world.GetBlockID(new Coordinates3D(x, y, z));
                        if (id == WaterBlock.BlockID || id == StationaryWaterBlock.BlockID)
                            return true;
                    }
                }
            }
            return false;
        }

        void HydrationCheckEvent(IMultiplayerServer server, Coordinates3D coords, IWorld world)
        {
            if (world.GetBlockID(coords) != BlockID)
                return;
            if (MathHelper.Random.Next(3) == 0)
            {
                if (IsHydrated(coords, world))
                {
                    world.SetMetadata(coords, 15);
                }
                else
                {
                    world.SetBlockID(coords, DirtBlock.BlockID);
                }
            }
            server.Scheduler.ScheduleEvent(DateTime.Now.AddSeconds(30), (_server) => HydrationCheckEvent(_server, coords, world));
        }

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            if (IsHydrated(descriptor.Coordinates, world))
            {
                world.SetMetadata(descriptor.Coordinates, 15);
            }
            user.Server.Scheduler.ScheduleEvent(DateTime.Now.AddSeconds(30), (server) => HydrationCheckEvent(server, descriptor.Coordinates, world));
        }
    }
}