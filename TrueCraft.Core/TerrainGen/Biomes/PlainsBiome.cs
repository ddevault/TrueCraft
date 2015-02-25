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
    public class PlainsBiome : BiomeProvider
    {
        public override byte ID
        {
            get { return (byte)Biome.Plains; }
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
    }
}