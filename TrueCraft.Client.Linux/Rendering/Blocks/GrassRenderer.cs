using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Client.Linux.Rendering.Blocks
{
    public class GrassRenderer : BlockRenderer
    {
        static GrassRenderer()
        {
            BlockRenderer.RegisterRenderer(GrassBlock.BlockID, new GrassRenderer());
            for (int i = 0; i < Texture.Length; i++)
                Texture[i] *= new Vector2(16f / 256f);
        }

        private static Vector2 TopTexture = new Vector2(0, 0);
        private static Vector2 BottomTexture = new Vector2(2, 0);
        private static Vector2 SideTexture = new Vector2(3, 0);
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
            // Negative X
            TopTexture + Vector2.UnitX + Vector2.UnitY,
            TopTexture + Vector2.UnitY,
            TopTexture,
            TopTexture + Vector2.UnitX,
            // Negative X
            BottomTexture + Vector2.UnitX + Vector2.UnitY,
            BottomTexture + Vector2.UnitY,
            BottomTexture,
            BottomTexture + Vector2.UnitX,
        };

        public override VertexPositionNormalTexture[] Render(Vector3 offset, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            return CreateUniformCube(offset, Texture, indiciesOffset, out indicies);
        }
    }
}