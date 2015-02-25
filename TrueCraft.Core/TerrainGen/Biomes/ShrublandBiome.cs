using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.Core.TerrainGen.Noise;
using TrueCraft.API;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.TerrainGen.Biomes
{
    public class ShrublandBiome : BiomeProvider
    {
        public override byte ID
        {
            get { return (byte)Biome.Shrubland; }
        }

        public override double Temperature
        {
            get { return 0.8f; }
        }

        public override double Rainfall
        {
            get { return 0.4f; }
        }

        public override TreeSpecies[] Trees
        {
            get
            {
                return new TreeSpecies[] { TreeSpecies.Oak };
            }
        }

        public override PlantSpecies[] Plants
        {
            get
            {
                return new PlantSpecies[0];
            }
        }
    }
}