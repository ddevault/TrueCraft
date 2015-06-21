using System;
using TrueCraft.Core.Logic.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.API.Logic;

namespace TrueCraft.Client.Rendering.Blocks
{
    public class CobwebRenderer : FlatQuadRenderer
    {
        static CobwebRenderer()
        {
            BlockRenderer.RegisterRenderer(CobwebBlock.BlockID, new CobwebRenderer());
        }

        protected Vector2 CobwebTextureMap { get { return new Vector2(11, 0); } }
        protected Vector2[] CobwebTexture;

        public CobwebRenderer()
        {
            CobwebTexture = new[]
                {
                    CobwebTextureMap + Vector2.UnitX + Vector2.UnitY,
                    CobwebTextureMap + Vector2.UnitY,
                    CobwebTextureMap,
                    CobwebTextureMap + Vector2.UnitX,
                };
            for (int i = 0; i < Texture.Length; i++)
            {
                CobwebTexture[i] *= new Vector2(16f / 256f);
            }
        }

        public override VertexPositionNormalColorTexture[] Render(BlockDescriptor descriptor, Vector3 offset,
            Tuple<int, int> textureMap, int indiciesOffset, out int[] indicies)
        {
            return RenderQuads(descriptor, offset, CobwebTexture, indiciesOffset, out indicies, Color.White);
        }
    }
}
