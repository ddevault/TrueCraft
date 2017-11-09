using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueCraft.API.World
{
    public interface ISpatialBlockInformationProvider
    {
        int X {get;}
        int Z { get; }
        int MaxHeight { get; }

        int[] HeightMap { get; }

        byte[] Biomes { get; }


        int GetHeight(byte x, byte z);


        // This is really all one related concept. 
        byte GetBlockID(Coordinates3D coordinates);
        void SetBlockID(Coordinates3D coordinates, byte value);
        byte GetMetadata(Coordinates3D locationToCheck);
        void SetMetadata(Coordinates3D blockLocation, byte meta);

        Coordinates2D Coordinates { get; set; }
    }
}
