using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.Client.Rendering;
using TrueCraft.Core.Logic.Items;
using TrueCraft.API.Logic;
using TrueCraft.Client.Input;
using Microsoft.Xna.Framework.Input;
using TrueCraft.Core.Networking.Packets;

namespace TrueCraft.Client.Modules
{
    public class WindowModule : InputModule, IGraphicalModule
    {
        private TrueCraftGame Game { get; set; }
        private SpriteBatch SpriteBatch { get; set; }
        private Texture2D Inventory { get; set; }
        private Texture2D Items { get; set; }
        private FontRenderer Font { get; set; }
        private Texture2D Background { get; set; }

        public WindowModule(TrueCraftGame game, FontRenderer font)
        {
            Game = game;
            Font = font;
            SpriteBatch = new SpriteBatch(game.GraphicsDevice);
            Inventory = game.TextureMapper.GetTexture("gui/inventory.png");
            Items = game.TextureMapper.GetTexture("gui/items.png");
            Background = new Texture2D(game.GraphicsDevice, 1, 1);
            Background.SetData<Color>(new[] { new Color(Color.Black, 180) });
        }

        private static readonly Rectangle InventoryWindowRect = new Rectangle(0, 0, 176, 166);

        public void Draw(GameTime gameTime)
        {
            if (Game.Client.CurrentWindow != null)
            {
                SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied);
                SpriteBatch.Draw(Background, new Rectangle(0, 0,
                    Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
                switch (Game.Client.CurrentWindow.Type)
                {
                    case -1:
                        DrawInventoryWindow();
                        break;
                }
                SpriteBatch.End();
            }
        }

        public override bool MouseMove(GameTime gameTime, MouseMoveEventArgs e)
        {
            if (Game.Client.CurrentWindow != null)
                return true;
            return base.MouseMove(gameTime, e);
        }

        public override bool KeyDown(GameTime gameTime, KeyboardKeyEventArgs e)
        {
            if (Game.Client.CurrentWindow != null)
            {
                if (e.Key == Keys.Escape)
                {
                    if (Game.Client.CurrentWindow.Type != -1)
                        Game.Client.QueuePacket(new CloseWindowPacket(Game.Client.CurrentWindow.ID));
                    Game.Client.CurrentWindow = null;
                    Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
                }
                return true;
            }
            return base.KeyDown(gameTime, e);
        }

        private void DrawInventoryWindow()
        {
            SpriteBatch.Draw(Inventory, new Vector2(
                Game.GraphicsDevice.Viewport.Width / 2 - Scale(InventoryWindowRect.Width / 2),
                Game.GraphicsDevice.Viewport.Height / 2 - Scale(InventoryWindowRect.Height / 2)),
                InventoryWindowRect, Color.White, 0, Vector2.Zero, Game.ScaleFactor * 2, SpriteEffects.None, 1);
        }

        public override void Update(GameTime gameTime)
        {
            Game.IsMouseVisible = Game.Client.CurrentWindow != null;
            base.Update(gameTime);
        }

        /// <summary>
        /// Scales a float depending on the game's scale factor.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private float Scale(float value)
        {
            return value * Game.ScaleFactor * 2;
        }
    }
}
