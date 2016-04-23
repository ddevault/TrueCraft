using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API.Logic;

namespace TrueCraft.Client.Rendering.Blocks
{
    public class SnowRenderer : BlockRenderer
    {
        static SnowRenderer()
        {
            BlockRenderer.RegisterRenderer(SnowfallBlock.BlockID, new SnowRenderer());
            for (int i = 0; i < Texture.Length; i++)
                Texture[i] *= new Vector2(16f / 256f);
        }

        private static Vector2 TextureMap = new Vector2(2, 4);
        private static Vector2[] Texture =
        {
            TextureMap + Vector2.UnitX + Vector2.UnitY,
            TextureMap + Vector2.UnitY,
            TextureMap,
            TextureMap + Vector2.UnitX,
        };

        public override VertexPositionNormalColorTexture[] Render(BlockDescriptor descriptor, Vector3 offset,
            VisibleFaces faces, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            var lighting = new int[6];
            for (int i = 0; i < 6; i++)
            {
                var coords = (descriptor.Coordinates + FaceCoords[i]);
                lighting[i] = GetLight(descriptor.Chunk, coords);
            }
            var cube = CreateUniformCube(Vector3.Zero, Texture, faces, indiciesOffset, out indicies,
                Color.White, lighting);
            var heightMultiplier = new Vector3(1, ((descriptor.Metadata + 1) / 16f), 1);
            for (int i = 0; i < cube.Length; i++)
            {
                if (cube[i].Position.Y > 0)
                    cube[i].Position *= heightMultiplier;
                cube[i].Position += offset;
            }
            return cube;
        }
    }
}