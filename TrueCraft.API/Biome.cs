using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API
{
    public enum Biome
    {
        //The 13 vanilla b1.7.3 biomes as found on http://b.wiki.vg/json/b1.7
        Plains = 0,
        Desert = 1,
        Forest = 2,
        Rainforest = 3,
        SeasonalForest = 4,
        Savanna = 5,
        Shrubland = 6,
        Swampland = 7,
        Hell = 8,
        Sky = 9,//Implementation into truecraft is undetermined at this point
        Taiga = 10,
        Tundra = 11,
        IceDesert = 12,//Implementation into truecraft is undetermined at this point
        //Below are "transitional" biomes/biomes which are not in b1.7.3
        Ocean = 13,
        River = 14,//Implementation into truecraft is undetermined at this point
        Beach = 15,//Implementation into truecraft is undetermined at this point
        FrozenOcean = 16,
        FrozenRiver = 17,//Implementation into truecraft is undetermined at this point
    }
}
