using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrueCraft.Client.Modules
{
    public class HUDModule : IGraphicalModule
    {
        private TrueCraftGame Game { get; set; }
        private SpriteBatch SpriteBatch { get; set; }
        private Texture2D GUI { get; set; }
        private Texture2D Icons { get; set; }

        public HUDModule(TrueCraftGame game)
        {
            Game = game;
            SpriteBatch = new SpriteBatch(game.GraphicsDevice);
            GUI = game.TextureMapper.GetTexture("gui/gui.png");
            Icons = game.TextureMapper.GetTexture("gui/icons.png");
        }

        public void Update(GameTime gameTime)
        {
        }

        static readonly Color CrosshairColor = new Color(255, 255, 255, 70);

        public void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.NonPremultiplied);
            
            SpriteBatch.Draw(Icons, new Vector2(
                Game.GraphicsDevice.Viewport.Width / 2 - (8 * Game.ScaleFactor * 2),
                Game.GraphicsDevice.Viewport.Height / 2 - (8 * Game.ScaleFactor * 2)),
                new Rectangle(0, 0, 16, 16), CrosshairColor,
                0, Vector2.Zero, Game.ScaleFactor * 2, SpriteEffects.None, 1);

            DrawHotbar(gameTime);

            SpriteBatch.End();
        }

        #region "Hotbar"

        /// <summary>
        /// The dimensions of the hotbar background.
        /// </summary>
        private static readonly Rectangle HotbarBackgroundRect =
            new Rectangle(0, 0, 182, 22);

        /// <summary>
        /// The dimensions of the hotbar selection.
        /// </summary>
        private static readonly Rectangle HotbarSelectionRect =
            new Rectangle(0, 22, 24, 24);

        /// <summary>
        /// Draws the inventory hotbar.
        /// </summary>
        /// <param name="gameTime"></param>
        private void DrawHotbar(GameTime gameTime)
        {
            // Background
            SpriteBatch.Draw(GUI, new Vector2(
                Game.GraphicsDevice.Viewport.Width / 2 - Scale(HotbarBackgroundRect.Width / 2),
                Game.GraphicsDevice.Viewport.Height - Scale(HotbarBackgroundRect.Height + 5)),
                HotbarBackgroundRect, Color.White, 0, Vector2.Zero, Game.ScaleFactor * 2, SpriteEffects.None, 1);

            // TODO: Icons

            // Selection
            SpriteBatch.Draw(GUI, new Vector2(
                Game.GraphicsDevice.Viewport.Width / 2 - Scale(HotbarBackgroundRect.Width / 2) + Scale(Game.Client.HotbarSelection * 20 - 1),
                Game.GraphicsDevice.Viewport.Height - Scale(HotbarBackgroundRect.Height + 6)),
                HotbarSelectionRect, Color.White, 0, Vector2.Zero, Game.ScaleFactor * 2, SpriteEffects.None, 1);
        }

        #endregion

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
