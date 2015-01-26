using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API.World
{
    public class BiomeCell
    {
        public byte BiomeID;
        public Coordinates2D CellPoint;

        public BiomeCell(byte BiomeID, Coordinates2D CellPoint)
        {
            this.BiomeID = BiomeID;
            this.CellPoint = CellPoint;
        }
    }

    public interface IBiomeMap
    {
        IList<BiomeCell> BiomeCells { get; }
        void AddCell(BiomeCell Cell);
        byte GetBiome(Coordinates2D Location);
        byte GenerateBiome(int Seed, IBiomeRepository Biomes, Coordinates2D Location);
        BiomeCell ClosestCell(Coordinates2D Location);
        double ClosestCellPoint(Coordinates2D Location);
    }
}