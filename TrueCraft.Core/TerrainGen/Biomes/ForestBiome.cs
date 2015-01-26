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
    public class ForestBiome : BiomeProvider
    {
        public override byte ID
        {
            get { return (byte)Biome.Forest; }
        }

        public override double Temperature
        {
            get { return 0.7f; }
        }

        public override double Rainfall
        {
            get { return 0.8f; }
        }

        public override PlantSpecies[] Plants
        {
            get
            {
                return new PlantSpecies[] { PlantSpecies.TallGrass };
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