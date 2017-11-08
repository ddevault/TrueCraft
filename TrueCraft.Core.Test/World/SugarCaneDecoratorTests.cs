using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using fNbt;
using NUnit.Framework;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.API.World;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.Core.TerrainGen;
using TrueCraft.Core.TerrainGen.Biomes;
using TrueCraft.Core.TerrainGen.Decorators;
using TrueCraft.Core.TerrainGen.Noise;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Test.World
{
    public class WorldWithJustASeed : IWorld
    {
        public WorldWithJustASeed(int seed)
        {
            this.Seed = seed;
        }

        public IEnumerator<IChunk> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string Name { get; set; }
        public IBlockRepository BlockRepository { get; set; }
        public int Seed { get; set; }
        public IBiomeMap BiomeDiagram { get; set; }
        public IChunkProvider ChunkProvider { get; set; }
        public Coordinates3D SpawnPoint { get; set; }
        public long Time { get; set; }
        public event EventHandler<BlockChangeEventArgs> BlockChanged;
        public event EventHandler<ChunkLoadedEventArgs> ChunkGenerated;
        public event EventHandler<ChunkLoadedEventArgs> ChunkLoaded;
        public IChunk GetChunk(Coordinates2D coordinates, bool generate = true)
        {
            throw new NotImplementedException();
        }

        public IChunk FindChunk(Coordinates3D coordinates, bool generate = true)
        {
            throw new NotImplementedException();
        }

        public byte GetBlockID(Coordinates3D coordinates)
        {
            throw new NotImplementedException();
        }

        public byte GetMetadata(Coordinates3D coordinates)
        {
            throw new NotImplementedException();
        }

        public byte GetBlockLight(Coordinates3D coordinates)
        {
            throw new NotImplementedException();
        }

        public byte GetSkyLight(Coordinates3D coordinates)
        {
            throw new NotImplementedException();
        }

        public Coordinates3D FindBlockPosition(Coordinates3D coordinates, out IChunk chunk, bool generate = true)
        {
            throw new NotImplementedException();
        }

        public NbtCompound GetTileEntity(Coordinates3D coordinates)
        {
            throw new NotImplementedException();
        }

        public BlockDescriptor GetBlockData(Coordinates3D coordinates)
        {
            throw new NotImplementedException();
        }

        public void SetBlockData(Coordinates3D coordinates, BlockDescriptor block)
        {
            throw new NotImplementedException();
        }

        public void SetBlockID(Coordinates3D coordinates, byte value)
        {
            throw new NotImplementedException();
        }

        public void SetMetadata(Coordinates3D coordinates, byte value)
        {
            throw new NotImplementedException();
        }

        public void SetSkyLight(Coordinates3D coordinates, byte value)
        {
            throw new NotImplementedException();
        }

        public void SetBlockLight(Coordinates3D coordinates, byte value)
        {
            throw new NotImplementedException();
        }

        public void SetTileEntity(Coordinates3D coordinates, NbtCompound value)
        {
            throw new NotImplementedException();
        }

        public bool IsValidPosition(Coordinates3D position)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Save(string path)
        {
            throw new NotImplementedException();
        }
    }


    public class NoiseAlwaysGrowsSugarCaneInTestBounds : NoiseGen
    {
        public override double Value2D(double x, double y)
        {
            return x > 5 ? 0 : (y > 5 ? 0 : 1);
        }

        public override double Value3D(double x, double y, double z)
        {
            double value2d = Value2D(x, y);

            return 1;
        }
    }

    public class PrimeSugarCaneGrowingSeasonChunk : IChunk
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public event EventHandler Disposed;
        public int X => 6;
        public int Z => 6;
        public int MaxHeight => 7;
        public Coordinates2D Coordinates { get; set; }
        public bool IsModified { get; set; }
        public bool LightPopulated { get; set; }
        public int[] HeightMap => Enumerable.Repeat(1, Chunk.Width * Chunk.Height).ToArray<int>();
        public byte[] Biomes => Enumerable.Repeat(new SwamplandBiome().ID, Chunk.Width*Chunk.Height).ToArray<byte>();

        public DateTime LastAccessed { get; set; }
        public byte[] Data { get; }
        public bool TerrainPopulated { get; set; }
        public Dictionary<Coordinates3D, NbtCompound> TileEntities { get; set; }
        public NibbleSlice Metadata { get; }
        public NibbleSlice BlockLight { get; }
        public NibbleSlice SkyLight { get; }
        public IRegion ParentRegion { get; set; }
        public int GetHeight(byte x, byte z)
        {
            // Pretty sure this is always one since we have a flat surface 
            // (Unless water heights are reported differently)
            return 1;
        }

        public void UpdateHeightMap()
        {
            throw new NotImplementedException();
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

            // What's in that block? 
            // Back at me. 
            // I have it, it's a block that doesn't matter because it's beyond our test grid.  
            // Look again
            // The block is now diamonds. 
            // Anything is possible when you're in a test mock
            // I'm on a pig. 
            return DiamondBlock.BlockID;
        }


        private static Dictionary<Coordinates3D, byte> createStartingBlockDictionary()
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
                        blockDictionary.Add(new Coordinates3D(x,y,z), blockToStore);
                    }
                }
            }

            return blockDictionary;
        }

        private Dictionary<Coordinates3D, byte> blockDictionary = createStartingBlockDictionary();


        public byte GetBlockID(Coordinates3D coordinates)
        {
            if(blockDictionary.ContainsKey(coordinates))
            {
                return blockDictionary[coordinates];
            }
            return AirBlock.BlockID;
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

        public void SetBlockID(Coordinates3D coordinates, byte value)
        {
            blockDictionary[coordinates] = value;
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
    }


    [TestFixture]
    public class SugarCaneDecoratorTests
    {
        bool ContainsSugarCane(IChunk aChunk, int x, int z)
        {
            return CountBlockInColumn(aChunk, x, z, SugarcaneBlock.BlockID) > 0;
        }
        
        static int CountBlockInColumn(IChunk aChunk, int x, int z, byte blockId)
        {
            int counter = 0;

            for (int y = 0; y < 7; y++)
            {
                byte block = aChunk.GetBlockID(new Coordinates3D(x:x, y:y, z:z));
                if (block == blockId)
                {
                    counter++;
                }
            }
            return counter;
        }

        
        [Test]
        public void TestBasicDecorator()
        {
            var decorator = new SugarCaneDecorator(new NoiseAlwaysGrowsSugarCaneInTestBounds());
            IWorld aWorld = new WorldWithJustASeed(9001);
            IChunk aChunk = new PrimeSugarCaneGrowingSeasonChunk();
            IBiomeRepository aBiomeRepository = new BiomeRepository();
            aBiomeRepository.RegisterBiomeProvider(new SwamplandBiome());
            
            decorator.Decorate(aWorld, aChunk, aBiomeRepository);

            AssertChunkHasNoSugarCaneInColumnsWhereItShouldNot(aChunk);
        }

        private void AssertChunkHasNoSugarCaneInColumnsWhereItShouldNot(IChunk aChunk)
        {
            for (int x = 0; x < 6; x++)
            {
                for (int z = 0; z < 6; z++)
                {
                    Coordinates2D coord = new Coordinates2D(x, z);
                    if (PrimeSugarCaneGrowingSeasonChunk.PointsWithoutAnySugarcane().Contains(coord))
                    {
                        Assert.AreEqual(0, CountBlockInColumn(aChunk, x, z, SugarcaneBlock.BlockID), string.Format("Sugarcane in column ({0},{1})", x,z));
                    }
                }
            }
        }
    }
}