using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API.Logic;

namespace TrueCraft.Client.Rendering.Blocks
{
    public class GrassRenderer : BlockRenderer
    {
        static GrassRenderer()
        {
            BlockRenderer.RegisterRenderer(GrassBlock.BlockID, new GrassRenderer());
            for (int i = 0; i < Texture.Length; i++)
                Texture[i] *= new Vector2(16f / 256f);
        }

        private static Vector2 TextureMap = new Vector2(0, 0);
        private static Vector2 EndsTexture = new Vector2(2, 0);
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
            // Positive Y
            TextureMap + Vector2.UnitX + Vector2.UnitY,
            TextureMap + Vector2.UnitY,
            TextureMap,
            TextureMap + Vector2.UnitX,
            // Negative Y
            EndsTexture + Vector2.UnitX + Vector2.UnitY,
            EndsTexture + Vector2.UnitY,
            EndsTexture,
            EndsTexture + Vector2.UnitX,
        };

        public static readonly Color BiomeColor = new Color(105, 169, 63);

        public override VertexPositionNormalColorTexture[] Render(BlockDescriptor descriptor, Vector3 offset,
            Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            var cube = CreateUniformCube(offset, Texture, indiciesOffset, out indicies, Color.White);
            // Apply biome colors to top of cube
            for (int i = (int)(CubeFace.PositiveY) * 4; i < (int)(CubeFace.PositiveY) * 4 + 4; i++)
            {
                cube[i].Color = BiomeColor; // TODO: Take this from biome
            }
            return cube;
        }
    }
}