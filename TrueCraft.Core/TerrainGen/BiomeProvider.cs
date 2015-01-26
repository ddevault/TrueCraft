using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.TerrainGen
{
    public abstract class BiomeProvider : IBiomeProvider
    {
        /// <summary>
        /// The ID of the biome.
        /// </summary>
        public abstract byte ID { get; }

        /// <summary>
        /// The base temperature of the biome.
        /// </summary>
        public abstract float Temperature { get; }

        /// <summary>
        /// The main surface block used for the terrain of the biome.
        /// Note: This field may be removed in the future.
        /// </summary>
        public virtual byte SurfaceBlock { get { return GrassBlock.BlockID; } }

        /// <summary>
        /// The main "filler" block found under the surface block in the terrain of the biome.
        /// Note: This field may be removed in the future.
        /// </summary>
        public virtual byte FillerBlock { get { return StoneBlock.BlockID; } }
    }
}