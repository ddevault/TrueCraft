using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Utilities.Png;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.API.World;
using TrueCraft.Core;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.Client.Input;
using TrueCraft.Client.Interface;
using TrueCraft.Client.Modules;
using TrueCraft.Client.Rendering;

namespace TrueCraft.Client
{
    public class TrueCraftGame : Game
    {
        public MultiplayerClient Client { get; private set; }
        public GraphicsDeviceManager Graphics { get; private set; }
        public TextureMapper TextureMapper { get; private set; }
        public Camera Camera { get; private set; }
        public ConcurrentBag<Action> PendingMainThreadActions { get; set; }
        public double Bobbing { get; set; }

        private List<IGameplayModule> Modules { get; set; }
        private SpriteBatch SpriteBatch { get; set; }
        private KeyboardHandler KeyboardComponent { get; set; }
        private MouseHandler MouseComponent { get; set; }
        private RenderTarget2D RenderTarget { get; set; }

        private FontRenderer Pixel { get; set; }
        private IPEndPoint EndPoint { get; set; }
        private DateTime LastPhysicsUpdate { get; set; }
        private DateTime NextPhysicsUpdate { get; set; }
        private bool MouseCaptured { get; set; }
        private GameTime GameTime { get; set; }
        private Microsoft.Xna.Framework.Vector3 Delta { get; set; }

        public static readonly int Reach = 5;

        public IBlockRepository BlockRepository
        {
            get
            {
                return Client.World.World.BlockRepository;
            }
        }

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
            LastPhysicsUpdate = DateTime.MinValue;
            NextPhysicsUpdate = DateTime.MinValue;
            PendingMainThreadActions = new ConcurrentBag<Action>();
            MouseCaptured = true;
            Bobbing = 0;

            var keyboardComponent = new KeyboardHandler(this);
            KeyboardComponent = keyboardComponent;
            Components.Add(keyboardComponent);

            var mouseComponent = new MouseHandler(this);
            MouseComponent = mouseComponent;
            Components.Add(mouseComponent);
        }

        protected override void Initialize()
        {
            Modules = new List<IGameplayModule>();

            base.Initialize(); // (calls LoadContent)

            Modules.Add(new ChunkModule(this));
            Modules.Add(new HighlightModule(this));
            Modules.Add(new PlayerControlModule(this));

            Client.PropertyChanged += HandleClientPropertyChanged;
            Client.Connect(EndPoint);

            var centerX = GraphicsDevice.Viewport.Width / 2;
            var centerY = GraphicsDevice.Viewport.Height / 2;
            Mouse.SetPosition(centerX, centerY);

            Camera = new Camera(GraphicsDevice.Viewport.AspectRatio, 70.0f, 0.1f, 1000.0f);
            UpdateCamera();

            MouseComponent.Move += OnMouseComponentMove;
            KeyboardComponent.KeyDown += OnKeyboardKeyDown;
            KeyboardComponent.KeyUp += OnKeyboardKeyUp;

            Window.ClientSizeChanged += (sender, e) => CreateRenderTarget();
            CreateRenderTarget();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        private void CreateRenderTarget()
        {
            RenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,
                false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
        }

        void HandleClientPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Position":
                    UpdateCamera();
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

            Pixel = new FontRenderer(
                new Font(Content, "Fonts/Pixel"),
                new Font(Content, "Fonts/Pixel", FontStyle.Bold),
                null, // No support for underlined or strikethrough yet. The FontRenderer will revert to using the regular font style.
                null, // (I don't think BMFont has those options?)
                new Font(Content, "Fonts/Pixel", FontStyle.Italic));

            base.LoadContent();
        }

        private void OnKeyboardKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            foreach (var module in Modules)
            {
                var input = module as IInputModule;
                if (input != null)
                {
                    if (input.KeyDown(GameTime, e))
                        break;
                }
            }
        }

        private void OnKeyboardKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            foreach (var module in Modules)
            {
                var input = module as IInputModule;
                if (input != null)
                {
                    if (input.KeyUp(GameTime, e))
                        break;
                }
            }
        }

        private void OnMouseComponentMove(object sender, MouseMoveEventArgs e)
        {
            foreach (var module in Modules)
            {
                var input = module as IInputModule;
                if (input != null)
                    input.MouseMove(GameTime, e);
            }
        }

        public void TakeScreenshot()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                           ".truecraft", "screenshots", DateTime.Now.ToString("yyyy-MM-dd_H.mm.ss") + ".png");
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var stream = File.OpenWrite(path))
                new PngWriter().Write(RenderTarget, stream);
        }

        protected override void Update(GameTime gameTime)
        {
            GameTime = gameTime;

            Action action;
            if (PendingMainThreadActions.TryTake(out action))
                action();

            IChunk chunk;
            var adjusted = Client.World.World.FindBlockPosition(
                new Coordinates3D((int)Client.Position.X, 0, (int)Client.Position.Z), out chunk);
            if (chunk != null && Client.LoggedIn)
            {
                if (chunk.GetHeight((byte)adjusted.X, (byte)adjusted.Z) != 0)
                    Client.Physics.Update(gameTime.ElapsedGameTime);
            }
            if (NextPhysicsUpdate < DateTime.UtcNow && Client.LoggedIn)
            {
                // NOTE: This is to make the vanilla server send us chunk packets
                // We should eventually make some means of detecing that we're on a vanilla server to enable this
                // It's a waste of bandwidth to do it on a TrueCraft server
                Client.QueuePacket(new PlayerGroundedPacket { OnGround = true });
                Client.QueuePacket(new PlayerPositionAndLookPacket(Client.Position.X, Client.Position.Y,
                    Client.Position.Y + MultiplayerClient.Height, Client.Position.Z, Client.Yaw, Client.Pitch, false));
                NextPhysicsUpdate = DateTime.UtcNow.AddMilliseconds(1000 / 20);
            }

            foreach (var module in Modules)
                module.Update(gameTime);

            UpdateCamera();

            base.Update(gameTime);
        }

        private void UpdateCamera()
        {
            const double bobbingMultiplier = 0.015;

            var bobbing = Bobbing * 1.5;
            var xbob = Math.Cos(bobbing + Math.PI / 2) * bobbingMultiplier;
            var ybob = Math.Sin(Math.PI / 2 - (2 * bobbing)) * bobbingMultiplier;
            Camera.Position = new TrueCraft.API.Vector3(
                Client.Position.X + xbob - (Client.Size.Width / 2),
                Client.Position.Y + (Client.Size.Height - 0.5) + ybob,
                Client.Position.Z - (Client.Size.Depth / 2));

            Camera.Pitch = Client.Pitch;
            Camera.Yaw = Client.Yaw;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(RenderTarget);
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            foreach (var module in Modules)
            {
                var drawable = module as IGraphicalModule;
                if (drawable != null)
                    drawable.Draw(gameTime);
            }

            GraphicsDevice.SetRenderTarget(null);

            SpriteBatch.Begin();
            SpriteBatch.Draw(RenderTarget, new Vector2(0));
            SpriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                KeyboardComponent.Dispose();
                MouseComponent.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
