using System;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;
using TrueCraft.API.Networking;
using TrueCraft.API.World;
using TrueCraft.API.Server;

namespace TrueCraft.Core.Logic.Blocks
{
    public class CropsBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x3B;
        
        public override byte ID { get { return 0x3B; } }
        
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Crops"; } }

        public override TrueCraft.API.BoundingBox? BoundingBox { get { return null; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(8, 5);
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
        {
            if (descriptor.Metadata >= 7)
                return new[] { new ItemStack(WheatItem.ItemID), new ItemStack(SeedsItem.ItemID, (sbyte)MathHelper.Random.Next(3)) };
            else
                return new[] { new ItemStack(SeedsItem.ItemID) };
        }

        private void GrowBlock(IMultiplayerServer server, IWorld world, Coordinates3D coords)
        {
            if (world.GetBlockID(coords) != BlockID)
                return;
            var meta = world.GetMetadata(coords);
            meta++;
            world.SetMetadata(coords, meta);
            if (meta < 7)
            {
                var chunk = world.FindChunk(coords);
                server.Scheduler.ScheduleEvent(
                    chunk, DateTime.UtcNow.AddSeconds(MathHelper.Random.Next(30, 60)),
                   (_server) => GrowBlock(_server, world, coords));
            }
        }

        public override void BlockUpdate(BlockDescriptor descriptor, BlockDescriptor source, IMultiplayerServer server, IWorld world)
        {
            if (world.GetBlockID(descriptor.Coordinates + Coordinates3D.Down) != FarmlandBlock.BlockID)
            {
                GenerateDropEntity(descriptor, world, server, ItemStack.EmptyStack);
                world.SetBlockID(descriptor.Coordinates, 0);
            }
        }

        public override void BlockPlaced(BlockDescriptor descriptor, BlockFace face, IWorld world, IRemoteClient user)
        {
            var chunk = world.FindChunk(descriptor.Coordinates);
            user.Server.Scheduler.ScheduleEvent(chunk, DateTime.UtcNow.AddSeconds(MathHelper.Random.Next(30, 60)),
                (server) => GrowBlock(server, world, descriptor.Coordinates + MathHelper.BlockFaceToCoordinates(face)));
        }

        public override void BlockLoadedFromChunk(Coordinates3D coords, IMultiplayerServer server, IWorld world)
        {
            var chunk = world.FindChunk(coords);
            server.Scheduler.ScheduleEvent(chunk, DateTime.UtcNow.AddSeconds(MathHelper.Random.Next(30, 60)),
                (s) => GrowBlock(s, world, coords));
        }
    }
}
