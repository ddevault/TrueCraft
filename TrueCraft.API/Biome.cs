using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API
{
    /// <summary>
    /// Enumerates the biomes in TrueCraft.
    /// </summary>
    /// <remarks>
    /// The 13 vanilla b1.7.3 biomes as found on http://b.wiki.vg/json/b1.7
    /// </remarks>
    public enum Biome
    {
        /// <summary>
        /// A plains biome.
        /// </summary>
        Plains = 0,

        /// <summary>
        /// A desert biome.
        /// </summary>
        Desert = 1,

        /// <summary>
        /// A forest biome.
        /// </summary>
        Forest = 2,

        /// <summary>
        /// A rainforest biome.
        /// </summary>
        Rainforest = 3,
        
        /// <summary>
        /// A seasonal forest biome.
        /// </summary>
        SeasonalForest = 4,

        /// <summary>
        /// A savanna biome.
        /// </summary>
        Savanna = 5,

        /// <summary>
        /// A shrubland biome.
        /// </summary>
        Shrubland = 6,

        /// <summary>
        /// A swampland biome.
        /// </summary>
        Swampland = 7,

        /// <summary>
        /// A Nether biome.
        /// </summary>
        Hell = 8,

        /// <summary>
        /// An End biome.
        /// </summary>
        /// <remarks>
        /// Implementation into TrueCraft is undetermined at this point
        /// </remarks>
        Sky = 9,

        /// <summary>
        /// A taiga biome.
        /// </summary>
        Taiga = 10,

        /// <summary>
        /// A tundra biome.
        /// </summary>
        Tundra = 11,

        /// <summary>
        /// An ice desert biome.
        /// </summary>
        /// <remarks>
        /// Implementation into TrueCraft is undetermined at this point
        /// </remarks>
        IceDesert = 12,
        
        //Below are "transitional" biomes/biomes which are not in b1.7.3

        /// <summary>
        /// An ocean biome.
        /// </summary>
        Ocean = 13,

        /// <summary>
        /// A river biome.
        /// </summary>
        /// <remarks>
        /// Implementation into TrueCraft is undetermined at this point
        /// </remarks>
        River = 14,

        /// <summary>
        /// A beach biome.
        /// </summary>
        /// <remarks>
        /// Implementation into TrueCraft is undetermined at this point
        /// </remarks>
        Beach = 15,

        /// <summary>
        /// A frozen ocean biome.
        /// </summary>
        FrozenOcean = 16,

        /// <summary>
        /// A frozen river biome.
        /// </summary>
        /// <remarks>
        /// Implementation into TrueCraft is undetermined at this point
        /// </remarks>
        FrozenRiver = 17,
    }
}
