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
    public class DesertBiome : BiomeProvider
    {
        public override byte ID
        {
            get { return (byte)Biome.Desert; }
        }

        public override double Temperature
        {
            get { return 2.0f; }
        }

        public override double Rainfall
        {
            get { return 0.0f; }
        }

        public override TreeSpecies[] Trees
        {
            get
            {
                return new TreeSpecies[0];
            }
        }

        public override PlantSpecies[] Plants
        {
            get
            {
                return new[] { PlantSpecies.Deadbush, PlantSpecies.Cactus };
            }
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
                return SandstoneBlock.BlockID;
            }
        }

        public override int SurfaceDepth
        {
            get
            {
                return 4;
            }
        }
    }
}