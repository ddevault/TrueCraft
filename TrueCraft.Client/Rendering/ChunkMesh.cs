using System;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.Core.World;
using Microsoft.Xna.Framework;

namespace TrueCraft.Client.Rendering
{
    /// <summary>
    /// 
    /// </summary>
    public class ChunkMesh : Mesh
    {
        /// <summary>
        /// 
        /// </summary>
        public ReadOnlyChunk Chunk { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="device"></param>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        public ChunkMesh(ReadOnlyChunk chunk, GraphicsDevice device, VertexPositionNormalTexture[] vertices, int[] indices)
            : base(device, 1, true)
        {
            Chunk = chunk;
            Vertices = vertices;
            SetSubmesh(0, indices);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        protected override BoundingBox RecalculateBounds(VertexPositionNormalTexture[] vertices)
        {
            return new BoundingBox(
                new Vector3(Chunk.X * TrueCraft.Core.World.Chunk.Width, 0, Chunk.Z * TrueCraft.Core.World.Chunk.Depth),
                new Vector3(Chunk.X * TrueCraft.Core.World.Chunk.Width
                    + TrueCraft.Core.World.Chunk.Width, TrueCraft.Core.World.Chunk.Height,
                    Chunk.Z * TrueCraft.Core.World.Chunk.Depth + TrueCraft.Core.World.Chunk.Depth));
        }
    }
}