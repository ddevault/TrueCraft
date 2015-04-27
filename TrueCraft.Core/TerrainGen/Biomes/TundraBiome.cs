using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.Core.TerrainGen.Noise;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API.World;
using TrueCraft.API;

namespace TrueCraft.Core.TerrainGen.Biomes
{
    public class TundraBiome : BiomeProvider
    {
        public override byte ID
        {
            get { return (byte)Biome.Tundra; }
        }

        public override double Temperature
        {
            get { return 0.1f; }
        }

        public override double Rainfall
        {
            get { return 0.7f; }
        }

        public override TreeSpecies[] Trees
        {
            get
            {
                return new[] { TreeSpecies.Spruce };
            }
        }

        public override PlantSpecies[] Plants
        {
            get
            {
                return new PlantSpecies[0];
            }
        }

        public override double TreeDensity
        {
            get
            {
                return 50;
            }
        }

        public override byte SurfaceBlock
        {
            get
            {
                return GrassBlock.BlockID;
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