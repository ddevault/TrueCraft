using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.Core.TerrainGen.Noise;

namespace TrueCraft.Core.World
{
    public class BiomeMap : IBiomeMap
    {
        public IList<BiomeCell> BiomeCells { get; private set; }
        Perlin TempNoise = new Perlin();
        Perlin RainNoise = new Perlin();
        public BiomeMap()
        {
            BiomeCells = new List<BiomeCell>();
            TempNoise.Persistance = 1.45;
            TempNoise.Frequency = 0.015;
            TempNoise.Amplitude = 5;
            TempNoise.Octaves = 2;
            TempNoise.Lacunarity = 1.3;
            RainNoise.Frequency = 0.03;
            RainNoise.Octaves = 2;
            RainNoise.Amplitude = 5;
            RainNoise.Lacunarity = 1.7;
        }

        public void AddCell(BiomeCell Cell)
        {
            BiomeCells.Add(Cell);
        }

        public byte GetBiome(Coordinates2D Location)
        {
            byte BiomeID = (ClosestCell(Location) != null) ? ClosestCell(Location).BiomeID : (byte)Biome.Plains;
            return BiomeID;
        }

        public byte GenerateBiome(int Seed, IBiomeRepository Biomes, Coordinates2D Location)
        {
            TempNoise.Seed = Seed;
            RainNoise.Seed = Seed;

            double Temperature = Math.Abs(TempNoise.Value2D(Location.X, Location.Z));
            double Rainfall = Math.Abs(RainNoise.Value2D(Location.X, Location.Z));
            byte ID = Biomes.GetBiome(Temperature, Rainfall).ID;
            return ID;
        }

        /*
         * The closest biome cell to the specified location(uses the Chebyshev distance function).
         */
        public BiomeCell ClosestCell(Coordinates2D Location)
        {
            BiomeCell Cell = null;
            var DistanceValue = double.MaxValue;
            foreach (BiomeCell C in BiomeCells)
            {
                var DistanceTo = Distance(Location, C.CellPoint);
                if (DistanceTo < DistanceValue)
                {
                    DistanceValue = DistanceTo;
                    Cell = C;
                }
            }
            return Cell;
        }

        /*
         * The distance to the closest biome cell point to the specified location(uses the Chebyshev distance function).
         */
        public double ClosestCellPoint(Coordinates2D Location)
        {
            var DistanceValue = double.MaxValue;
            foreach (BiomeCell C in BiomeCells)
            {
                var DistanceTo = Distance(Location, C.CellPoint);
                if (DistanceTo < DistanceValue)
                {
                    DistanceValue = DistanceTo;
                }
            }
            return DistanceValue;
        }

        public double Distance(Coordinates2D Location1, Coordinates2D Location2)
        {
            Coordinates2D Difference = Location1 - Location2;
            return Math.Max(Math.Abs(Difference.X), Math.Abs(Difference.Z));
        }
    }
}