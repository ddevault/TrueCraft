using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.API.Logic;
using fNbt;

namespace TrueCraft.Core.World
{
    public class World : IDisposable, IWorld
    {
        public static readonly int Height = 128;

        public string Name { get; set; }
        public int Seed { get; set; }
        private Coordinates3D? _SpawnPoint;
        public Coordinates3D SpawnPoint
        {
            get
            {
                if (_SpawnPoint == null)
                    _SpawnPoint = ChunkProvider.GetSpawn(this);
                return _SpawnPoint.Value;
            }
            set
            {
                _SpawnPoint = value;
            }
        }
        public string BaseDirectory { get; internal set; }
        public IDictionary<Coordinates2D, IRegion> Regions { get; set; }
        public IBiomeMap BiomeDiagram { get; set; }
        public IChunkProvider ChunkProvider { get; set; }
        public IBlockRepository BlockRepository { get; set; }
        public DateTime BaseTime { get; set; }

        public long Time
        {
            get
            {
                return (long)((DateTime.Now - BaseTime).TotalSeconds * 20) % 24000;
            }
            set
            {
                BaseTime = DateTime.Now.AddSeconds(-value/20);
            }
        }

        public event EventHandler<BlockChangeEventArgs> BlockChanged;

        public World()
        {
            Regions = new Dictionary<Coordinates2D, IRegion>();
            BaseTime = DateTime.Now;
        }

        public World(string name) : this()
        {
            Name = name;
            Seed = new Random().Next();
            BiomeDiagram = new BiomeMap(Seed);
        }

        public World(string name, IChunkProvider chunkProvider) : this(name)
        {
            ChunkProvider = chunkProvider;
        }

        public World(string name, int seed, IChunkProvider chunkProvider) : this(name, chunkProvider)
        {
            Seed = seed;
            BiomeDiagram = new BiomeMap(Seed);
        }

        public static World LoadWorld(string baseDirectory)
        {
            if (!Directory.Exists(baseDirectory))
                throw new DirectoryNotFoundException();
            var world = new World(Path.GetFileName(baseDirectory));
            world.BaseDirectory = baseDirectory;
            if (File.Exists(Path.Combine(baseDirectory, "manifest.nbt")))
            {
                var file = new NbtFile(Path.Combine(baseDirectory, "manifest.nbt"));
                world.SpawnPoint = new Coordinates3D(file.RootTag["SpawnPoint"]["X"].IntValue,
                    file.RootTag["SpawnPoint"]["Y"].IntValue,
                    file.RootTag["SpawnPoint"]["Z"].IntValue);
                world.Seed = file.RootTag["Seed"].IntValue;
                var providerName = file.RootTag["ChunkProvider"].StringValue;
                var provider = (IChunkProvider)Activator.CreateInstance(Type.GetType(providerName), world);
                world.ChunkProvider = provider;
            }
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

        public byte GetBlockID(Coordinates3D coordinates)
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

        public NbtCompound GetTileEntity(Coordinates3D coordinates)
        {
            IChunk chunk;
            coordinates = FindBlockPosition(coordinates, out chunk);
            return chunk.GetTileEntity(coordinates);
        }

        public BlockDescriptor GetBlockData(Coordinates3D coordinates)
        {
            IChunk chunk;
            var adjustedCoordinates = FindBlockPosition(coordinates, out chunk);
            return GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates);
        }

        public void SetBlockData(Coordinates3D coordinates, BlockDescriptor descriptor)
        {
            // TODO: Figure out the best way to handle light in this scenario
            IChunk chunk;
            var adjustedCoordinates = FindBlockPosition(coordinates, out chunk);
            var old = GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates);
            chunk.SetBlockID(adjustedCoordinates, descriptor.ID);
            chunk.SetMetadata(adjustedCoordinates,descriptor.Metadata);
            if (BlockChanged != null)
                BlockChanged(this, new BlockChangeEventArgs(coordinates, old, GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates)));
        }

        private BlockDescriptor GetBlockDataFromChunk(Coordinates3D adjustedCoordinates, IChunk chunk, Coordinates3D coordinates)
        {
            return new BlockDescriptor
            {
                ID = chunk.GetBlockID(adjustedCoordinates),
                Metadata = chunk.GetMetadata(adjustedCoordinates),
                BlockLight = chunk.GetBlockLight(adjustedCoordinates),
                SkyLight = chunk.GetSkyLight(adjustedCoordinates),
                Coordinates = coordinates
            };
        }

        public void SetBlockID(Coordinates3D coordinates, byte value)
        {
            IChunk chunk;
            var adjustedCoordinates = FindBlockPosition(coordinates, out chunk);
            BlockDescriptor old = new BlockDescriptor();
            if (BlockChanged != null)
                old = GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates);
            chunk.SetBlockID(adjustedCoordinates, value);
            if (BlockChanged != null)
                BlockChanged(this, new BlockChangeEventArgs(coordinates, old, GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates)));
        }

        public void SetMetadata(Coordinates3D coordinates, byte value)
        {
            IChunk chunk;
            var adjustedCoordinates = FindBlockPosition(coordinates, out chunk);
            BlockDescriptor old = new BlockDescriptor();
            if (BlockChanged != null)
                old = GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates);
            chunk.SetMetadata(adjustedCoordinates, value);
            if (BlockChanged != null)
                BlockChanged(this, new BlockChangeEventArgs(coordinates, old, GetBlockDataFromChunk(adjustedCoordinates, chunk, coordinates)));
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

        public void SetTileEntity(Coordinates3D coordinates, NbtCompound value)
        {
            IChunk chunk;
            coordinates = FindBlockPosition(coordinates, out chunk);
            chunk.SetTileEntity(coordinates, value);
        }

        public void Save()
        {
            lock (Regions)
            {
                foreach (var region in Regions)
                    region.Value.Save(Path.Combine(BaseDirectory, Region.GetRegionFileName(region.Key)));
            }
            var file = new NbtFile();
            file.RootTag.Add(new NbtCompound("SpawnPoint", new[]
            {
                new NbtInt("X", this.SpawnPoint.X),
                new NbtInt("Y", this.SpawnPoint.Y),
                new NbtInt("Z", this.SpawnPoint.Z)
            }));
            file.RootTag.Add(new NbtInt("Seed", this.Seed));
            file.RootTag.Add(new NbtString("ChunkProvider", this.ChunkProvider.GetType().FullName));
            file.SaveToFile(Path.Combine(this.BaseDirectory, "manifest.nbt"), NbtCompression.ZLib);
        }

        public void Save(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            BaseDirectory = path;
            Save();
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