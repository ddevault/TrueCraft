using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API.Logic;

namespace TrueCraft.Client.Rendering.Blocks
{
    public class FarmlandRenderer : BlockRenderer
    {
        static FarmlandRenderer()
        {
            BlockRenderer.RegisterRenderer(FarmlandBlock.BlockID, new FarmlandRenderer());
            for (int i = 0; i < DryTexture.Length; i++)
            {
                DryTexture[i] *= new Vector2(16f / 256f);
                MoistTexture[i] *= new Vector2(16f / 256f);
            }
        }

        private static Vector2 DryTopTexture = new Vector2(7, 5);
        private static Vector2 MoistTopTexture = new Vector2(6, 5);
        private static Vector2 SideTexture = new Vector2(2, 0);

        private static Vector2[] DryTexture =
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
                DryTopTexture + Vector2.UnitX + Vector2.UnitY,
                DryTopTexture + Vector2.UnitY,
                DryTopTexture,
                DryTopTexture + Vector2.UnitX,
                // Negative Y
                SideTexture + Vector2.UnitX + Vector2.UnitY,
                SideTexture + Vector2.UnitY,
                SideTexture,
                SideTexture + Vector2.UnitX,
            };

        private static Vector2[] MoistTexture =
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
                MoistTopTexture + Vector2.UnitX + Vector2.UnitY,
                MoistTopTexture + Vector2.UnitY,
                MoistTopTexture,
                MoistTopTexture + Vector2.UnitX,
                // Negative Y
                SideTexture + Vector2.UnitX + Vector2.UnitY,
                SideTexture + Vector2.UnitY,
                SideTexture,
                SideTexture + Vector2.UnitX,
            };

        public override VertexPositionNormalColorTexture[] Render(BlockDescriptor descriptor, Vector3 offset,
            VisibleFaces faces, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            var texture = DryTexture;
            if (descriptor.Metadata == (byte)FarmlandBlock.MoistureLevel.Moist)
                texture = MoistTexture;

            var overhead = new Vector3(0.5f, 0.5f, 0.5f);
            var cube = CreateUniformCube(overhead, texture, faces, indiciesOffset, out indicies, Color.White);
            for (int i = 0; i < cube.Length; i++)
            {
                if (cube[i].Position.Y > 0)
                {
                    cube[i].Position.Y *= 15f / 16f;
                }
                cube[i].Position += offset;
                cube[i].Position -= overhead;
            }
            return cube;
        }
    }
}