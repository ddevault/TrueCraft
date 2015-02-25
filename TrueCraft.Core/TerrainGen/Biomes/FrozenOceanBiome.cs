using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;

namespace TrueCraft.Core.TerrainGen.Biomes
{
    public class FrozenOceanBiome : OceanBiome
    {
        public override byte ID
        {
            get { return (byte)Biome.FrozenOcean; }
        }

        public override double Temperature
        {
            get { return 0.0f; }
        }
    }
}
