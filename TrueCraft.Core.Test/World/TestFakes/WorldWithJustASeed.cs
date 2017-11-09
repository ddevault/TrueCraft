using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fNbt;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.API.World;

namespace TrueCraft.Core.Test.World.TestFakes
{
    public class WorldWithJustASeed : IWorld
    {
        public WorldWithJustASeed(int seed)
        {
            this.Seed = seed;
        }

        public int Seed { get; set; }

        // This is everything we don't need for block decoration tests. 

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
}
