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
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            SpriteBatch.Draw(Icons, new Vector2(
                Game.GraphicsDevice.Viewport.Width / 2 - (8 * Game.ScaleFactor * 2),
                Game.GraphicsDevice.Viewport.Height / 2 - (8 * Game.ScaleFactor * 2)),
                new Rectangle(0, 0, 16, 16), CrosshairColor,
                0, Vector2.Zero, Game.ScaleFactor * 2, SpriteEffects.None, 1);
            SpriteBatch.End();
        }
    }
}
