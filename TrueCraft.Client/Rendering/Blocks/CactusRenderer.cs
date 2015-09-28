using System;
using TrueCraft.API.Logic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Client.Rendering.Blocks
{
    public class CactusRenderer : BlockRenderer
    {
        static CactusRenderer()
        {
            BlockRenderer.RegisterRenderer(CactusBlock.BlockID, new CactusRenderer());
            for (int j = 0; j < Texture.Length; j++)
                Texture[j] *= new Vector2(16f / 256f);
            for (int j = 0; j < TopTexture.Length; j++)
                TopTexture[j] *= new Vector2(16f / 256f);
        }

        private static Vector2 TextureMap = new Vector2(6, 4);
        private static Vector2[] Texture =
        {
            TextureMap + Vector2.UnitX + Vector2.UnitY,
            TextureMap + Vector2.UnitY,
            TextureMap,
            TextureMap + Vector2.UnitX
        };
        private static Vector2 TopTextureMap = new Vector2(6, 4);
        private static Vector2[] TopTexture =
        {
            TopTextureMap + Vector2.UnitX + Vector2.UnitY,
            TopTextureMap + Vector2.UnitY,
            TopTextureMap,
            TopTextureMap + Vector2.UnitX
        };

        public override VertexPositionNormalColorTexture[] Render(BlockDescriptor descriptor, Vector3 offset,
            VisibleFaces faces, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            // This is similar to how wheat is rendered
            indicies = new int[5 * 6];
            var verticies = new VertexPositionNormalColorTexture[5 * 6];
            int[] _indicies;
            var center = new Vector3(-0.5f, -0.5f, -0.5f);
            CubeFace side;
            VertexPositionNormalColorTexture[] quad;
            for (int _side = 0; _side < 4; _side++)
            {
                side = (CubeFace)_side;
                quad = CreateQuad(side, center, Texture, 0, indiciesOffset, out _indicies, Color.White);
                if (side == CubeFace.NegativeX || side == CubeFace.PositiveX)
                {
                    for (int i = 0; i < quad.Length; i++)
                    {
                        quad[i].Position.X *= 14f / 16f;
                        quad[i].Position += offset;
                    }
                }
                else
                {
                    for (int i = 0; i < quad.Length; i++)
                    {
                        quad[i].Position.Z *= 14f / 16f;
                        quad[i].Position += offset;
                    }
                }
                Array.Copy(quad, 0, verticies, _side * 4, 4);
                Array.Copy(_indicies, 0, indicies, _side * 6, 6);
            }
            side = CubeFace.PositiveY;
            quad = CreateQuad(side, center, TopTexture, 0, indiciesOffset, out _indicies, Color.White);
            if (side == CubeFace.NegativeX || side == CubeFace.PositiveX)
            {
                for (int i = 0; i < quad.Length; i++)
                {
                    quad[i].Position.X *= 14f / 16f;
                    quad[i].Position += offset;
                }
            }
            else
            {
                for (int i = 0; i < quad.Length; i++)
                {
                    quad[i].Position.Z *= 14f / 16f;
                    quad[i].Position += offset;
                }
            }
            Array.Copy(quad, 0, verticies, (int)side * 4, 4);
            Array.Copy(_indicies, 0, indicies, (int)side * 6, 6);
            for (int i = 0; i < verticies.Length; i++)
            {
                verticies[i].Position.Y -= 1 / 16f;
                verticies[i].Position -= center;
            }
            return verticies;
        }
    }
}
