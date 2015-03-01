using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.World;

namespace TrueCraft.Core.TerrainGen.Decorations
{
    public class BirchTree : Decoration
    {
        int LeafRadius = 2;

        public override bool ValidLocation(Coordinates3D location)
        {
            if (location.X - LeafRadius < 0 || location.X + LeafRadius >= Chunk.Width || location.Z - LeafRadius < 0 || location.Z + LeafRadius >= Chunk.Depth)
                return false;
            return true;
        }

        public override bool GenerateAt(IWorld world, IChunk chunk, Coordinates3D location)
        {
            if (!ValidLocation(location))
                return false;

            Random R = new Random(world.Seed);
            int Height = R.Next(4, 5);
            GenerateColumn(chunk, location, Height, WoodBlock.BlockID, 0x2);
            Coordinates3D LeafLocation = location + new Coordinates3D(0, Height, 0);
            GenerateVanillaLeaves(chunk, LeafLocation, LeafRadius, LeavesBlock.BlockID, 0x2);
            return true;
        }
    }
}