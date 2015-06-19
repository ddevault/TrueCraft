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
using TrueCraft.Client.Input;
using TrueCraft.Core;

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
        private List<Mesh> ChunkMeshes { get; set; }
        public ConcurrentBag<Action> PendingMainThreadActions { get; set; }
        private ConcurrentBag<Mesh> IncomingChunks { get; set; }
        public ChatInterface ChatInterface { get; set; }
        private RenderTarget2D RenderTarget;
        private BoundingFrustum CameraView;
        private Camera Camera;
        private bool MouseCaptured;
        private KeyboardComponent KeyboardComponent { get; set; }
        private MouseComponent MouseComponent { get; set; }
        private GameTime GameTime { get; set; }
        private Microsoft.Xna.Framework.Vector3 Delta { get; set; }
        private TextureMapper TextureMapper { get; set; }

        private BasicEffect OpaqueEffect;
        private AlphaTestEffect TransparentEffect;

        public TrueCraftGame(MultiplayerClient client, IPEndPoint endPoint)
        {
            Window.Title = "TrueCraft";
            Content.RootDirectory = "Content";
            Graphics = new GraphicsDeviceManager(this);
            Graphics.SynchronizeWithVerticalRetrace = false;
            Graphics.IsFullScreen = UserSettings.Local.IsFullscreen;
            Graphics.PreferredBackBufferWidth = UserSettings.Local.WindowResolution.Width;
            Graphics.PreferredBackBufferHeight = UserSettings.Local.WindowResolution.Height;
            Client = client;
            EndPoint = endPoint;
            NextPhysicsUpdate = DateTime.MinValue;
            ChunkMeshes = new List<Mesh>();
            IncomingChunks = new ConcurrentBag<Mesh>();
            PendingMainThreadActions = new ConcurrentBag<Action>();
            MouseCaptured = true;

            var keyboardComponent = new KeyboardComponent(this);
            KeyboardComponent = keyboardComponent;
            Components.Add(keyboardComponent);

            var mouseComponent = new MouseComponent(this);
            MouseComponent = mouseComponent;
            Components.Add(mouseComponent);
        }

        protected override void Initialize()
        {
            Interfaces = new List<IGameInterface>();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            base.Initialize(); // (calls LoadContent)
            ChunkConverter = new ChunkRenderer(this, Client.World.World.BlockRepository);
            Client.ChunkLoaded += (sender, e) => ChunkConverter.Enqueue(e.Chunk);
            Client.ChunkModified += (sender, e) => ChunkConverter.Enqueue(e.Chunk, true);
            ChunkConverter.MeshCompleted += ChunkConverter_MeshGenerated;
            ChunkConverter.Start();
            Client.PropertyChanged += HandleClientPropertyChanged;
            Client.Connect(EndPoint);
            var centerX = GraphicsDevice.Viewport.Width / 2;
            var centerY = GraphicsDevice.Viewport.Height / 2;
            Mouse.SetPosition(centerX, centerY);
            Camera = new Camera(GraphicsDevice.Viewport.AspectRatio, 70.0f, 0.25f, 1000.0f);
            UpdateCamera();
            Window.ClientSizeChanged += (sender, e) => CreateRenderTarget();
            MouseComponent.Move += OnMouseComponentMove;
            KeyboardComponent.KeyDown += OnKeyboardKeyDown;
            KeyboardComponent.KeyUp += OnKeyboardKeyUp;
            CreateRenderTarget();
        }

        private void CreateRenderTarget()
        {
            RenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
                false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
        }

        void ChunkConverter_MeshGenerated(object sender, RendererEventArgs<ReadOnlyChunk> e)
        {
            IncomingChunks.Add(e.Result);
        }

        void HandleClientPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Position":
                    UpdateCamera();
                    var sorter = new ChunkRenderer.ChunkSorter(new Coordinates3D(
                        (int)Client.Position.X, 0, (int)Client.Position.Z));
                    PendingMainThreadActions.Add(() => ChunkMeshes.Sort(sorter));
                    break;
            }
        }

        protected override void LoadContent()
        {
            // Ensure we have default textures loaded.
            TextureMapper.LoadDefaults(GraphicsDevice);

            // Load any custom textures if needed.
            TextureMapper = new TextureMapper(GraphicsDevice);
            if (UserSettings.Local.SelectedTexturePack != TexturePack.Default.Name)
                TextureMapper.AddTexturePack(TexturePack.FromArchive(Path.Combine(TexturePack.TexturePackPath, UserSettings.Local.SelectedTexturePack)));

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
            OpaqueEffect.Texture = TextureMapper.GetTexture("terrain.png");
            OpaqueEffect.FogEnabled = true;
            OpaqueEffect.FogStart = 512f;
            OpaqueEffect.FogEnd = 1000f;
            OpaqueEffect.FogColor = Color.CornflowerBlue.ToVector3();

            TransparentEffect = new AlphaTestEffect(GraphicsDevice);
            TransparentEffect.AlphaFunction = CompareFunction.Greater;
            TransparentEffect.ReferenceAlpha = 127;
            TransparentEffect.Texture = TextureMapper.GetTexture("terrain.png");

            base.LoadContent();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            ChunkConverter.Stop();
            base.OnExiting(sender, args);
        }

        private void OnKeyboardKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            // TODO: Rebindable keys
            // TODO: Horizontal terrain collisions

            switch (e.Key)
            {
                // Quit the game.
                case Keys.Escape:
                    Exit();
                    break;

                // Take a screenshot.
                case Keys.F2:
                    TakeScreenshot();
                    break;

                // Move to the left.
                case Keys.A:
                case Keys.Left:
                    Delta += Microsoft.Xna.Framework.Vector3.Left;
                    break;

                // Move to the right.
                case Keys.D:
                case Keys.Right:
                    Delta += Microsoft.Xna.Framework.Vector3.Right;
                    break;

                // Move forwards.
                case Keys.W:
                case Keys.Up:
                    Delta += Microsoft.Xna.Framework.Vector3.Forward;
                    break;

                // Move backwards.
                case Keys.S:
                case Keys.Down:
                    Delta += Microsoft.Xna.Framework.Vector3.Backward;
                    break;

                // Toggle mouse capture.
                case Keys.Tab:
                    MouseCaptured = !MouseCaptured;
                    break;
            }

        }

        private void OnKeyboardKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                // Stop moving to the left.
                case Keys.A:
                case Keys.Left:
                    Delta -= Microsoft.Xna.Framework.Vector3.Left;
                    break;

                // Stop moving to the right.
                case Keys.D:
                case Keys.Right:
                    Delta -= Microsoft.Xna.Framework.Vector3.Right;
                    break;

                // Stop moving forwards.
                case Keys.W:
                case Keys.Up:
                    Delta -= Microsoft.Xna.Framework.Vector3.Forward;
                    break;

                // Stop moving backwards.
                case Keys.S:
                case Keys.Down:
                    Delta -= Microsoft.Xna.Framework.Vector3.Backward;
                    break;
            }
        }

        private void OnMouseComponentMove(object sender, MouseMoveEventArgs e)
        {
            if (MouseCaptured)
            {
                var centerX = GraphicsDevice.Viewport.Width / 2;
                var centerY = GraphicsDevice.Viewport.Height / 2;
                Mouse.SetPosition(centerX, centerY);

                var look = new Vector2((centerX - e.X), (centerY - e.Y))
                    * (float)(GameTime.ElapsedGameTime.TotalSeconds * 70);

                Client.Yaw += look.X;
                Client.Pitch += look.Y;
                Client.Yaw %= 360;
                Client.Pitch = Microsoft.Xna.Framework.MathHelper.Clamp(Client.Pitch, -89.9f, 89.9f);

                if (look != Vector2.Zero)
                    UpdateCamera();
            }
        }

        private void TakeScreenshot()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                       ".truecraft", "screenshots", DateTime.Now.ToString("yyyy-MM-dd_H.mm.ss") + ".png");
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var stream = File.OpenWrite(path))
                RenderTarget.SaveAsPng(stream, RenderTarget.Width, RenderTarget.Height);
            ChatInterface.AddMessage(string.Format("Screenshot saved as {0}", Path.GetFileName(path)));
        }

        protected override void Update(GameTime gameTime)
        {
            GameTime = gameTime;

            foreach (var i in Interfaces)
            {
                i.Update(gameTime);
            }

            Mesh mesh;
            if (IncomingChunks.TryTake(out mesh))
            {
                ChunkMeshes.Add(mesh);
            }
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

            if (Delta != Microsoft.Xna.Framework.Vector3.Zero)
            {
                var lookAt = Microsoft.Xna.Framework.Vector3.Transform(
                             Delta, Matrix.CreateRotationY(Microsoft.Xna.Framework.MathHelper.ToRadians(Client.Yaw)));

                Client.Position += new TrueCraft.API.Vector3(lookAt.X, lookAt.Y, lookAt.Z)
                    * (gameTime.ElapsedGameTime.TotalSeconds * 4.3717);
            }

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

        private static readonly BlendState ColorWriteDisable = new BlendState()
        {
            ColorWriteChannels = ColorWriteChannels.None
        };

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(RenderTarget);
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            int verticies = 0, chunks = 0;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            for (int i = 0; i < ChunkMeshes.Count; i++)
            {
                if (CameraView.Intersects(ChunkMeshes[i].BoundingBox))
                {
                    verticies += ChunkMeshes[i].GetTotalVertices();
                    chunks++;
                    ChunkMeshes[i].Draw(OpaqueEffect, 0);
                }
            }

            GraphicsDevice.BlendState = ColorWriteDisable;
            for (int i = 0; i < ChunkMeshes.Count; i++)
            {
                if (CameraView.Intersects(ChunkMeshes[i].BoundingBox))
                {
                    if (!ChunkMeshes[i].IsDisposed)
                        verticies += ChunkMeshes[i].GetTotalVertices();
                    ChunkMeshes[i].Draw(TransparentEffect, 1);
                }
            }

            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            for (int i = 0; i < ChunkMeshes.Count; i++)
            {
                if (CameraView.Intersects(ChunkMeshes[i].BoundingBox))
                {
                    if (!ChunkMeshes[i].IsDisposed)
                        verticies += ChunkMeshes[i].GetTotalVertices();
                    ChunkMeshes[i].Draw(TransparentEffect, 1);
                }
            }

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ChunkConverter.Dispose();

                KeyboardComponent.Dispose();
                MouseComponent.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
