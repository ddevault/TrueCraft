using System;
using TrueCraft.Core.Logic.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.API.Logic;

namespace TrueCraft.Client.Rendering.Blocks
{
    public class SlabRenderer : BlockRenderer
    {
        static SlabRenderer()
        {
            BlockRenderer.RegisterRenderer(StoneSlabBlock.BlockID, new SlabRenderer());
            BlockRenderer.RegisterRenderer(DoubleStoneSlabBlock.BlockID, new SlabRenderer());
        }

        public enum SlabType : byte
        {
            Stone = 0,
            Sandstone = 1,
            Wood =2,
            Cobblestone = 3
        }

        protected virtual Vector2 GetSideTexture(SlabType type)
        {
            switch (type)
            {
                case SlabType.Stone:
                    return new Vector2(5, 0);

                case SlabType.Sandstone:
                    return new Vector2(0, 12);

                case SlabType.Wood:
                    return new Vector2(4, 0);

                case SlabType.Cobblestone:
                    return new Vector2(1, 0);

                default:
                    return Vector2.Zero;
            }
        }

        protected virtual Vector2 GetTopTexture(SlabType type)
        {
            switch (type)
            {
                case SlabType.Stone:
                    return new Vector2(6, 0);

                case SlabType.Sandstone:
                    return new Vector2(0, 13);

                case SlabType.Wood:
                    return new Vector2(4, 0);

                case SlabType.Cobblestone:
                    return new Vector2(1, 0);

                default:
                    return Vector2.Zero;
            }
        }

        protected virtual Vector2 GetBottomTexture(SlabType type)
        {
            switch (type)
            {
                case SlabType.Stone:
                    return new Vector2(6, 0);

                case SlabType.Sandstone:
                    return new Vector2(0, 14);

                case SlabType.Wood:
                    return new Vector2(4, 0);

                case SlabType.Cobblestone:
                    return new Vector2(1, 0);

                default:
                    return Vector2.Zero;
            }
        }

        protected virtual Vector2[] GetTextureMap(SlabType type, bool isDouble)
        {
            var sideTexture = GetSideTexture(type);
            var topTexture = GetTopTexture(type);
            var bottomTexture = GetBottomTexture(type);

            var result = new Vector2[]
            {
                // Positive Z
                sideTexture + Vector2.UnitX + ((isDouble) ? Vector2.UnitY : Vector2.UnitY / 2),
                sideTexture + ((isDouble) ? Vector2.UnitY : Vector2.UnitY / 2),
                sideTexture,
                sideTexture + Vector2.UnitX,
                // Negative Z
                sideTexture + Vector2.UnitX + ((isDouble) ? Vector2.UnitY : Vector2.UnitY / 2),
                sideTexture + ((isDouble) ? Vector2.UnitY : Vector2.UnitY / 2),
                sideTexture,
                sideTexture + Vector2.UnitX,
                // Positive X
                sideTexture + Vector2.UnitX + ((isDouble) ? Vector2.UnitY : Vector2.UnitY / 2),
                sideTexture + ((isDouble) ? Vector2.UnitY : Vector2.UnitY / 2),
                sideTexture,
                sideTexture + Vector2.UnitX,
                // Negative X
                sideTexture + Vector2.UnitX + ((isDouble) ? Vector2.UnitY : Vector2.UnitY / 2),
                sideTexture + ((isDouble) ? Vector2.UnitY : Vector2.UnitY / 2),
                sideTexture,
                sideTexture + Vector2.UnitX,
                // Positive Y
                topTexture + Vector2.UnitX + Vector2.UnitY,
                topTexture + Vector2.UnitY,
                topTexture,
                topTexture + Vector2.UnitX,
                // Negative Y
                bottomTexture + Vector2.UnitX + Vector2.UnitY,
                bottomTexture + Vector2.UnitY,
                bottomTexture,
                bottomTexture + Vector2.UnitX
            };

            for (int i = 0; i < result.Length; i++)
                result[i] *= new Vector2(16f / 256f);

            return result;
        }

        public static int Value = 0;

        public override VertexPositionNormalColorTexture[] Render(BlockDescriptor descriptor, Vector3 offset, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            if (descriptor.ID == StoneSlabBlock.BlockID)
                return RenderSlab(descriptor, offset, textureMap, indiciesOffset, out indicies);
            else
                return RenderDoubleSlab(descriptor, offset, textureMap, indiciesOffset, out indicies);
        }

        protected virtual VertexPositionNormalColorTexture[] RenderSlab(BlockDescriptor descriptor, Vector3 offset, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            var result = CreateUniformCube(offset, GetTextureMap((SlabType)descriptor.Metadata, false), indiciesOffset, out indicies, Color.White);
            for (int i = 0; i < 6; i++)
            {
                var face = (CubeFace)i;
                switch(face)
                {
                    case CubeFace.PositiveZ:
                    case CubeFace.NegativeZ:
                    case CubeFace.PositiveX:
                    case CubeFace.NegativeX:
                        for (int j = 2; j < 4; j++)
                            result[(i * 4) + j].Position.Y -= 0.5f;
                        break;

                    case CubeFace.PositiveY:
                        for (int j = 0; j < 4; j++)
                            result[(i * 4) + j].Position.Y -= 0.5f;
                        break;
                }
            }

            return result;
        }

        protected virtual VertexPositionNormalColorTexture[] RenderDoubleSlab(BlockDescriptor descriptor, Vector3 offset, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            return CreateUniformCube(offset, GetTextureMap((SlabType)descriptor.Metadata, true), indiciesOffset, out indicies, Color.White);
        }
    }
}
