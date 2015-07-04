using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueCraft.API.World;
using TrueCraft.API;
using TrueCraft.Core.World;

namespace TrueCraft.Core.TerrainGen
{
    public class FlatlandGenerator : IChunkProvider
    {
        static FlatlandGenerator()
        {
            DefaultGeneratorOptions = "1;7,2x3,2;1";
        }

        public FlatlandGenerator()
        {
            GeneratorOptions = DefaultGeneratorOptions;
            ChunkDecorators = new List<IChunkDecorator>();
        }

        public FlatlandGenerator(string generatorOptions)
        {
            GeneratorOptions = generatorOptions;
        }

        public static string DefaultGeneratorOptions { get; set; }

        public string GeneratorOptions
        {
            get { return generatorOptions; }
            set
            {
                generatorOptions = value;
                CreateLayers();
            }
        }

        public Biome Biome { get; set; }

        protected List<GeneratorLayer> Layers;
        private string generatorOptions;

        private void CreateLayers()
        {
            var parts = GeneratorOptions.Split(';');
            var layers = parts[1].Split(',');
            Layers = new List<GeneratorLayer>();
            double y = 0;
            foreach (var layer in layers)
            {
                var generatorLayer = new GeneratorLayer(layer);
                y += generatorLayer.Height;
                Layers.Add(generatorLayer);
            }
            Biome = (Biome)byte.Parse(parts[2]);
        }

        public IChunk GenerateChunk(IWorld world, Coordinates2D position)
        {
            var chunk = new Chunk(position);
            int y = 0;
            for (int i = 0; i < Layers.Count; i++)
            {
                int height = y + Layers[i].Height;
                while (y < height)
                {
                    for (int x = 0; x < 16; x++)
                    {
                        for (int z = 0; z < 16; z++)
                        {
                            chunk.SetBlockID(new Coordinates3D(x, y, z), Layers[i].BlockId);
                            chunk.SetMetadata(new Coordinates3D(x, y, z), Layers[i].Metadata);
                        }
                    }
                    y++;
                }
            }
            for (int i = 0; i < chunk.Biomes.Length; i++)
                chunk.Biomes[i] = (byte)Biome;
            chunk.TerrainPopulated = true;
            chunk.UpdateHeightMap();
            return chunk;
        }

        public string LevelType
        {
            get { return "FLAT"; }
        }

        public string GeneratorName { get { return "FLAT"; } }

        public long Seed { get; set; }

        public IList<IChunkDecorator> ChunkDecorators { get; set; }

        public Coordinates3D GetSpawn(IWorld world)
        {
            return Coordinates3D.Up * 5;
        }

        protected class GeneratorLayer
        {
            public GeneratorLayer(string layer)
            {
                var parts = layer.Split('x');
                int idIndex = 0;
                if (parts.Length == 2)
                    idIndex++;
                var idParts = parts[idIndex].Split(':');
                BlockId = byte.Parse(idParts[0]);
                if (idParts.Length == 2)
                    Metadata = (byte)(byte.Parse(idParts[1]) & 0xF);
                Height = 1;
                if (parts.Length == 2)
                    Height = int.Parse(parts[0]);
            }

            public byte BlockId { get; set; }
            public byte Metadata { get; set; }
            public int Height { get; set; }
        }
    }
}
