using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;
using TrueCraft.Client.Linux.Rendering;
using TrueCraft.Core.World;
using TrueCraft.API;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using TrueCraft.API.Logic;

namespace TrueCraft.Client.Linux
{
    /// <summary>
    /// A daemon of sorts that creates meshes from chunks.
    /// Passing meshes back is NOT thread-safe.
    /// </summary>
    public class ChunkConverter
    {
        public delegate void ChunkConsumer(Mesh mesh);

        private ConcurrentQueue<ReadOnlyChunk> ChunkQueue { get; set; }
        private Thread ChunkWorker { get; set; }
        private GraphicsDevice Graphics { get; set; }
        private IBlockRepository BlockRepository { get; set; }
        private ChunkConsumer Consumer { get; set; }

        public ChunkConverter(GraphicsDevice graphics, IBlockRepository blockRepository)
        {
            ChunkQueue = new ConcurrentQueue<ReadOnlyChunk>();
            ChunkWorker = new Thread(new ThreadStart(DoChunks));
            BlockRepository = blockRepository;
            Graphics = graphics;
        }

        public void QueueChunk(ReadOnlyChunk chunk)
        {
            ChunkQueue.Enqueue(chunk);
        }

        public void Start(ChunkConsumer consumer)
        {
            Consumer = consumer;
            ChunkWorker.Start();
        }

        public void Stop()
        {
            ChunkWorker.Abort();
        }

        private void DoChunks()
        {
            bool idle = true;
            while (true)
            {
                ReadOnlyChunk chunk;
                if (ChunkQueue.Any())
                {
                    while (!ChunkQueue.TryDequeue(out chunk))
                    {
                    }
                    var mesh = ProcessChunk(chunk);
                    Consumer(mesh);
                }
                if (idle)
                    Thread.Sleep(100);
            }
        }

        private Mesh ProcessChunk(ReadOnlyChunk chunk)
        {
            var verticies = new List<VertexPositionNormalTexture>();
            var indicies = new List<int>();
            for (byte x = 0; x < Chunk.Width; x++)
            {
                for (byte z = 0; z < Chunk.Depth; z++)
                {
                    //var height = chunk.Chunk.GetHeight(x, z);
                    for (byte y = 0; y < Chunk.Height; y++)
                    {
                        var coords = new Coordinates3D(x, y, z);
                        var id = chunk.GetBlockId(coords);
                        var provider = BlockRepository.GetBlockProvider(id);
                        var textureMap = provider.GetTextureMap(chunk.GetMetadata(coords));
                        if (textureMap == null)
                        {
                            // TODO: handle this better
                            textureMap = new Tuple<int, int>(0, 0);
                        }
                        if (id != 0)
                        {
                            int[] i;
                            var v = CreateUniformCube(new Vector3(chunk.X * Chunk.Width + x, y, chunk.Z * Chunk.Depth + z),
                                textureMap, indicies.Count, out i);
                            verticies.AddRange(v);
                            indicies.AddRange(i);
                        }
                    }
                }
            }
            Console.WriteLine("Created mesh for {0}, {0}", chunk.X, chunk.Z);
            return new Mesh(Graphics, verticies.ToArray(), indicies.ToArray());
        }

        private VertexPositionNormalTexture[] CreateUniformCube(Vector3 offset, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            var texCoords = new Vector2(textureMap.Item1, textureMap.Item2);
            var texture = new[]
            {
                texCoords + Vector2.UnitX + Vector2.UnitY,
                texCoords + Vector2.UnitY,
                texCoords,
                texCoords + Vector2.UnitX
            };
            for (int i = 0; i < texture.Length; i++)
                texture[i] *= new Vector2(16f / 256f);
            indicies = new int[6 * 6];
            var verticies = new VertexPositionNormalTexture[4 * 6];
            int[] _indicies;
            for (int _side = 0; _side < 6; _side++)
            {
                var side = (Mesh.CubeFace)_side;
                var quad = Mesh.CreateQuad(side, offset, texture, indiciesOffset, out _indicies);
                Array.Copy(quad, 0, verticies, _side * 4, 4);
                Array.Copy(_indicies, 0, indicies, _side * 6, 6);
            }
            return verticies;
        }
    }
}