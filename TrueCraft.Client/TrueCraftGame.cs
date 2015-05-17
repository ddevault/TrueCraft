using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TrueCraft.Client.Interface;
using System.IO;
using System.Net;
using TrueCraft.API;
using TrueCraft.Client.Rendering;
using System.Linq;
using System.ComponentModel;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API.World;

namespace TrueCraft.Client
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
        private List<Mesh> TransparentChunkMeshes { get; set; }
        private readonly object ChunkMeshesLock = new object();
        private Matrix Camera;
        private Matrix Perspective;
        private BoundingFrustum CameraView;
        private bool MouseCaptured;
        private KeyboardState PreviousKeyboardState;

        private BasicEffect OpaqueEffect, TransparentEffect;

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
            TransparentChunkMeshes = new List<Mesh>();
            MouseCaptured = true;
        }

        protected override void Initialize()
        {
            Interfaces = new List<IGameInterface>();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            base.Initialize(); // (calls LoadContent)
            ChunkConverter = new ChunkConverter(Graphics.GraphicsDevice, Client.World.World.BlockRepository);
            Client.ChunkLoaded += (sender, e) => ChunkConverter.QueueChunk(e.Chunk);
            ChunkConverter.Start((opaque, transparent) =>
            {
                lock (ChunkMeshesLock)
                {
                    ChunkMeshes.Add(opaque);
                    TransparentChunkMeshes.Add(transparent);
                }
            });
            Client.PropertyChanged += HandleClientPropertyChanged;
            Client.Connect(EndPoint);
            var centerX = GraphicsDevice.Viewport.Width / 2;
            var centerY = GraphicsDevice.Viewport.Height / 2;
            Mouse.SetPosition(centerX, centerY);
            UpdateMatricies();
            PreviousKeyboardState = Keyboard.GetState();
        }

        void HandleClientPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Position":
                    UpdateMatricies();
                    var sorter = new ChunkConverter.ChunkSorter(new Coordinates3D(
                        (int)Client.Position.X, 0, (int)Client.Position.Z));
                    lock (ChunkMeshesLock)
                        TransparentChunkMeshes.Sort(sorter);
                    break;
            }
        }

        protected override void LoadContent()
        {
            FontFile fontFile;
            using (var f = File.OpenRead(Path.Combine(Content.RootDirectory, "dejavu.fnt")))
                fontFile = FontLoader.Load(f);
            var fontTexture = Content.Load<Texture2D>("dejavu_0.png");
            DejaVu = new FontRenderer(fontFile, fontTexture);
            Interfaces.Add(new ChatInterface(Client, DejaVu));

            OpaqueEffect = new BasicEffect(GraphicsDevice);
            OpaqueEffect.EnableDefaultLighting();
            OpaqueEffect.DirectionalLight0.SpecularColor = Color.Black.ToVector3();
            OpaqueEffect.DirectionalLight1.SpecularColor = Color.Black.ToVector3();
            OpaqueEffect.DirectionalLight2.SpecularColor = Color.Black.ToVector3();
            OpaqueEffect.TextureEnabled = true;
            OpaqueEffect.Texture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/terrain.png"));
            OpaqueEffect.FogEnabled = true;
            OpaqueEffect.FogStart = 512f;
            OpaqueEffect.FogEnd = 1000f;
            OpaqueEffect.FogColor = Color.CornflowerBlue.ToVector3();

            TransparentEffect = new BasicEffect(GraphicsDevice);
            TransparentEffect.TextureEnabled = true;
            TransparentEffect.Texture = Texture2D.FromStream(GraphicsDevice, File.OpenRead("Content/terrain.png"));

            base.LoadContent();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            ChunkConverter.Stop();
            base.OnExiting(sender, args);
        }

        protected virtual void UpdateKeyboard(GameTime gameTime, KeyboardState state, KeyboardState oldState)
        {
            if (state.IsKeyDown(Keys.Escape))
                Exit();
            // TODO: Rebindable keys
            // TODO: Horizontal terrain collisions
            Microsoft.Xna.Framework.Vector3 delta = Microsoft.Xna.Framework.Vector3.Zero;
            if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
                delta += Microsoft.Xna.Framework.Vector3.Left;
            if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
                delta += Microsoft.Xna.Framework.Vector3.Right;
            if (state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.W))
                delta += Microsoft.Xna.Framework.Vector3.Forward;
            if (state.IsKeyDown(Keys.Down) || state.IsKeyDown(Keys.S))
                delta += Microsoft.Xna.Framework.Vector3.Backward;
            
            if (state.IsKeyUp(Keys.Tab) && oldState.IsKeyDown(Keys.Tab))
                MouseCaptured = !MouseCaptured;

            var lookAt = Microsoft.Xna.Framework.Vector3.Transform(
                 delta, Matrix.CreateRotationY(MathHelper.ToRadians(Client.Yaw)));

            Client.Position += new TrueCraft.API.Vector3(lookAt.X, lookAt.Y, lookAt.Z) * (gameTime.ElapsedGameTime.TotalSeconds * 4.3717);

            if (MouseCaptured)
            {
                var centerX = GraphicsDevice.Viewport.Width / 2;
                var centerY = GraphicsDevice.Viewport.Height / 2;
                var mouse = Mouse.GetState();
                var look = new Vector2(centerX - mouse.Position.X, centerY - mouse.Position.Y)
                       * (float)(gameTime.ElapsedGameTime.TotalSeconds * 70);
                Mouse.SetPosition(centerX, centerY);
                Client.Yaw += look.X;
                Client.Pitch += look.Y;
                Client.Yaw %= 360;
                Client.Pitch = MathHelper.Clamp(Client.Pitch, -90, 90);

                if (look != Vector2.Zero)
                    UpdateMatricies();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            foreach (var i in Interfaces)
            {
                i.Update(gameTime);
            }
            if (NextPhysicsUpdate < DateTime.Now)
            {
                IChunk chunk;
                var adjusted = Client.World.World.FindBlockPosition(new Coordinates3D((int)Client.Position.X, 0, (int)Client.Position.Z), out chunk);
                if (Client.LoggedIn && chunk != null)
                {
                    if (chunk.GetHeight((byte)adjusted.X, (byte)adjusted.Z) != 0)
                        Client.Physics.Update();
                }
                if (Client.LoggedIn)
                {
                    // NOTE: This is to make the vanilla server send us chunk packets
                    // We should eventually make some means of detecing that we're on a vanilla server to enable this
                    // It's a waste of bandwidth to do it on a TrueCraft server
                    Console.WriteLine("Sending position packet");
                    Client.QueuePacket(new PlayerGroundedPacket { OnGround = true });
                    Client.QueuePacket(new PlayerPositionAndLookPacket(Client.Position.X, Client.Position.Y,
                        Client.Position.Y + MultiplayerClient.Height, Client.Position.Z, Client.Yaw, Client.Pitch, false));
                }
                NextPhysicsUpdate = DateTime.Now.AddMilliseconds(1000 / 20);
            }
            var state = Keyboard.GetState();
            UpdateKeyboard(gameTime, state, PreviousKeyboardState);
            PreviousKeyboardState = state;
            base.Update(gameTime);
        }

        private void UpdateMatricies()
        {
            var player = new Microsoft.Xna.Framework.Vector3(
                (float)Client.Position.X,
                (float)(Client.Position.Y + (Client.Size.Height / 2)),
                (float)Client.Position.Z);

            var lookAt = Microsoft.Xna.Framework.Vector3.Transform(
                new Microsoft.Xna.Framework.Vector3(0, 0, -1),
                Matrix.CreateRotationX(MathHelper.ToRadians(Client.Pitch)) * Matrix.CreateRotationY(MathHelper.ToRadians(Client.Yaw)));

            Camera = Matrix.CreateLookAt(
                player, player + lookAt,
                Microsoft.Xna.Framework.Vector3.Up);

            Perspective = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70f), GraphicsDevice.Viewport.AspectRatio, 0.01f, 1000f);

            CameraView = new BoundingFrustum(Camera * Perspective);
        }

        protected override void Draw(GameTime gameTime)
        {
            Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            OpaqueEffect.View = TransparentEffect.View = Camera;
            OpaqueEffect.Projection = TransparentEffect.Projection = Perspective;
            OpaqueEffect.World = TransparentEffect.World = Matrix.Identity;
            int verticies = 0, chunks = 0;
            lock (ChunkMeshesLock)
            {
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                foreach (var chunk in ChunkMeshes)
                {
                    if (CameraView.Intersects(chunk.BoundingBox))
                    {
                        verticies += chunk.Verticies.VertexCount;
                        chunks++;
                        chunk.Draw(OpaqueEffect);
                    }
                }
                GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                foreach (var chunk in TransparentChunkMeshes)
                {
                    if (CameraView.Intersects(chunk.BoundingBox))
                    {
                        if (chunk.Verticies != null)
                            verticies += chunk.Verticies.VertexCount;
                        chunk.Draw(TransparentEffect);
                    }
                }
            }
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            SpriteBatch.Begin();
            foreach (var i in Interfaces)
            {
                i.DrawSprites(gameTime, SpriteBatch);
            }

            int fps = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);
            DejaVu.DrawText(SpriteBatch, 0, GraphicsDevice.Viewport.Height - 30,
                string.Format("{0} FPS, {1} verticies, {2} chunks", fps + 1, verticies, chunks));
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}