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
    public class ConiferTree : PineTree
    {
        const int LeafRadius = 2;

        public override bool GenerateAt(IWorld world, IChunk chunk, Coordinates3D location)
        {
            if (!ValidLocation(location))
                return false;

            var random = new Random(world.Seed);
            int height = random.Next(7, 8);
            GenerateColumn(chunk, location, height, WoodBlock.BlockID, 0x1);
            GenerateCircle(chunk, location + new Coordinates3D(0, height - 2, 0), LeafRadius - 1, LeavesBlock.BlockID, 0x1);
            GenerateCircle(chunk, location + new Coordinates3D(0, height - 1, 0), LeafRadius, LeavesBlock.BlockID, 0x1);
            GenerateCircle(chunk, location + new Coordinates3D(0, height, 0), LeafRadius, LeavesBlock.BlockID, 0x1);
            GenerateTopper(chunk, (location + new Coordinates3D(0, height + 1, 0)), 0x0);
            return true;
        }
    }
}
