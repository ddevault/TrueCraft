using System;
using TrueCraft.Core.Logic.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.API.Logic;

namespace TrueCraft.Client.Rendering.Blocks
{
    public class SlabRenderer : BlockRenderer
    {
        private static Vector2 StoneTopTexture = new Vector2(6, 0);
        private static Vector2 StoneSideTexture = new Vector2(5, 0);
        private static Vector2 StoneBottomTexture = new Vector2(6, 0);
        private static Vector2 SandstoneTopTexture = new Vector2(0, 13);
        private static Vector2 SandstoneSideTexture = new Vector2(0, 12);
        private static Vector2 SandstoneBottomTexture = new Vector2(0, 14);
        private static Vector2 WoodTopTexture = new Vector2(4, 0);
        private static Vector2 WoodSideTexture = new Vector2(4, 0);
        private static Vector2 WoodBottomTexture = new Vector2(4, 0);
        private static Vector2 CobbleTopTexture = new Vector2(0, 1);
        private static Vector2 CobbleSideTexture = new Vector2(0, 1);
        private static Vector2 CobbleBottomTexture = new Vector2(0, 1);

        private static Vector2[] StoneTextureMap =
        {
            // Positive Z
            StoneSideTexture + Vector2.UnitX + Vector2.UnitY,
            StoneSideTexture + Vector2.UnitY,
            StoneSideTexture,
            StoneSideTexture + Vector2.UnitX,
            // Negative Z
            StoneSideTexture + Vector2.UnitX + Vector2.UnitY,
            StoneSideTexture + Vector2.UnitY,
            StoneSideTexture,
            StoneSideTexture + Vector2.UnitX,
            // Positive X
            StoneSideTexture + Vector2.UnitX + Vector2.UnitY,
            StoneSideTexture + Vector2.UnitY,
            StoneSideTexture,
            StoneSideTexture + Vector2.UnitX,
            // Negative X
            StoneSideTexture + Vector2.UnitX + Vector2.UnitY,
            StoneSideTexture + Vector2.UnitY,
            StoneSideTexture,
            StoneSideTexture + Vector2.UnitX,
            // Negative Y
            StoneTopTexture + Vector2.UnitX + Vector2.UnitY,
            StoneTopTexture + Vector2.UnitY,
            StoneTopTexture,
            StoneTopTexture + Vector2.UnitX,
            // Negative Y
            StoneBottomTexture + Vector2.UnitX + Vector2.UnitY,
            StoneBottomTexture + Vector2.UnitY,
            StoneBottomTexture,
            StoneBottomTexture + Vector2.UnitX,
        };

        private static Vector2[] SandstoneTextureMap =
        {
            // Positive Z
            SandstoneSideTexture + Vector2.UnitX + Vector2.UnitY,
            SandstoneSideTexture + Vector2.UnitY,
            SandstoneSideTexture,
            SandstoneSideTexture + Vector2.UnitX,
            // Negative Z
            SandstoneSideTexture + Vector2.UnitX + Vector2.UnitY,
            SandstoneSideTexture + Vector2.UnitY,
            SandstoneSideTexture,
            SandstoneSideTexture + Vector2.UnitX,
            // Positive X
            SandstoneSideTexture + Vector2.UnitX + Vector2.UnitY,
            SandstoneSideTexture + Vector2.UnitY,
            SandstoneSideTexture,
            SandstoneSideTexture + Vector2.UnitX,
            // Negative X
            SandstoneSideTexture + Vector2.UnitX + Vector2.UnitY,
            SandstoneSideTexture + Vector2.UnitY,
            SandstoneSideTexture,
            SandstoneSideTexture + Vector2.UnitX,
            // Negative Y
            SandstoneTopTexture + Vector2.UnitX + Vector2.UnitY,
            SandstoneTopTexture + Vector2.UnitY,
            SandstoneTopTexture,
            SandstoneTopTexture + Vector2.UnitX,
            // Negative Y
            SandstoneBottomTexture + Vector2.UnitX + Vector2.UnitY,
            SandstoneBottomTexture + Vector2.UnitY,
            SandstoneBottomTexture,
            SandstoneBottomTexture + Vector2.UnitX,
        };

        private static Vector2[] WoodTextureMap =
        {
            // Positive Z
            WoodSideTexture + Vector2.UnitX + Vector2.UnitY,
            WoodSideTexture + Vector2.UnitY,
            WoodSideTexture,
            WoodSideTexture + Vector2.UnitX,
            // Negative Z
            WoodSideTexture + Vector2.UnitX + Vector2.UnitY,
            WoodSideTexture + Vector2.UnitY,
            WoodSideTexture,
            WoodSideTexture + Vector2.UnitX,
            // Positive X
            WoodSideTexture + Vector2.UnitX + Vector2.UnitY,
            WoodSideTexture + Vector2.UnitY,
            WoodSideTexture,
            WoodSideTexture + Vector2.UnitX,
            // Negative X
            WoodSideTexture + Vector2.UnitX + Vector2.UnitY,
            WoodSideTexture + Vector2.UnitY,
            WoodSideTexture,
            WoodSideTexture + Vector2.UnitX,
            // Negative Y
            WoodTopTexture + Vector2.UnitX + Vector2.UnitY,
            WoodTopTexture + Vector2.UnitY,
            WoodTopTexture,
            WoodTopTexture + Vector2.UnitX,
            // Negative Y
            WoodBottomTexture + Vector2.UnitX + Vector2.UnitY,
            WoodBottomTexture + Vector2.UnitY,
            WoodBottomTexture,
            WoodBottomTexture + Vector2.UnitX,
        };

        private static Vector2[] CobbleTextureMap =
        {
            // Positive Z
            CobbleSideTexture + Vector2.UnitX + Vector2.UnitY,
            CobbleSideTexture + Vector2.UnitY,
            CobbleSideTexture,
            CobbleSideTexture + Vector2.UnitX,
            // Negative Z
            CobbleSideTexture + Vector2.UnitX + Vector2.UnitY,
            CobbleSideTexture + Vector2.UnitY,
            CobbleSideTexture,
            CobbleSideTexture + Vector2.UnitX,
            // Positive X
            CobbleSideTexture + Vector2.UnitX + Vector2.UnitY,
            CobbleSideTexture + Vector2.UnitY,
            CobbleSideTexture,
            CobbleSideTexture + Vector2.UnitX,
            // Negative X
            CobbleSideTexture + Vector2.UnitX + Vector2.UnitY,
            CobbleSideTexture + Vector2.UnitY,
            CobbleSideTexture,
            CobbleSideTexture + Vector2.UnitX,
            // Negative Y
            CobbleTopTexture + Vector2.UnitX + Vector2.UnitY,
            CobbleTopTexture + Vector2.UnitY,
            CobbleTopTexture,
            CobbleTopTexture + Vector2.UnitX,
            // Negative Y
            CobbleBottomTexture + Vector2.UnitX + Vector2.UnitY,
            CobbleBottomTexture + Vector2.UnitY,
            CobbleBottomTexture,
            CobbleBottomTexture + Vector2.UnitX,
        };

        static SlabRenderer()
        {
            BlockRenderer.RegisterRenderer(SlabBlock.BlockID, new SlabRenderer());
            BlockRenderer.RegisterRenderer(DoubleSlabBlock.BlockID, new SlabRenderer());

            for (int i = 0; i < StoneTextureMap.Length; i++)
            {
                StoneTextureMap[i] *= new Vector2(16f / 256f);
                SandstoneTextureMap[i] *= new Vector2(16f / 256f);
                WoodTextureMap[i] *= new Vector2(16f / 256f);
                CobbleTextureMap[i] *= new Vector2(16f / 256f);
            }
        }

        protected virtual Vector2[] GetTextureMap(SlabBlock.SlabMaterial material)
        {
            switch (material)
            {
                case SlabBlock.SlabMaterial.Stone:
                    return StoneTextureMap;
                case SlabBlock.SlabMaterial.Standstone:
                    return SandstoneTextureMap;
                case SlabBlock.SlabMaterial.Wooden:
                    return WoodTextureMap;
                case SlabBlock.SlabMaterial.Cobblestone:
                    return CobbleTextureMap;
                default:
                    return StoneTextureMap;
            }
        }

        public override VertexPositionNormalColorTexture[] Render(BlockDescriptor descriptor, Vector3 offset, 
            VisibleFaces faces, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            if (descriptor.ID == SlabBlock.BlockID)
                return RenderSlab(descriptor, offset, textureMap, indiciesOffset, out indicies);
            else
                return RenderDoubleSlab(descriptor, offset, textureMap, indiciesOffset, out indicies);
        }

        protected virtual VertexPositionNormalColorTexture[] RenderSlab(BlockDescriptor descriptor, Vector3 offset, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            int[] lighting = new int[6];
            for (int i = 0; i < 6; i++)
            {
                var coords = (descriptor.Coordinates + FaceCoords[i]);
                lighting[i] = GetLight(descriptor.Chunk, coords);
            }
            var result = CreateUniformCube(offset,
                GetTextureMap((SlabBlock.SlabMaterial)descriptor.Metadata), VisibleFaces.All,
                indiciesOffset, out indicies, Color.White, lighting);
            for (int i = 0; i < 6; i++)
            {
                var face = (CubeFace)i;
                switch(face)
                {
                    case CubeFace.PositiveZ:
                    case CubeFace.NegativeZ:
                    case CubeFace.PositiveX:
                    case CubeFace.NegativeX:
                        for (int j = 0; j < 2; j++)
                            result[(i * 4) + j].Texture.Y -= (1f / 32f);
                        for (int k = 2; k < 4; k++)
                        {
                            result[(i * 4) + k].Position.Y -= 0.5f;
                            // result[(i * 4) + k].Texture.Y -= (1f / 16f);
                        }
                        break;

                    case CubeFace.PositiveY:
                        for (int j = 0; j < 4; j++)
                            result[(i * 4) + j].Position.Y -= 0.5f;
                        break;
                }
            }

            return result;
        }

        protected virtual VertexPositionNormalColorTexture[] RenderDoubleSlab(BlockDescriptor descriptor,
            Vector3 offset, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            return CreateUniformCube(offset, GetTextureMap((SlabBlock.SlabMaterial)descriptor.Metadata),
                VisibleFaces.All, indiciesOffset, out indicies, Color.White);
        }
    }
}
