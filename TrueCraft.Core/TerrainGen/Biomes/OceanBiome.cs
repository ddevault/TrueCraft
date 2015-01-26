using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.TerrainGen.Noise;
using TrueCraft.API.World;
using TrueCraft.API;

namespace TrueCraft.Core.TerrainGen.Biomes
{
    public class OceanBiome : BiomeProvider
    {
        public override byte ID
        {
            get { return (byte)Biome.Ocean; }
        }

        public override double Temperature
        {
            get { return 0.5f; }
        }

        public override double Rainfall
        {
            get { return 0.5f; }
        }

        public override byte SurfaceBlock
        {
            get
            {
                return SandBlock.BlockID;
            }
        }

        public override byte FillerBlock
        {
            get
            {
                return DirtBlock.BlockID;
            }
        }
    }
}