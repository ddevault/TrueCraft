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
    public class PineTree : Decoration
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

        public override bool GenerateAt(IWorldSeed world, ApplesauceChunk chunk, Coordinates3D location)
        {
            if (!ValidLocation(location))
                return false;

            var random = new Random(world.Seed);
            int height = random.Next(7, 8);
            GenerateColumn(chunk, location, height, WoodBlock.BlockID, 0x1);
            for (int y = 1; y < height; y++)
            {
                if (y % 2 == 0)
                {
                    GenerateVanillaCircle(chunk, location + new Coordinates3D(0, y + 1, 0), LeafRadius - 1, LeavesBlock.BlockID, 0x1);
                    continue;
                }
                GenerateVanillaCircle(chunk, location + new Coordinates3D(0, y + 1, 0), LeafRadius, LeavesBlock.BlockID, 0x1);
            }

            GenerateTopper(chunk, location + new Coordinates3D(0, height, 0), 0x1);
            return true;
        }

        /*
         * Generates the top of the pine/conifer trees.
         * Type:
         * 0x0 - two level topper
         * 0x1 - three level topper
         */
        protected void GenerateTopper(ApplesauceChunk chunk, Coordinates3D location, byte type = 0x0)
        {
            const int sectionRadius = 1;
            GenerateCircle(chunk, location, sectionRadius, LeavesBlock.BlockID, 0x1);
            var top = location + Coordinates3D.Up;
            chunk.SetBlockID(top, LeavesBlock.BlockID);
            chunk.SetMetadata(top, 0x1);
            if (type == 0x1 && (top + Coordinates3D.Up).Y < Chunk.Height)
                GenerateVanillaCircle(chunk, top + Coordinates3D.Up, sectionRadius, LeavesBlock.BlockID, 0x1); 
        }
    }
}