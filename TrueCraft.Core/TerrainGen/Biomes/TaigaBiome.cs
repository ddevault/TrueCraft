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
    public class TaigaBiome : BiomeProvider
    {
        public override byte ID
        {
            get { return (byte)Biome.Taiga; }
        }

        public override double Temperature
        {
            get { return 0.0f; }
        }

        public override double Rainfall
        {
            get { return 0.0f; }
        }

        public override TreeSpecies[] Trees
        {
            get
            {
                return new[] { TreeSpecies.Spruce };
            }
        }

        public override double TreeDensity
        {
            get
            {
                return 5;
            }
        }
    }
}