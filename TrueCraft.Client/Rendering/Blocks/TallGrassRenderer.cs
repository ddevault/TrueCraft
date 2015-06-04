using System;
using TrueCraft.Core.Logic.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.API.Logic;

namespace TrueCraft.Client.Rendering.Blocks
{
    public class TallGrassRenderer : FlatQuadRenderer
    {
        static TallGrassRenderer()
        {
            BlockRenderer.RegisterRenderer(TallGrassBlock.BlockID, new TallGrassRenderer());
            BlockRenderer.RegisterRenderer(DeadBushBlock.BlockID, new TallGrassRenderer());
        }

        protected override Vector2 TextureMap { get { return new Vector2(7, 2); } }
        protected Vector2 DeadBushTextureMap { get { return new Vector2(7, 3); } }
        protected Vector2 FernTextureMap { get { return new Vector2(8, 3); } }
        protected Vector2[] DeadBushTexture, FernTexture;

        public TallGrassRenderer()
        {
            DeadBushTexture = new[]
            {
                DeadBushTextureMap + Vector2.UnitX + Vector2.UnitY,
                DeadBushTextureMap + Vector2.UnitY,
                DeadBushTextureMap,
                DeadBushTextureMap + Vector2.UnitX,
            };
            FernTexture = new[]
            {
                FernTextureMap + Vector2.UnitX + Vector2.UnitY,
                FernTextureMap + Vector2.UnitY,
                FernTextureMap,
                FernTextureMap + Vector2.UnitX,
            };
            for (int i = 0; i < Texture.Length; i++)
            {
                DeadBushTexture[i] *= new Vector2(16f / 256f);
                FernTexture[i] *= new Vector2(16f / 256f);
            }
        }

        public override VertexPositionNormalTexture[] Render(BlockDescriptor descriptor, Vector3 offset,
            Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            switch ((TallGrassBlock.TallGrassType)descriptor.Metadata)
            {
                case TallGrassBlock.TallGrassType.DeadBush:
                    return RenderQuads(descriptor, offset, DeadBushTexture, indiciesOffset, out indicies);
                case TallGrassBlock.TallGrassType.Fern:
                    return RenderQuads(descriptor, offset, FernTexture, indiciesOffset, out indicies);
                case TallGrassBlock.TallGrassType.TallGrass:
                default:
                    return RenderQuads(descriptor, offset, Texture, indiciesOffset, out indicies);
            }
        }
    }
}