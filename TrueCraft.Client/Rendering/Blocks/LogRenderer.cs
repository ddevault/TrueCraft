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
            for (int i = 0; i < BaseTexture.Length; i++)
            {
                BaseTexture[i] *= new Vector2(16f / 256f);
                SpruceTexture[i] *= new Vector2(16f / 256f);
                BirchTexture[i] *= new Vector2(16f / 256f);
            }
        }

        private static Vector2 BaseEndsTexture = new Vector2(5, 1);
        private static Vector2 BaseSidesTexture = new Vector2(4, 1);
        private static Vector2 SpruceSidesTexture = new Vector2(4, 7);
        private static Vector2 BirchSidesTexture = new Vector2(5, 7);
        private static Vector2[] BaseTexture =
        {
            // Positive Z
            BaseSidesTexture + Vector2.UnitX + Vector2.UnitY,
            BaseSidesTexture + Vector2.UnitY,
            BaseSidesTexture,
            BaseSidesTexture + Vector2.UnitX,
            // Negative Z
            BaseSidesTexture + Vector2.UnitX + Vector2.UnitY,
            BaseSidesTexture + Vector2.UnitY,
            BaseSidesTexture,
            BaseSidesTexture + Vector2.UnitX,
            // Positive X
            BaseSidesTexture + Vector2.UnitX + Vector2.UnitY,
            BaseSidesTexture + Vector2.UnitY,
            BaseSidesTexture,
            BaseSidesTexture + Vector2.UnitX,
            // Negative X
            BaseSidesTexture + Vector2.UnitX + Vector2.UnitY,
            BaseSidesTexture + Vector2.UnitY,
            BaseSidesTexture,
            BaseSidesTexture + Vector2.UnitX,
            // Positive Y
            BaseEndsTexture + Vector2.UnitX + Vector2.UnitY,
            BaseEndsTexture + Vector2.UnitY,
            BaseEndsTexture,
            BaseEndsTexture + Vector2.UnitX,
            // Negative Y
            BaseEndsTexture + Vector2.UnitX + Vector2.UnitY,
            BaseEndsTexture + Vector2.UnitY,
            BaseEndsTexture,
            BaseEndsTexture + Vector2.UnitX,
        };
        private static Vector2[] SpruceTexture =
        {
            // Positive Z
            SpruceSidesTexture + Vector2.UnitX + Vector2.UnitY,
            SpruceSidesTexture + Vector2.UnitY,
            SpruceSidesTexture,
            SpruceSidesTexture + Vector2.UnitX,
            // Negative Z
            SpruceSidesTexture + Vector2.UnitX + Vector2.UnitY,
            SpruceSidesTexture + Vector2.UnitY,
            SpruceSidesTexture,
            SpruceSidesTexture + Vector2.UnitX,
            // Positive X
            SpruceSidesTexture + Vector2.UnitX + Vector2.UnitY,
            SpruceSidesTexture + Vector2.UnitY,
            SpruceSidesTexture,
            SpruceSidesTexture + Vector2.UnitX,
            // Negative X
            SpruceSidesTexture + Vector2.UnitX + Vector2.UnitY,
            SpruceSidesTexture + Vector2.UnitY,
            SpruceSidesTexture,
            SpruceSidesTexture + Vector2.UnitX,
            // Positive Y
            BaseEndsTexture + Vector2.UnitX + Vector2.UnitY,
            BaseEndsTexture + Vector2.UnitY,
            BaseEndsTexture,
            BaseEndsTexture + Vector2.UnitX,
            // Negative Y
            BaseEndsTexture + Vector2.UnitX + Vector2.UnitY,
            BaseEndsTexture + Vector2.UnitY,
            BaseEndsTexture,
            BaseEndsTexture + Vector2.UnitX,
        };
        private static Vector2[] BirchTexture =
        {
            // Positive Z
            BirchSidesTexture + Vector2.UnitX + Vector2.UnitY,
            BirchSidesTexture + Vector2.UnitY,
            BirchSidesTexture,
            BirchSidesTexture + Vector2.UnitX,
            // Negative Z
            BirchSidesTexture + Vector2.UnitX + Vector2.UnitY,
            BirchSidesTexture + Vector2.UnitY,
            BirchSidesTexture,
            BirchSidesTexture + Vector2.UnitX,
            // Positive X
            BirchSidesTexture + Vector2.UnitX + Vector2.UnitY,
            BirchSidesTexture + Vector2.UnitY,
            BirchSidesTexture,
            BirchSidesTexture + Vector2.UnitX,
            // Negative X
            BirchSidesTexture + Vector2.UnitX + Vector2.UnitY,
            BirchSidesTexture + Vector2.UnitY,
            BirchSidesTexture,
            BirchSidesTexture + Vector2.UnitX,
            // Positive Y
            BaseEndsTexture + Vector2.UnitX + Vector2.UnitY,
            BaseEndsTexture + Vector2.UnitY,
            BaseEndsTexture,
            BaseEndsTexture + Vector2.UnitX,
            // Negative Y
            BaseEndsTexture + Vector2.UnitX + Vector2.UnitY,
            BaseEndsTexture + Vector2.UnitY,
            BaseEndsTexture,
            BaseEndsTexture + Vector2.UnitX,
        };

        public override VertexPositionNormalTexture[] Render(BlockDescriptor descriptor, Vector3 offset,
             Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            switch ((WoodBlock.WoodType)descriptor.Metadata)
            {
                case WoodBlock.WoodType.Spruce:
                    return CreateUniformCube(offset, SpruceTexture, indiciesOffset, out indicies);
                case WoodBlock.WoodType.Birch:
                    return CreateUniformCube(offset, BirchTexture, indiciesOffset, out indicies);
                case WoodBlock.WoodType.Oak:
                default:
                    return CreateUniformCube(offset, BaseTexture, indiciesOffset, out indicies);
            }
        }
    }
}