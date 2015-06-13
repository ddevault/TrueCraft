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
using System.Collections.Concurrent;

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
        private ChunkRenderer ChunkConverter { get; set; }
        private DateTime NextPhysicsUpdate { get; set; }
        private List<ChunkMesh> ChunkMeshes { get; set; }
        private ConcurrentBag<Action> PendingMainThreadActions { get; set; }
        private ConcurrentBag<ChunkMesh> IncomingChunks { get; set; }
        private ConcurrentBag<ChunkMesh> IncomingTransparentChunks { get; set; }
        private List<ChunkMesh> TransparentChunkMeshes { get; set; }
        public ChatInterface ChatInterface { get; set; }
        private RenderTarget2D RenderTarget;
        private BoundingFrustum CameraView;
        private Camera Camera;
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
            ChunkMeshes = new List<ChunkMesh>();
            TransparentChunkMeshes = new List<ChunkMesh>();
            IncomingChunks = new ConcurrentBag<ChunkMesh>();
            IncomingTransparentChunks = new ConcurrentBag<ChunkMesh>();
            PendingMainThreadActions = new ConcurrentBag<Action>();
            MouseCaptured = true;
        }

        protected override void Initialize()
        {
            Interfaces = new List<IGameInterface>();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            base.Initialize(); // (calls LoadContent)
            ChunkConverter = new ChunkRenderer(Graphics.GraphicsDevice, Client.World.World.BlockRepository);
            Client.ChunkLoaded += (sender, e) => ChunkConverter.QueueChunk(e.Chunk);
            Client.ChunkModified += (sender, e) => ChunkConverter.QueueHighPriorityChunk(e.Chunk);
            ChunkConverter.MeshGenerated += ChunkConverter_MeshGenerated;
            ChunkConverter.Start();
            Client.PropertyChanged += HandleClientPropertyChanged;
            Client.Connect(EndPoint);
            var centerX = GraphicsDevice.Viewport.Width / 2;
            var centerY = GraphicsDevice.Viewport.Height / 2;
            Mouse.SetPosition(centerX, centerY);
            Camera = new Camera(GraphicsDevice.Viewport.AspectRatio, 70.0f, 0.1f, 1000.0f);
            UpdateCamera();
            PreviousKeyboardState = Keyboard.GetState();
            Window.ClientSizeChanged += (sender, e) => CreateRenderTarget();
            CreateRenderTarget();
        }

        private void CreateRenderTarget()
        {
            RenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
                false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
        }

        void ChunkConverter_MeshGenerated(object sender, ChunkRenderer.MeshGeneratedEventArgs e)
        {
            if (e.Transparent)
                IncomingTransparentChunks.Add(e.Mesh);
            else
                IncomingChunks.Add(e.Mesh);
        }

        void HandleClientPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Position":
                    UpdateCamera();
                    var sorter = new ChunkRenderer.ChunkSorter(new Coordinates3D(
                        (int)Client.Position.X, 0, (int)Client.Position.Z));
                    PendingMainThreadActions.Add(() => TransparentChunkMeshes.Sort(sorter));
                    break;
            }
        }

        protected override void LoadContent()
        {
            DejaVu = new FontRenderer(
                new Font(Content, "Fonts/DejaVu", FontStyle.Regular),
                new Font(Content, "Fonts/DejaVu", FontStyle.Bold),
                null, // No support for underlined or strikethrough yet. The FontRenderer will revert to using the regular font style.
                null, // (I don't think BMFont has those options?)
                new Font(Content, "Fonts/DejaVu", FontStyle.Italic));
            Interfaces.Add(ChatInterface = new ChatInterface(Client, DejaVu));

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

            if (state.IsKeyDown(Keys.F2) && oldState.IsKeyUp(Keys.F2)) // Take a screenshot
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".truecraft", "screenshots", DateTime.Now.ToString("yyyy-MM-dd_H.mm.ss") + ".png");
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                using (var stream = File.OpenWrite(path))
                    RenderTarget.SaveAsPng(stream, RenderTarget.Width, RenderTarget.Height);
                ChatInterface.AddMessage(string.Format("Screenshot saved as {0}", Path.GetFileName(path)));
            }

            Microsoft.Xna.Framework.Vector3 delta = Microsoft.Xna.Framework.Vector3.Zero;

            if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
                delta += Microsoft.Xna.Framework.Vector3.Left;
            if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
                delta += Microsoft.Xna.Framework.Vector3.Right;
            if (state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.W))
                delta += Microsoft.Xna.Framework.Vector3.Forward;
            if (state.IsKeyDown(Keys.Down) || state.IsKeyDown(Keys.S))
                delta += Microsoft.Xna.Framework.Vector3.Backward;
            
            if (delta != Microsoft.Xna.Framework.Vector3.Zero)
            {
                var lookAt = Microsoft.Xna.Framework.Vector3.Transform(
                             delta, Matrix.CreateRotationY(MathHelper.ToRadians(Client.Yaw)));

                Client.Position += new TrueCraft.API.Vector3(lookAt.X, lookAt.Y, lookAt.Z) * (gameTime.ElapsedGameTime.TotalSeconds * 4.3717);
            }

            if (state.IsKeyUp(Keys.Tab) && oldState.IsKeyDown(Keys.Tab))
                MouseCaptured = !MouseCaptured;
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
                Client.Pitch = MathHelper.Clamp(Client.Pitch, -89.9f, 89.9f);

                if (look != Vector2.Zero)
                    UpdateCamera();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            foreach (var i in Interfaces)
            {
                i.Update(gameTime);
            }

            ChunkMesh mesh;
            if (IncomingChunks.TryTake(out mesh))
            {
                var existing = ChunkMeshes.SingleOrDefault(m => m.Chunk.Chunk.Coordinates == mesh.Chunk.Chunk.Coordinates);
                if (existing != null)
                    ChunkMeshes.Remove(existing);
                ChunkMeshes.Add(mesh);
            }
            if (IncomingTransparentChunks.TryTake(out mesh)) // TODO: re-render transparent meshes
                TransparentChunkMeshes.Add(mesh);
            Action action;
            if (PendingMainThreadActions.TryTake(out action))
                action();

            if (NextPhysicsUpdate < DateTime.Now && Client.LoggedIn)
            {
                IChunk chunk;
                var adjusted = Client.World.World.FindBlockPosition(new Coordinates3D((int)Client.Position.X, 0, (int)Client.Position.Z), out chunk);
                if (chunk != null)
                {
                    if (chunk.GetHeight((byte)adjusted.X, (byte)adjusted.Z) != 0)
                        Client.Physics.Update();
                }
                // NOTE: This is to make the vanilla server send us chunk packets
                // We should eventually make some means of detecing that we're on a vanilla server to enable this
                // It's a waste of bandwidth to do it on a TrueCraft server
                Client.QueuePacket(new PlayerGroundedPacket { OnGround = true });
                Client.QueuePacket(new PlayerPositionAndLookPacket(Client.Position.X, Client.Position.Y,
                    Client.Position.Y + MultiplayerClient.Height, Client.Position.Z, Client.Yaw, Client.Pitch, false));
                NextPhysicsUpdate = DateTime.Now.AddMilliseconds(1000 / 20);
            }
            var state = Keyboard.GetState();
            UpdateKeyboard(gameTime, state, PreviousKeyboardState);
            PreviousKeyboardState = state;
            base.Update(gameTime);
        }

        private void UpdateCamera()
        {
            Camera.Position = new TrueCraft.API.Vector3(
                Client.Position.X,
                Client.Position.Y + (Client.Size.Height / 2),
                Client.Position.Z);

            Camera.Pitch = Client.Pitch;
            Camera.Yaw = Client.Yaw;

            CameraView = Camera.GetFrustum();

            Camera.ApplyTo(OpaqueEffect);
            Camera.ApplyTo(TransparentEffect);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(RenderTarget);

            Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;

            int verticies = 0, chunks = 0;
            GraphicsDevice.DepthStencilState = new DepthStencilState { DepthBufferEnable = true };
            for (int i = 0; i < ChunkMeshes.Count; i++)
            {
                if (CameraView.Intersects(ChunkMeshes[i].BoundingBox) && !ChunkMeshes[i].Empty)
                {
                    verticies += ChunkMeshes[i].Verticies.VertexCount;
                    chunks++;
                    ChunkMeshes[i].Draw(OpaqueEffect);
                }
            }
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            for (int i = 0; i < TransparentChunkMeshes.Count; i++)
            {
                if (CameraView.Intersects(TransparentChunkMeshes[i].BoundingBox) && !TransparentChunkMeshes[i].Empty)
                {
                    if (TransparentChunkMeshes[i].Verticies != null)
                        verticies += TransparentChunkMeshes[i].Verticies.VertexCount;
                    TransparentChunkMeshes[i].Draw(TransparentEffect);
                }
            }
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            SpriteBatch.Begin();
            for (int i = 0; i < Interfaces.Count; i++)
            {
                Interfaces[i].DrawSprites(gameTime, SpriteBatch);
            }

            int fps = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);
            DejaVu.DrawText(SpriteBatch, 0, GraphicsDevice.Viewport.Height - 30,
                string.Format("{0} FPS, {1} verticies, {2} chunks, {3}", fps + 1, verticies, chunks, Client.Position));
            SpriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.CornflowerBlue);
            SpriteBatch.Begin();
            SpriteBatch.Draw(RenderTarget, new Vector2(0));
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
