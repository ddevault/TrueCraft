using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TrueCraft.Client.Input;
using TrueCraft.Client.Rendering;

namespace TrueCraft.Client.Modules
{
    public class DebugInfoModule : IGraphicalModule, IInputModule
    {
        public bool Chunks { get; set; }

        private TrueCraftGame Game { get; set; }
        private FontRenderer Font { get; set; }
        private SpriteBatch SpriteBatch { get; set; }
        private bool Enabled { get; set; }

        public DebugInfoModule(TrueCraftGame game, FontRenderer font)
        {
            Game = game;
            Font = font;
            SpriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public bool KeyDown(GameTime gameTime, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.F3:
                    return true;
            }
            return false;
        }

        public bool KeyUp(GameTime gameTime, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.F3:
                    Enabled = !Enabled;
                    return true;
            }
            return false;
        }

        public void MouseMove(GameTime gameTime, MouseMoveEventArgs e)
        {
        }

        public void Update(GameTime gameTime)
        {
        }
        
        public void Draw(GameTime gameTime)
        {
            if (!Enabled)
                return;

            var fps = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds) + 1;

            const int xOrigin = 10;
            const int yOrigin = 5;
            const int yOffset = 25;

            SpriteBatch.Begin();
            Font.DrawText(SpriteBatch, xOrigin, yOrigin, string.Format("§lRunning at {0}{1} FPS",
                GetFPSColor(fps), fps), 1);
            Font.DrawText(SpriteBatch, xOrigin, yOrigin + (yOffset * 1), string.Format("§o{0} vertices, {1} indicies",
                Mesh.VerticiesRendered, Mesh.IndiciesRendered), 1);
            Font.DrawText(SpriteBatch, xOrigin, yOrigin + (yOffset * 2),
                string.Format("§o{0} chunks", Game.ChunkModule.ChunksRendered), 1);
            Font.DrawText(SpriteBatch, xOrigin, yOrigin + (yOffset * 3),
                string.Format("§o<{0:N2}, {1:N2}, {2:N2}>",
                Game.Client.Position.X, Game.Client.Position.Y, Game.Client.Position.Z), 1);
            SpriteBatch.End();
        }

        private string GetFPSColor(int fps)
        {
            if (fps <= 16)
                return "§c";
            if (fps <= 32)
                return "§e";
            return "§a";
        }
    }
}
