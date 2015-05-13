using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TrueCraft.Client.Linux.Interface;
using System.IO;
using System.Net;
using TrueCraft.API;

namespace TrueCraft.Client.Linux
{
    public class TrueCraftGame : Game
    {
        private MultiplayerClient Client { get; set; }
        private GraphicsDeviceManager Graphics { get; set; }
        private List<IGameInterface> Interfaces { get; set; }
        private FontRenderer DejaVu { get; set; }
        private SpriteBatch SpriteBatch { get; set; }
        private IPEndPoint EndPoint { get; set; }
        private ChunkConverter ChunkConverter { get; set; }
        private DateTime NextPhysicsUpdate { get; set; }

        public TrueCraftGame(MultiplayerClient client, IPEndPoint endPoint)
        {
            Window.Title = "TrueCraft";
            Content.RootDirectory = "Content";
            Graphics = new GraphicsDeviceManager(this);
            Graphics.IsFullScreen = false;
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Client = client;
            EndPoint = endPoint;
            ChunkConverter = new ChunkConverter();
            Client.ChunkLoaded += (sender, e) => ChunkConverter.QueueChunk(e.Chunk);
            NextPhysicsUpdate = DateTime.MinValue;
        }

        protected override void Initialize()
        {
            Interfaces = new List<IGameInterface>();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            base.Initialize(); // (calls LoadContent)
            ChunkConverter.Start();
            Client.Connect(EndPoint);
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

        protected override void OnExiting(object sender, EventArgs args)
        {
            ChunkConverter.Stop();
            base.OnExiting(sender, args);
        }

        protected virtual void UpdateKeyboard(GameTime gameTime, KeyboardState state)
        {
            if (state.IsKeyDown(Keys.Escape))
                Exit();
            // TODO: Handle rotation
            // TODO: Rebindable keys
            // TODO: Horizontal terrain collisions
            TrueCraft.API.Vector3 delta = TrueCraft.API.Vector3.Zero;
            if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
                delta = TrueCraft.API.Vector3.Left;
            if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
                delta = TrueCraft.API.Vector3.Right;
            if (state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.W))
                delta = TrueCraft.API.Vector3.Forwards;
            if (state.IsKeyDown(Keys.Down) || state.IsKeyDown(Keys.S))
                delta = TrueCraft.API.Vector3.Backwards;
            Client.Position += delta * (gameTime.ElapsedGameTime.TotalSeconds * 4.3717); // Note: 4.3717 is the speed of a Minecraft player in m/s
        }

        protected override void Update(GameTime gameTime)
        {
            foreach (var i in Interfaces)
            {
                i.Update(gameTime);
            }
            if (NextPhysicsUpdate < DateTime.Now)
            {
                if (Client.World.FindChunk(new Coordinates2D((int)Client.Position.X, (int)Client.Position.Z)) != null)
                    Client.Physics.Update();
                NextPhysicsUpdate = DateTime.Now.AddMilliseconds(1000 / 20);
            }
            UpdateKeyboard(gameTime, Keyboard.GetState());
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