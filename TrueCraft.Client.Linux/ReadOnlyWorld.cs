using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.Core.World;
using TrueCraft.API;

namespace TrueCraft.Client.Linux
{
    public class ReadOnlyWorld
    {
        private bool UnloadChunks { get; set; }

        internal World World { get; set; }

        internal ReadOnlyWorld()
        {
            World = new World("default");
            UnloadChunks = true;
        }

        public short GetBlockID(Coordinates3D coordinates)
        {
            return World.GetBlockID(coordinates);
        }

        internal void SetBlockID(Coordinates3D coordinates, byte value)
        {
          World.SetBlockID(coordinates, value);
        }

        internal void SetMetadata(Coordinates3D coordinates, byte value)
        {
          World.SetMetadata(coordinates, value);
        }

        public byte GetMetadata(Coordinates3D coordinates)
        {
            return World.GetMetadata(coordinates);
        }

        public byte GetSkyLight(Coordinates3D coordinates)
        {
            return World.GetSkyLight(coordinates);
        }

        internal IChunk FindChunk(Coordinates2D coordinates)
        {
            return World.FindChunk(new Coordinates3D(coordinates.X, 0, coordinates.Z));
        }

        public ReadOnlyChunk GetChunk(Coordinates2D coordinates)
        {
            return new ReadOnlyChunk(World.GetChunk(coordinates));
        }

        internal void SetChunk(Coordinates2D coordinates, Chunk chunk)
        {
            World.SetChunk(coordinates, chunk);
        }

        internal void RemoveChunk(Coordinates2D coordinates)
        {
            if (UnloadChunks)
                World.UnloadChunk(coordinates);
        }
    }

    public class ReadOnlyChunk
    {
        internal IChunk Chunk { get; set; }

        internal ReadOnlyChunk(IChunk chunk)
        {
            Chunk = chunk;
        }

        public short GetBlockId(Coordinates3D coordinates)
        {
            return Chunk.GetBlockID(coordinates);
        }

        public byte GetMetadata(Coordinates3D coordinates)
        {
            return Chunk.GetMetadata(coordinates);
        }

        public byte GetSkyLight(Coordinates3D coordinates)
        {
            return Chunk.GetSkyLight(coordinates);
        }

        public byte GetBlockLight(Coordinates3D coordinates)
        {
            return Chunk.GetBlockLight(coordinates);
        }

        public int X { get { return Chunk.X; } }
        public int Z { get { return Chunk.Z; } }

        public ReadOnlyCollection<byte> Blocks { get { return Array.AsReadOnly(Chunk.Blocks); } }
        public ReadOnlyNibbleArray Metadata { get { return new ReadOnlyNibbleArray(Chunk.Metadata); } }
        public ReadOnlyNibbleArray BlockLight { get { return new ReadOnlyNibbleArray(Chunk.BlockLight); } }
        public ReadOnlyNibbleArray SkyLight { get { return new ReadOnlyNibbleArray(Chunk.SkyLight); } }
    }
}
