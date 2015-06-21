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

        protected override Vector2 TextureMap { get { return new Vector2(11, 0); } }
    }
}
