using System;
using Microsoft.Xna.Framework;

namespace TrueCraft.Client.Modules
{
    public interface IGraphicalModule : IGameplayModule
    {
        void Draw(GameTime gameTime);
    }
}