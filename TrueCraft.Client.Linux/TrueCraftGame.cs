using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TrueCraft.Client.Linux.Interface;
using System.IO;

namespace TrueCraft.Client.Linux
{
    public class TrueCraftGame : Game
    {
        private MultiplayerClient Client { get; set; }
        private GraphicsDeviceManager Graphics { get; set; }
        private List<IGameInterface> Interfaces { get; set; }
        private FontRenderer DejaVu { get; set; }
        private SpriteBatch SpriteBatch { get; set; }

        public TrueCraftGame(MultiplayerClient client)
        {
            Window.Title = "TrueCraft";
            Content.RootDirectory = "Content";
            Graphics = new GraphicsDeviceManager(this);
            Graphics.IsFullScreen = false;
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Client = client;
        }

        protected override void Initialize()
        {
            Interfaces = new List<IGameInterface>();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            FontFile fontFile;
            using (var f = File.OpenRead(Path.Combine(Content.RootDirectory, "dejavu.fnt")))
                fontFile = FontLoader.Load(f);
            var fontTexture = Content.Load<Texture2D>("dejavu_0.png");
            DejaVu = new FontRenderer(fontFile, fontTexture);
            Interfaces.Add(new ChatInterface(Client, DejaVu));
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            foreach (var i in Interfaces)
            {
                i.Update(gameTime);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            SpriteBatch.Begin();
            foreach (var i in Interfaces)
            {
                i.DrawSprites(gameTime, SpriteBatch);
            }
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}