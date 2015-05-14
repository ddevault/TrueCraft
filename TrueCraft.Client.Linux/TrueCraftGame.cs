using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TrueCraft.Client.Linux.Interface;
using System.IO;
using System.Net;
using TrueCraft.API;
using TrueCraft.Client.Linux.Rendering;

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
        private List<Mesh> ChunkMeshes { get; set; }
        private object ChunkMeshesLock = new object();

        private BasicEffect effect;

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
            NextPhysicsUpdate = DateTime.MinValue;
            ChunkMeshes = new List<Mesh>();
        }

        protected override void Initialize()
        {
            Interfaces = new List<IGameInterface>();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            base.Initialize(); // (calls LoadContent)
            ChunkConverter = new ChunkConverter(Graphics.GraphicsDevice, Client.World.World.BlockRepository);
            Client.ChunkLoaded += (sender, e) => ChunkConverter.QueueChunk(e.Chunk);
            ChunkConverter.Start(mesh =>
            {
                lock (ChunkMeshesLock)
                    ChunkMeshes.Add(mesh);
            });
            Client.Connect(EndPoint);
            var centerX = GraphicsDevice.Viewport.Width / 2;
            var centerY = GraphicsDevice.Viewport.Height / 2;
            Mouse.SetPosition(centerX, centerY);
        }

        protected override void LoadContent()
        {
            FontFile fontFile;
            using (var f = File.OpenRead(Path.Combine(Content.RootDirectory, "dejavu.fnt")))
                fontFile = FontLoader.Load(f);
            var fontTexture = Content.Load<Texture2D>("dejavu_0.png");
            DejaVu = new FontRenderer(fontFile, fontTexture);
            Interfaces.Add(new ChatInterface(Client, DejaVu));
            effect = new BasicEffect(GraphicsDevice);
            effect.TextureEnabled = true;
            effect.Texture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/terrain.png"));
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

            var centerX = GraphicsDevice.Viewport.Width / 2;
            var centerY = GraphicsDevice.Viewport.Height / 2;
            var mouse = Mouse.GetState();
            var look = new Vector2(centerX - mouse.Position.X, centerY - mouse.Position.Y) * 0.2f; // TODO: fewer magic numbers
            Mouse.SetPosition(centerX, centerY);
            Client.Yaw += look.X;
            Client.Pitch += look.Y;
            Client.Yaw %= 360;
            Client.Pitch %= 360;
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
            // TODO: Move camera logic elsewhere
            var player = new Microsoft.Xna.Framework.Vector3(
                (float)Client.Position.X,
                (float)Client.Position.Y,
                (float)Client.Position.Z);

            var lookAt = Microsoft.Xna.Framework.Vector3.Transform(
                new Microsoft.Xna.Framework.Vector3(0, 0, -1),
                Matrix.CreateRotationX(MathHelper.ToRadians(Client.Pitch)) * Matrix.CreateRotationY(MathHelper.ToRadians(Client.Yaw)));

            var cameraMatrix = Matrix.CreateLookAt(
                player, player + lookAt,
                Microsoft.Xna.Framework.Vector3.Up);

            var projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60f), GraphicsDevice.Viewport.AspectRatio, 0.3f, 10000f);

            Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            Graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            effect.View = cameraMatrix;
            effect.Projection = projectionMatrix;
            effect.World = Matrix.CreateTranslation(Microsoft.Xna.Framework.Vector3.Zero);
            lock (ChunkMeshesLock)
            {
                foreach (var chunk in ChunkMeshes)
                    chunk.Draw(effect);
            }

            SpriteBatch.Begin();
            foreach (var i in Interfaces)
            {
                i.DrawSprites(gameTime, SpriteBatch);
            }
            DejaVu.DrawText(SpriteBatch, 0, 500, string.Format("X: {0}, Y: {1}, Z: {2}", Client.Position.X, Client.Position.Y, Client.Position.Z));
            DejaVu.DrawText(SpriteBatch, 0, 530, string.Format("Yaw: {0}, Pitch: {1}", Client.Yaw, Client.Pitch));
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}