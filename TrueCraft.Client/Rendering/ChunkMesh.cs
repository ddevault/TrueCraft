using System;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.Core.World;
using Microsoft.Xna.Framework;

namespace TrueCraft.Client.Rendering
{
    public class ChunkMesh : Mesh
    {
        public ReadOnlyChunk Chunk { get; set; }

        public ChunkMesh(ReadOnlyChunk chunk, GraphicsDevice device, VertexPositionNormalTexture[] verticies, int[] indicies)
            : base(device, verticies, indicies, false)
        {
            Chunk = chunk;
            BoundingBox = new BoundingBox(
                new Vector3(chunk.X * TrueCraft.Core.World.Chunk.Width, 0, chunk.Z * TrueCraft.Core.World.Chunk.Depth),
                new Vector3(chunk.X * TrueCraft.Core.World.Chunk.Width
                    + TrueCraft.Core.World.Chunk.Width, TrueCraft.Core.World.Chunk.Height,
                    chunk.Z * TrueCraft.Core.World.Chunk.Depth + TrueCraft.Core.World.Chunk.Depth));
        }
    }
}