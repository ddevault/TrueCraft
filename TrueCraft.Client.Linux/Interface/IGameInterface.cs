using System;
using Microsoft.Xna.Framework;

namespace TrueCraft.Client.Linux.Interface
{
    public interface IGameInterface
    {
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}