using System;
using TrueCraft.Core.Logic.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.API.Logic;

namespace TrueCraft.Client.Rendering.Blocks
{
    public class CraftingTableRenderer : BlockRenderer
    {
        static CraftingTableRenderer()
        {
            BlockRenderer.RegisterRenderer(CraftingTableBlock.BlockID, new CraftingTableRenderer());
            for (int i = 0; i < Texture.Length; i++)
                Texture[i] *= new Vector2(16f / 256f);
        }

        private static Vector2 TopTexture = new Vector2(11, 2);
        private static Vector2 BottomTexture = new Vector2(4, 0);
        private static Vector2 SideATexture = new Vector2(11, 3);
        private static Vector2 SideBTexture = new Vector2(12, 3);
        private static Vector2[] Texture =
        {
            // Positive Z
            SideATexture + Vector2.UnitX + Vector2.UnitY,
            SideATexture + Vector2.UnitY,
            SideATexture,
            SideATexture + Vector2.UnitX,
            // Negative Z
            SideATexture + Vector2.UnitX + Vector2.UnitY,
            SideATexture + Vector2.UnitY,
            SideATexture,
            SideATexture + Vector2.UnitX,
            // Positive X
            SideBTexture + Vector2.UnitX + Vector2.UnitY,
            SideBTexture + Vector2.UnitY,
            SideBTexture,
            SideBTexture + Vector2.UnitX,
            // Negative X
            SideBTexture + Vector2.UnitX + Vector2.UnitY,
            SideBTexture + Vector2.UnitY,
            SideBTexture,
            SideBTexture + Vector2.UnitX,
            // Negative Y
            TopTexture + Vector2.UnitX + Vector2.UnitY,
            TopTexture + Vector2.UnitY,
            TopTexture,
            TopTexture + Vector2.UnitX,
            // Negative Y
            BottomTexture + Vector2.UnitX + Vector2.UnitY,
            BottomTexture + Vector2.UnitY,
            BottomTexture,
            BottomTexture + Vector2.UnitX,
        };

        public override VertexPositionNormalColorTexture[] Render(BlockDescriptor descriptor, Vector3 offset,
            Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            return CreateUniformCube(offset, Texture, indiciesOffset, out indicies, Color.White);
        }
    }
}