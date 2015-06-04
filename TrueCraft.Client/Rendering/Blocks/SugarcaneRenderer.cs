using System;
using TrueCraft.Core.Logic.Blocks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.API.Logic;

namespace TrueCraft.Client.Rendering.Blocks
{
    public class SugarcaneRenderer : FlatQuadRenderer
    {
        static SugarcaneRenderer()
        {
            BlockRenderer.RegisterRenderer(SugarcaneBlock.BlockID, new SugarcaneRenderer());
        }

        protected override Vector2 TextureMap { get { return new Vector2(9, 4); } }
    }
}