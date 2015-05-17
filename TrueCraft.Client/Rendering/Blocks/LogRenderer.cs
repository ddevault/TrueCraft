using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API.Logic;

namespace TrueCraft.Client.Rendering.Blocks
{
    public class LogRenderer : BlockRenderer
    {
        static LogRenderer()
        {
            BlockRenderer.RegisterRenderer(WoodBlock.BlockID, new LogRenderer());
            for (int i = 0; i < Texture.Length; i++)
                Texture[i] *= new Vector2(16f / 256f);
        }

        private static Vector2 EndsTexture = new Vector2(5, 1);
        private static Vector2 SideTexture = new Vector2(4, 1);
        private static Vector2[] Texture =
        {
            // Positive Z
            SideTexture + Vector2.UnitX + Vector2.UnitY,
            SideTexture + Vector2.UnitY,
            SideTexture,
            SideTexture + Vector2.UnitX,
            // Negative Z
            SideTexture + Vector2.UnitX + Vector2.UnitY,
            SideTexture + Vector2.UnitY,
            SideTexture,
            SideTexture + Vector2.UnitX,
            // Positive X
            SideTexture + Vector2.UnitX + Vector2.UnitY,
            SideTexture + Vector2.UnitY,
            SideTexture,
            SideTexture + Vector2.UnitX,
            // Negative X
            SideTexture + Vector2.UnitX + Vector2.UnitY,
            SideTexture + Vector2.UnitY,
            SideTexture,
            SideTexture + Vector2.UnitX,
            // Positive Y
            EndsTexture + Vector2.UnitX + Vector2.UnitY,
            EndsTexture + Vector2.UnitY,
            EndsTexture,
            EndsTexture + Vector2.UnitX,
            // Negative Y
            EndsTexture + Vector2.UnitX + Vector2.UnitY,
            EndsTexture + Vector2.UnitY,
            EndsTexture,
            EndsTexture + Vector2.UnitX,
        };

        public override VertexPositionNormalTexture[] Render(BlockDescriptor descriptor, Vector3 offset,
            Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            return CreateUniformCube(offset, Texture, indiciesOffset, out indicies);
        }
    }
}