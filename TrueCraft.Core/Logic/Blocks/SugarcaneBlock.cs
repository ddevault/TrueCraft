using System;
using TrueCraft.API.Logic;
using TrueCraft.API.Server;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
    public class SugarcaneBlock : BlockProvider
    {
        public static readonly byte BlockID = 0x53;
        
        public override byte ID { get { return 0x53; } }
        
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Sugar cane"; } }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(9, 4);
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor)
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

        public override void BlockUpdate(BlockDescriptor descriptor, IMultiplayerServer server, IWorld world)
        {
            if (!ValidPlacement(descriptor, world))
            {
                // Destroy self
                world.SetBlockID(descriptor.Coordinates, 0);
                GenerateDropEntity(descriptor, world, server);
            }
        }
    }
}