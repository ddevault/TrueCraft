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
        int LeafRadius = 2;
        int LeafOffset = 3;
        public override bool GenerateAt(IWorld world, IChunk chunk, Coordinates3D location)
        {
            if (!ValidLocation(location))
                return false;

            Random R = new Random(world.Seed);
            int Height = R.Next(7, 8);
            GenerateColumn(chunk, location, Height, WoodBlock.BlockID, 0x1);
            GenerateCircle(chunk, location + new Coordinates3D(0, Height - 2, 0), LeafRadius - 1, LeavesBlock.BlockID, 0x1);
            GenerateCircle(chunk, location + new Coordinates3D(0, Height - 1, 0), LeafRadius, LeavesBlock.BlockID, 0x1);
            GenerateCircle(chunk, location + new Coordinates3D(0, Height, 0), LeafRadius, LeavesBlock.BlockID, 0x1);
            GenerateTopper(chunk, (location + new Coordinates3D(0, Height + 1, 0)), 0x0);
            return true;
        }
    }
}
