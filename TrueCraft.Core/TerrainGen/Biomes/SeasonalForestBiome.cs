using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API.World;
using TrueCraft.Core.TerrainGen.Noise;

namespace TrueCraft.Core.TerrainGen.Biomes
{
    public class SeasonalForestBiome : BiomeProvider
    {
        public override byte ID
        {
            get { return (byte)Biome.SeasonalForest; }
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
                return new[] { PlantSpecies.Fern, PlantSpecies.TallGrass };
            }
        }
    }
}