using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fNbt;
using Ionic.Zlib;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.Core.Networking;

namespace TrueCraft.Core.World
{
    /// <summary>
    /// Represents a 32x32 area of <see cref="Chunk"/> objects.
    /// Not all of these chunks are represented at any given time, and
    /// will be loaded from disk or generated when the need arises.
    /// </summary>
    public class Region : IDisposable, IRegion
    {
        // In chunks
        public const int Width = 32, Depth = 32;

        private ConcurrentDictionary<Coordinates2D, IChunk> _Chunks { get; set; }
        /// <summary>
        /// The currently loaded chunk list.
        /// </summary>
        public IDictionary<Coordinates2D, IChunk> Chunks { get { return _Chunks; } }
        /// <summary>
        /// The location of this region in the overworld.
        /// </summary>
        public Coordinates2D Position { get; set; }

        public World World { get; set; }

        private HashSet<Coordinates2D> DirtyChunks { get; set; } = new HashSet<Coordinates2D>();
        private Stream regionFile { get; set; }
        private object streamLock = new object();

        /// <summary>
        /// Creates a new Region for server-side use at the given position using
        /// the provided terrain generator.
        /// </summary>
        public Region(Coordinates2D position, World world)
        {
            _Chunks = new ConcurrentDictionary<Coordinates2D, IChunk>();
            Position = position;
            World = world;
        }

        /// <summary>
        /// Creates a region from the given region file.
        /// </summary>
        public Region(Coordinates2D position, World world, string file) : this(position, world)
        {
            if (File.Exists(file))
            {
                regionFile = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                regionFile.Read(HeaderCache, 0, 8192);
            }
            else
            {
                regionFile = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                CreateRegionHeader();
            }
        }

        public void DamageChunk(Coordinates2D coords)
        {
            int x = coords.X / Region.Width - ((coords.X < 0) ? 1 : 0);
            int z = coords.Z / Region.Depth - ((coords.Z < 0) ? 1 : 0);
            DirtyChunks.Add(new Coordinates2D(coords.X - x * 32, coords.Z - z * 32));
        }

        /// <summary>
        /// Retrieves the requested chunk from the region, or
        /// generates it if a world generator is provided.
        /// </summary>
        /// <param name="position">The position of the requested local chunk coordinates.</param>
        public IChunk GetChunk(Coordinates2D position, bool generate = true)
        {
            if (!Chunks.ContainsKey(position))
            {
                if (regionFile != null)
                {
                    // Search the stream for that region
                    var chunkData = GetChunkFromTable(position);
                    if (chunkData == null)
                    {
                        if (World.ChunkProvider == null)
                            throw new ArgumentException("The requested chunk is not loaded.", "position");
                        if (generate)
                            GenerateChunk(position);
                        else
                            return null;
                        return Chunks[position];
                    }
                    lock (streamLock)
                    {
                        regionFile.Seek(chunkData.Item1, SeekOrigin.Begin);
                        /*int length = */
                        new MinecraftStream(regionFile).ReadInt32(); // TODO: Avoid making new objects here, and in the WriteInt32
                        int compressionMode = regionFile.ReadByte();
                        switch (compressionMode)
                        {
                            case 1: // gzip
                                throw new NotImplementedException("gzipped chunks are not implemented");
                            case 2: // zlib
                                var nbt = new NbtFile();
                                nbt.LoadFromStream(regionFile, NbtCompression.ZLib, null);
                                var chunk = Chunk.FromNbt(nbt);
                                chunk.ParentRegion = this;
                                Chunks.Add(position, chunk);
                                World.OnChunkLoaded(new ChunkLoadedEventArgs(chunk));
                                break;
                            default:
                                throw new InvalidDataException("Invalid compression scheme provided by region file.");
                        }
                    }
                }
                else if (World.ChunkProvider == null)
                    throw new ArgumentException("The requested chunk is not loaded.", nameof(position));
                else
                {
                    if (generate)
                        GenerateChunk(position);
                    else
                        return null;
                }
            }
            return Chunks[position];
        }

        public void GenerateChunk(Coordinates2D position)
        {
            var globalPosition = (Position * new Coordinates2D(Width, Depth)) + position;
            var chunk = World.ChunkProvider.GenerateChunk(World, globalPosition);
            chunk.IsModified = true;
            chunk.Coordinates = globalPosition;
            chunk.ParentRegion = this;
            DirtyChunks.Add(position);
            Chunks[position] = chunk;
            World.OnChunkGenerated(new ChunkLoadedEventArgs(chunk));
        }

        /// <summary>
        /// Sets the chunk at the specified local position to the given value.
        /// </summary>
        public void SetChunk(Coordinates2D position, IChunk chunk)
        {
            if (!Chunks.ContainsKey(position))
                Chunks.Add(position, chunk);
            chunk.IsModified = true;
            DirtyChunks.Add(position);
            chunk.ParentRegion = this;
            Chunks[position] = chunk;
        }

        /// <summary>
        /// Saves this region to the specified file.
        /// </summary>
        public void Save(string file)
        {
            if(File.Exists(file))
                regionFile = regionFile ?? File.Open(file, FileMode.OpenOrCreate);
            else
            {
                regionFile = regionFile ?? File.Open(file, FileMode.OpenOrCreate);
                CreateRegionHeader();
            }
            Save();
        }

        /// <summary>
        /// Saves this region to the open region file.
        /// </summary>
        public void Save()
        {
            lock (streamLock)
            {
                var toRemove = new List<Coordinates2D>();
                var chunks = DirtyChunks.ToList();
                DirtyChunks.Clear();
                foreach (var coords in chunks)
                {
                    var chunk = GetChunk(coords, generate: false);
                    if (chunk.IsModified)
                    {
                        var data = ((Chunk)chunk).ToNbt();
                        byte[] raw = data.SaveToBuffer(NbtCompression.ZLib);

                        var header = GetChunkFromTable(coords);
                        if (header == null || header.Item2 > raw.Length)
                            header = AllocateNewChunks(coords, raw.Length);

                        regionFile.Seek(header.Item1, SeekOrigin.Begin);
                        new MinecraftStream(regionFile).WriteInt32(raw.Length);
                        regionFile.WriteByte(2); // Compressed with zlib
                        regionFile.Write(raw, 0, raw.Length);

                        chunk.IsModified = false;
                    }
                    if ((DateTime.UtcNow - chunk.LastAccessed).TotalMinutes > 5)
                        toRemove.Add(coords);
                }
                regionFile.Flush();
                // Unload idle chunks
                foreach (var chunk in toRemove)
                {
                    var c = Chunks[chunk];
                    Chunks.Remove(chunk);
                    c.Dispose();
                }
            }
        }

        #region Stream Helpers

        private const int ChunkSizeMultiplier = 4096;
        private byte[] HeaderCache = new byte[8192];
        
        private Tuple<int, int> GetChunkFromTable(Coordinates2D position) // <offset, length>
        {
            int tableOffset = ((position.X % Width) + (position.Z % Depth) * Width) * 4;
            byte[] offsetBuffer = new byte[4];
            Buffer.BlockCopy(HeaderCache, tableOffset, offsetBuffer, 0, 3);
            Array.Reverse(offsetBuffer);
            int length = HeaderCache[tableOffset + 3];
            int offset = BitConverter.ToInt32(offsetBuffer, 0) << 4;
            if (offset == 0 || length == 0)
                return null;
            return new Tuple<int, int>(offset,
                length * ChunkSizeMultiplier);
        }

        private void CreateRegionHeader()
        {
            HeaderCache = new byte[8192];
            regionFile.Write(HeaderCache, 0, 8192);
            regionFile.Flush();
        }

        private Tuple<int, int> AllocateNewChunks(Coordinates2D position, int length)
        {
            // Expand region file
            regionFile.Seek(0, SeekOrigin.End);
            int dataOffset = (int)regionFile.Position;

            length /= ChunkSizeMultiplier;
            length++;
            regionFile.Write(new byte[length * ChunkSizeMultiplier], 0, length * ChunkSizeMultiplier);

            // Write table entry
            int tableOffset = ((position.X % Width) + (position.Z % Depth) * Width) * 4;
            regionFile.Seek(tableOffset, SeekOrigin.Begin);

            byte[] entry = BitConverter.GetBytes(dataOffset >> 4);
            entry[0] = (byte)length;
            Array.Reverse(entry);
            regionFile.Write(entry, 0, entry.Length);
            Buffer.BlockCopy(entry, 0, HeaderCache, tableOffset, 4);

            return new Tuple<int, int>(dataOffset, length * ChunkSizeMultiplier);
        }

        #endregion

        public static string GetRegionFileName(Coordinates2D position)
        {
            return string.Format("r.{0}.{1}.mca", position.X, position.Z);
        }

        public void UnloadChunk(Coordinates2D position)
        {
            Chunks.Remove(position);
        }

        public void Dispose()
        {
            if (regionFile == null)
                return;
            lock (streamLock)
            {
                regionFile.Flush();
                regionFile.Close();
            }
        }
    }
}
