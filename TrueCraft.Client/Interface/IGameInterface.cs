using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrueCraft.Client.Interface
{
    public interface IGameInterface
    {
        void Update(GameTime gameTime);
        void DrawSprites(GameTime gameTime, SpriteBatch spriteBatch);
    }
}