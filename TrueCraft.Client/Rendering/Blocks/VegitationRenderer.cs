using System;
using TrueCraft.Core.Logic.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.API.Logic;

namespace TrueCraft.Client.Rendering.Blocks
{
    public class VegitationRenderer : FlatQuadRenderer
    {
        static VegitationRenderer()
        {
            BlockRenderer.RegisterRenderer(DandelionBlock.BlockID, new VegitationRenderer());
            BlockRenderer.RegisterRenderer(RoseBlock.BlockID, new VegitationRenderer());
            BlockRenderer.RegisterRenderer(TallGrassBlock.BlockID, new VegitationRenderer());
            BlockRenderer.RegisterRenderer(DeadBushBlock.BlockID, new VegitationRenderer());
            BlockRenderer.RegisterRenderer(SaplingBlock.BlockID, new VegitationRenderer());
        }
            
        protected Vector2 DandelionTextureMap { get { return new Vector2(13, 0); } }
        protected Vector2 RoseTextureMap { get { return new Vector2(12, 0); } }
        protected Vector2 TallGrassTextureMap { get { return new Vector2(7, 2); } }
        protected Vector2 DeadBushTextureMap { get { return new Vector2(7, 3); } }
        protected Vector2 FernTextureMap { get { return new Vector2(8, 3); } }
        protected Vector2 OakSaplingTextureMap { get { return new Vector2(15, 0); } }
        protected Vector2 SpruceSaplingTextureMap { get { return new Vector2(15, 3); } }
        protected Vector2 BirchSaplingTextureMap { get { return new Vector2(15, 4); } }
        protected Vector2[] DandelionTexture, RoseTexture;
        protected Vector2[] TallGrassTexture, DeadBushTexture, FernTexture;
        protected Vector2[] OakSaplingTexture, SpruceSaplingTexture, BirchSaplingTexture;

        public VegitationRenderer()
        {
            DandelionTexture = new[]
                {
                    DandelionTextureMap + Vector2.UnitX + Vector2.UnitY,
                    DandelionTextureMap + Vector2.UnitY,
                    DandelionTextureMap,
                    DandelionTextureMap + Vector2.UnitX,
                };
            RoseTexture = new[]
                {
                    RoseTextureMap + Vector2.UnitX + Vector2.UnitY,
                    RoseTextureMap + Vector2.UnitY,
                    RoseTextureMap,
                    RoseTextureMap + Vector2.UnitX,
                };
            TallGrassTexture = new[]
                {
                    TallGrassTextureMap + Vector2.UnitX + Vector2.UnitY,
                    TallGrassTextureMap + Vector2.UnitY,
                    TallGrassTextureMap,
                    TallGrassTextureMap + Vector2.UnitX,
                };
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
            OakSaplingTexture = new[]
                {
                    OakSaplingTextureMap + Vector2.UnitX + Vector2.UnitY,
                    OakSaplingTextureMap + Vector2.UnitY,
                    OakSaplingTextureMap,
                    OakSaplingTextureMap + Vector2.UnitX,
                };
            SpruceSaplingTexture = new[]
                {
                    SpruceSaplingTextureMap + Vector2.UnitX + Vector2.UnitY,
                    SpruceSaplingTextureMap + Vector2.UnitY,
                    SpruceSaplingTextureMap,
                    SpruceSaplingTextureMap + Vector2.UnitX,
                };
            BirchSaplingTexture = new[]
                {
                    BirchSaplingTextureMap + Vector2.UnitX + Vector2.UnitY,
                    BirchSaplingTextureMap + Vector2.UnitY,
                    BirchSaplingTextureMap,
                    BirchSaplingTextureMap + Vector2.UnitX,
                };
            for (int i = 0; i < Texture.Length; i++)
            {
                DandelionTexture[i] *= new Vector2(16f / 256f);
                RoseTexture[i] *= new Vector2(16f / 256f);
                TallGrassTexture[i] *= new Vector2(16f / 256f);
                DeadBushTexture[i] *= new Vector2(16f / 256f);
                FernTexture[i] *= new Vector2(16f / 256f);
                OakSaplingTexture[i] *= new Vector2(16f / 256f);
                SpruceSaplingTexture[i] *= new Vector2(16f / 256f);
                BirchSaplingTexture[i] *= new Vector2(16f / 256f);
            }
        }

        public override VertexPositionNormalColorTexture[] Render(BlockDescriptor descriptor, Vector3 offset,
            VisibleFaces faces, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            if (descriptor.ID == RoseBlock.BlockID)
                return RenderQuads(descriptor, offset, RoseTexture, indiciesOffset, out indicies, Color.White);
            else if (descriptor.ID == DandelionBlock.BlockID)
                return RenderQuads(descriptor, offset, DandelionTexture, indiciesOffset, out indicies, Color.White);
            else if (descriptor.ID == SaplingBlock.BlockID)
            {
                switch ((SaplingBlock.SaplingType)descriptor.Metadata)
                {
                    case SaplingBlock.SaplingType.Oak:
                    default:
                        return RenderQuads(descriptor, offset, OakSaplingTexture, indiciesOffset, out indicies, Color.White);
                    case SaplingBlock.SaplingType.Spruce:
                        return RenderQuads(descriptor, offset, SpruceSaplingTexture, indiciesOffset, out indicies, Color.White);
                    case SaplingBlock.SaplingType.Birch:
                        return RenderQuads(descriptor, offset, BirchSaplingTexture, indiciesOffset, out indicies, Color.White);
                }
            }
            else
            {
                switch ((TallGrassBlock.TallGrassType)descriptor.Metadata)
                {
                    case TallGrassBlock.TallGrassType.DeadBush:
                        return RenderQuads(descriptor, offset, DeadBushTexture, indiciesOffset, out indicies, Color.White);
                    case TallGrassBlock.TallGrassType.Fern:
                        return RenderQuads(descriptor, offset, FernTexture, indiciesOffset, out indicies, GrassRenderer.BiomeColor);
                    case TallGrassBlock.TallGrassType.TallGrass:
                    default:
                        return RenderQuads(descriptor, offset, TallGrassTexture, indiciesOffset, out indicies, GrassRenderer.BiomeColor);
                }
            }
        }
    }
}