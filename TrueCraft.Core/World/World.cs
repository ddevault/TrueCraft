using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using TrueCraft.API;
using TrueCraft.API.World;

namespace TrueCraft.Core.World
{
    public class World : IDisposable, IWorld
    {
        public const int Height = 256;

        public string Name { get; set; }
        public string BaseDirectory { get; internal set; }
        public IDictionary<Coordinates2D, IRegion> Regions { get; set; }
        public IChunkProvider ChunkProvider { get; set; }

        public World(string name)
        {
            Name = name;
            Regions = new Dictionary<Coordinates2D, IRegion>();
        }

        public World(string name, IChunkProvider chunkProvider) : this(name)
        {
            ChunkProvider = chunkProvider;
        }

        public static World LoadWorld(string baseDirectory)
        {
            if (!Directory.Exists(baseDirectory))
                throw new DirectoryNotFoundException();
            var world = new World(Path.GetFileName(baseDirectory));
            world.BaseDirectory = baseDirectory;
            return world;
        }

        /// <summary>
        /// Finds a chunk that contains the specified block coordinates.
        /// </summary>
        public IChunk FindChunk(Coordinates3D coordinates)
        {
            IChunk chunk;
            FindBlockPosition(coordinates, out chunk);
            return chunk;
        }

        public IChunk GetChunk(Coordinates2D coordinates)
        {
            int regionX = coordinates.X / Region.Width - ((coordinates.X < 0) ? 1 : 0);
            int regionZ = coordinates.Z / Region.Depth - ((coordinates.Z < 0) ? 1 : 0);

            var region = LoadOrGenerateRegion(new Coordinates2D(regionX, regionZ));
            return region.GetChunk(new Coordinates2D(coordinates.X - regionX * 32, coordinates.Z - regionZ * 32));
        }

        public void GenerateChunk(Coordinates2D coordinates)
        {
            int regionX = coordinates.X / Region.Width - ((coordinates.X < 0) ? 1 : 0);
            int regionZ = coordinates.Z / Region.Depth - ((coordinates.Z < 0) ? 1 : 0);

            var region = LoadOrGenerateRegion(new Coordinates2D(regionX, regionZ));
            region.GenerateChunk(new Coordinates2D(coordinates.X - regionX * 32, coordinates.Z - regionZ * 32));
        }

        public Chunk GetChunkWithoutGeneration(Coordinates2D coordinates)
        {
            int regionX = coordinates.X / Region.Width - ((coordinates.X < 0) ? 1 : 0);
            int regionZ = coordinates.Z / Region.Depth - ((coordinates.Z < 0) ? 1 : 0);

            var regionPosition = new Coordinates2D(regionX, regionZ);
            if (!Regions.ContainsKey(regionPosition)) return null;
            return (Chunk)((Region)Regions[regionPosition]).GetChunkWithoutGeneration(
                new Coordinates2D(coordinates.X - regionX * 32, coordinates.Z - regionZ * 32));
        }

        public void SetChunk(Coordinates2D coordinates, Chunk chunk)
        {
            int regionX = coordinates.X / Region.Width - ((coordinates.X < 0) ? 1 : 0);
            int regionZ = coordinates.Z / Region.Depth - ((coordinates.Z < 0) ? 1 : 0);

            var region = LoadOrGenerateRegion(new Coordinates2D(regionX, regionZ));
            lock (region)
            {
                chunk.IsModified = true;
                region.SetChunk(new Coordinates2D(coordinates.X - regionX * 32, coordinates.Z - regionZ * 32), chunk);
            }
        }

        public void UnloadRegion(Coordinates2D coordinates)
        {
            lock (Regions)
            {
                Regions[coordinates].Save(Path.Combine(BaseDirectory, Region.GetRegionFileName(coordinates)));
                Regions.Remove(coordinates);
            }
        }

        public void UnloadChunk(Coordinates2D coordinates)
        {
            int regionX = coordinates.X / Region.Width - ((coordinates.X < 0) ? 1 : 0);
            int regionZ = coordinates.Z / Region.Depth - ((coordinates.Z < 0) ? 1 : 0);

            var regionPosition = new Coordinates2D(regionX, regionZ);
            if (!Regions.ContainsKey(regionPosition))
                throw new ArgumentOutOfRangeException("coordinates");
            Regions[regionPosition].UnloadChunk(new Coordinates2D(coordinates.X - regionX * 32, coordinates.Z - regionZ * 32));
        }

        public short GetBlockID(Coordinates3D coordinates)
        {
            IChunk chunk;
            coordinates = FindBlockPosition(coordinates, out chunk);
            return chunk.GetBlockID(coordinates);
        }

        public byte GetMetadata(Coordinates3D coordinates)
        {
            IChunk chunk;
            coordinates = FindBlockPosition(coordinates, out chunk);
            return chunk.GetMetadata(coordinates);
        }

        public byte GetSkyLight(Coordinates3D coordinates)
        {
            IChunk chunk;
            coordinates = FindBlockPosition(coordinates, out chunk);
            return chunk.GetSkyLight(coordinates);
        }

        public byte GetBlockLight(Coordinates3D coordinates)
        {
            IChunk chunk;
            coordinates = FindBlockPosition(coordinates, out chunk);
            return chunk.GetBlockLight(coordinates);
        }

        public void SetBlockID(Coordinates3D coordinates, short value)
        {
            IChunk chunk;
            var adjustedCoordinates = FindBlockPosition(coordinates, out chunk);
            chunk.SetBlockID(adjustedCoordinates, value);
        }

        public void SetMetadata(Coordinates3D coordinates, byte value)
        {
            IChunk chunk;
            var adjustedCoordinates = FindBlockPosition(coordinates, out chunk);
            chunk.SetMetadata(adjustedCoordinates, value);
        }

        public void SetSkyLight(Coordinates3D coordinates, byte value)
        {
            IChunk chunk;
            coordinates = FindBlockPosition(coordinates, out chunk);
            chunk.SetSkyLight(coordinates, value);
        }

        public void SetBlockLight(Coordinates3D coordinates, byte value)
        {
            IChunk chunk;
            coordinates = FindBlockPosition(coordinates, out chunk);
            chunk.SetBlockLight(coordinates, value);
        }

        public void Save()
        {
            lock (Regions)
            {
                foreach (var region in Regions)
                    region.Value.Save(Path.Combine(BaseDirectory, Region.GetRegionFileName(region.Key)));
            }
        }

        public void Save(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            BaseDirectory = path;
            lock (Regions)
            {
                foreach (var region in Regions)
                    region.Value.Save(Path.Combine(BaseDirectory, Region.GetRegionFileName(region.Key)));
            }
        }

        public Coordinates3D FindBlockPosition(Coordinates3D coordinates, out IChunk chunk)
        {
            if (coordinates.Y < 0 || coordinates.Y >= Chunk.Height)
                throw new ArgumentOutOfRangeException("coordinates", "Coordinates are out of range");

            var chunkX = (int)Math.Floor((double)coordinates.X / Chunk.Width);
            var chunkZ = (int)Math.Floor((double)coordinates.Z / Chunk.Depth);

            chunk = GetChunk(new Coordinates2D(chunkX, chunkZ));
            return new Coordinates3D(
                (coordinates.X - chunkX * Chunk.Width) % Chunk.Width,
                coordinates.Y,
                (coordinates.Z - chunkZ * Chunk.Depth) % Chunk.Depth);
        }

        public bool IsValidPosition(Coordinates3D position)
        {
            return position.Y >= 0 && position.Y <= 255;
        }

        private Region LoadOrGenerateRegion(Coordinates2D coordinates)
        {
            if (Regions.ContainsKey(coordinates))
                return (Region)Regions[coordinates];
            Region region;
            if (BaseDirectory != null)
            {
                var file = Path.Combine(BaseDirectory, Region.GetRegionFileName(coordinates));
                if (File.Exists(file))
                    region = new Region(coordinates, this, file);
                else
                    region = new Region(coordinates, this);
            }
            else
                region = new Region(coordinates, this);
            lock (Regions)
                Regions[coordinates] = region;
            return region;
        }

        public void Dispose()
        {
            foreach (var region in Regions)
                region.Value.Dispose();
        }
    }
}