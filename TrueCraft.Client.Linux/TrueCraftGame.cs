using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TrueCraft.Client.Linux
{
    public class TrueCraftGame : Game
    {
        private GraphicsDeviceManager Graphics { get; set; }
        
        public TrueCraftGame()
        {
            Window.Title = "TrueCraft";
            Graphics = new GraphicsDeviceManager(this);
            Graphics.IsFullScreen = false;
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1080;
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}