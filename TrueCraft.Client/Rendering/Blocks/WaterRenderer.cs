using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.API.Logic;

namespace TrueCraft.Client.Rendering.Blocks
{
    public class WaterRenderer : BlockRenderer
    {
        static WaterRenderer()
        {
            BlockRenderer.RegisterRenderer(WaterBlock.BlockID, new WaterRenderer());
            BlockRenderer.RegisterRenderer(StationaryWaterBlock.BlockID, new WaterRenderer());
            for (int i = 0; i < Texture.Length; i++)
                Texture[i] *= new Vector2(16f / 256f);
        }

        private static Vector2 TextureMap = new Vector2(13, 12);
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

            // TODO: Rest of water rendering (shape and level and so on)
            var overhead = new Vector3(0.5f, 0.5f, 0.5f);
            var cube = CreateUniformCube(overhead, Texture, faces, indiciesOffset, out indicies, Color.Blue, lighting);
            for (int i = 0; i < cube.Length; i++)
            {
                if (cube[i].Position.Y > 0)
                {
                    cube[i].Position.Y *= 14f / 16f;
                }
                cube[i].Position += offset;
                cube[i].Position -= overhead;
            }
            return cube;
        }
    }
}