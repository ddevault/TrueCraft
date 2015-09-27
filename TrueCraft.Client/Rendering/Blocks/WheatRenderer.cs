using System;
using TrueCraft.API.Logic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Client.Rendering
{
    public class WheatRenderer : BlockRenderer
    {
        static WheatRenderer()
        {
            BlockRenderer.RegisterRenderer(CropsBlock.BlockID, new WheatRenderer());
        }

        private Vector2[][] Textures;

        public WheatRenderer()
        {
            var textureMap = new Vector2(8, 5);
            Textures = new Vector2[8][];
            for (int i = 0; i < 8; i++)
            {
                Textures[i] = new[]
                {
                    textureMap + Vector2.UnitX + Vector2.UnitY,
                    textureMap + Vector2.UnitY,
                    textureMap,
                    textureMap + Vector2.UnitX,
                };
                for (int j = 0; j < Textures[i].Length; j++)
                    Textures[i][j] *= new Vector2(16f / 256f);
                textureMap += new Vector2(1, 0);
            }
        }

        public override VertexPositionNormalColorTexture[] Render(BlockDescriptor descriptor, Vector3 offset,
            VisibleFaces faces, Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            // Wheat is rendered by rendering the four vertical faces of a cube, then moving them
            // towards the middle. We also render a second set of four faces so that you can see
            // each face from the opposite side (to avoid culling)
            var texture = Textures[0];
            if (descriptor.Metadata < Textures.Length)
                texture = Textures[descriptor.Metadata];
            indicies = new int[4 * 2 * 6];
            var verticies = new VertexPositionNormalColorTexture[4 * 2 * 6];
            int[] _indicies;
            var center = new Vector3(-0.5f, -0.5f, -0.5f);
            for (int _side = 0; _side < 4; _side++) // Y faces are the last two in the CubeFace enum, so we can just iterate to 4
            {
                var side = (CubeFace)_side;
                var quad = CreateQuad(side, center, texture, 0, indiciesOffset, out _indicies, Color.White);
                if (side == CubeFace.NegativeX || side == CubeFace.PositiveX)
                {
                    for (int i = 0; i < quad.Length; i++)
                    {
                        quad[i].Position.X *= 0.5f;
                        quad[i].Position += offset;
                    }
                }
                else
                {
                    for (int i = 0; i < quad.Length; i++)
                    {
                        quad[i].Position.Z *= 0.5f;
                        quad[i].Position += offset;
                    }
                }
                Array.Copy(quad, 0, verticies, _side * 4, 4);
                Array.Copy(_indicies, 0, indicies, _side * 6, 6);
            }
            indiciesOffset += 4 * 6;
            for (int _side = 0; _side < 4; _side++)
            {
                var side = (CubeFace)_side;
                var quad = CreateQuad(side, center, texture, 0, indiciesOffset, out _indicies, Color.White);
                if (side == CubeFace.NegativeX || side == CubeFace.PositiveX)
                {
                    for (int i = 0; i < quad.Length; i++)
                    {
                        quad[i].Position.X *= 0.5f;
                        quad[i].Position.X = -quad[i].Position.X;
                        quad[i].Position += offset;
                    }
                }
                else
                {
                    for (int i = 0; i < quad.Length; i++)
                    {
                        quad[i].Position.Z *= 0.5f;
                        quad[i].Position.Z = -quad[i].Position.Z;
                        quad[i].Position += offset;
                    }
                }
                Array.Copy(quad, 0, verticies, _side * 4 + 4 * 4, 4);
                Array.Copy(_indicies, 0, indicies, _side * 6 + 6 * 4, 6);
            }
            for (int i = 0; i < verticies.Length; i++)
            {
                verticies[i].Position.Y -= 1 / 16f;
                verticies[i].Position -= center;
            }
            return verticies;
        }
    }
}