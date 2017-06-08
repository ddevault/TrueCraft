using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Logic;

namespace TrueCraft.API.World
{
    public interface IBiomeProvider
    {
        bool Spawn { get; }
        byte ID { get; }
        int Elevation { get; }
        double Temperature { get; }
        double Rainfall { get; }
        TreeSpecies[] Trees { get; }
        PlantSpecies[] Plants { get; }
        OreTypes[] Ores { get; }
        double TreeDensity { get; }
        byte WaterBlock { get; }
        byte SurfaceBlock { get; }
        byte FillerBlock { get; }
        int SurfaceDepth { get; }
        int FillerDepth { get; }
    }
}