using System;
using System.Collections.Generic;
using System.Linq;
using fNbt;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.TerrainGen.Biomes;
using TrueCraft.Core.World;

public class PrimeSugarCaneGrowingSeasonChunk : IChunk
{
    public int X => 6;
    public int Z => 6;
    public int MaxHeight => 7;

    public int[] HeightMap => Enumerable.Repeat(1, Chunk.Width * Chunk.Height).ToArray<int>();
    public byte[] Biomes => Enumerable.Repeat(new SwamplandBiome().ID, Chunk.Width * Chunk.Height).ToArray<byte>();

    public int GetHeight(byte x, byte z)
    {
        // Pretty sure this is always one since we have a flat surface 
        // (Unless water heights are reported differently)
        return 1;
    }

    private Dictionary<Coordinates3D, byte> blockDictionary = createStartingBlockDictionary();

    public byte GetBlockID(Coordinates3D coordinates)
    {
        if (blockDictionary.ContainsKey(coordinates))
        {
            return blockDictionary[coordinates];
        }
        return AirBlock.BlockID;
    }

    public void SetBlockID(Coordinates3D coordinates, byte value)
    {
        blockDictionary[coordinates] = value;
    }

    public static int CountBlockInColumn(Dictionary<Coordinates3D, byte> aChunk, int x, int z, byte blockId)
    {
        int counter = 0;

        for (int y = 0; y < 7; y++)
        {
            byte block = aChunk[new Coordinates3D(x: x, y: y, z: z)];
            if (block == blockId)
            {
                counter++;
            }
        }
        return counter;
    }

    private static byte MapXZToTestBlock(int X, int Z)
    {
        // Sand on the outer left hand side
        if (X == 0)
        {
            return GrassBlock.BlockID;
        }

        // Sand on the outer right hand side
        if (X == 5)
        {
            return SandBlock.BlockID;
        }

        if (Z == 0)
            return WaterBlock.BlockID;

        if (Z == 3)
            return StationaryWaterBlock.BlockID;

        switch (X)
        {
            case 1:
                return GrassBlock.BlockID;
            case 2:
                return DirtBlock.BlockID;
            case 3:
                return SandBlock.BlockID;
            case 4:
                return StoneBlock.BlockID;
        }

        return DiamondBlock.BlockID;
    }


    public static Dictionary<Coordinates3D, byte> createStartingBlockDictionary()
    {
        int xBounds = 6;
        int yBounds = 7;
        int zBounds = 6;

        Dictionary<Coordinates3D, byte> blockDictionary = new Dictionary<Coordinates3D, byte>();

        for (int x = 0; x < xBounds; x++)
        {
            for (int z = 0; z < zBounds; z++)
            {
                byte row1Blocks = MapXZToTestBlock(x, z);
                for (int y = 0; y < yBounds; y++)
                {
                    byte blockToStore = AirBlock.BlockID;
                    if (y == 0)
                    {
                        // Dirt on the lowest layer
                        blockToStore = DirtBlock.BlockID;
                    }
                    if (y == 1)
                    {
                        blockToStore = row1Blocks;
                    }
                    blockDictionary.Add(new Coordinates3D(x, y, z), blockToStore);
                }
            }
        }

        return blockDictionary;
    }




    public static HashSet<Coordinates2D> PointsWithoutAnySugarcane()
    {
        var result = new HashSet<Coordinates2D>();
        var startingDictionary = createStartingBlockDictionary();
        foreach (var block in createStartingBlockDictionary())
        {
            var placesToGrow = CountBlockInColumn(startingDictionary, block.Key.X, block.Key.Z, SandBlock.BlockID);
            placesToGrow += CountBlockInColumn(startingDictionary, block.Key.X, block.Key.Z, GrassBlock.BlockID);

            if (placesToGrow == 0)
            {
                result.Add(new Coordinates2D(block.Key.X, block.Key.Z));
            }
        }

        result.Add(new Coordinates2D(0, 0));
        result.Add(new Coordinates2D(0, 1));
        result.Add(new Coordinates2D(0, 0));
        result.Add(new Coordinates2D(0, 2));
        result.Add(new Coordinates2D(0, 4));
        result.Add(new Coordinates2D(0, 5));


        for (int i = 1; i < 5; i++)
        {
            // Top row of the test data is all landlocked
            result.Add(new Coordinates2D(i, 5));
        }

        result.Add(new Coordinates2D(5, 5));
        result.Add(new Coordinates2D(5, 4));
        result.Add(new Coordinates2D(5, 2));
        result.Add(new Coordinates2D(5, 1));

        return result;
    }


    // Extra IChunk interface things we don't need for block decoration tests below. 
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public event EventHandler Disposed;
    public Coordinates2D Coordinates { get; set; }
    public bool IsModified { get; set; }
    public bool LightPopulated { get; set; }

    public DateTime LastAccessed { get; set; }
    public byte[] Data { get; }
    public bool TerrainPopulated { get; set; }
    public Dictionary<Coordinates3D, NbtCompound> TileEntities { get; set; }
    public NibbleSlice Metadata { get; }
    public NibbleSlice BlockLight { get; }
    public NibbleSlice SkyLight { get; }
    public IRegion ParentRegion { get; set; }

    public void UpdateHeightMap()
    {
        throw new NotImplementedException();
    }

    public byte GetMetadata(Coordinates3D coordinates)
    {
        throw new NotImplementedException();
    }

    public byte GetSkyLight(Coordinates3D coordinates)
    {
        throw new NotImplementedException();
    }

    public byte GetBlockLight(Coordinates3D coordinates)
    {
        throw new NotImplementedException();
    }


    public void SetMetadata(Coordinates3D coordinates, byte value)
    {
        // 
    }

    public void SetSkyLight(Coordinates3D coordinates, byte value)
    {
        throw new NotImplementedException();
    }

    public void SetBlockLight(Coordinates3D coordinates, byte value)
    {
        throw new NotImplementedException();
    }

    public NbtCompound GetTileEntity(Coordinates3D coordinates)
    {
        throw new NotImplementedException();
    }

    public void SetTileEntity(Coordinates3D coordinates, NbtCompound value)
    {
        throw new NotImplementedException();
    }


}
