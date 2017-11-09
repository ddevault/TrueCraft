using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.World;

namespace TrueCraft.Core.TerrainGen.Decorations
{
    public class OakTree : Decoration
    {
        const int LeafRadius = 2;

        public override bool ValidLocation(Coordinates3D location)
        {
            if (location.X - LeafRadius < 0
                || location.X + LeafRadius >= Chunk.Width
                || location.Z - LeafRadius < 0
                || location.Z + LeafRadius >= Chunk.Depth)
                return false;
            return true;
        }

        public override bool GenerateAt(IWorldSeed world, ISpatialBlockInformationProvider chunk, Coordinates3D location)
        {
            if (!ValidLocation(location))
                return false;

            var random = new Random(world.Seed);
            int height = random.Next(4, 5);
            GenerateColumn(chunk, location, height, WoodBlock.BlockID, 0x0);
            Coordinates3D LeafLocation = location + new Coordinates3D(0, height, 0);
            GenerateVanillaLeaves(chunk, LeafLocation, LeafRadius, LeavesBlock.BlockID, 0x0);
            return true;
        }
    }
}
