using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.Client.Rendering;
using TrueCraft.Client.Input;
using TrueCraft.API;

namespace TrueCraft.Client.Interface
{
    public class DebugInterface : Control
    {
        public MultiplayerClient Client { get; set; }
        public FontRenderer Font { get; set; }

        public int Vertices { private get; set; }
        public int Chunks { private get; set; }

        public DebugInterface(MultiplayerClient client, FontRenderer font)
        {
            Client = client;
            Font = font;
        }

        protected override void OnShow() { }

        protected override void OnUpdate(GameTime gameTime) { }

        protected override void OnDrawSprites(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // UI scaling
            var scale = GetScaleFactor();
            var xOrigin = (int)(10 * scale);
            var yOrigin = (int)(5 * scale);
            var yOffset = (int)(25 * scale);

            var fps = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds) + 1;
            var position = Client.Position;

            Font.DrawText(spriteBatch, xOrigin, yOrigin, string.Format("§lRunning at {0} FPS", GetFPSColor(fps) + fps.ToString()), scale);
            Font.DrawText(spriteBatch, xOrigin, yOrigin + (yOffset * 1), string.Format("§o{0} vertices", Vertices), scale);
            Font.DrawText(spriteBatch, xOrigin, yOrigin + (yOffset * 2), string.Format("§o{0} chunks", Chunks), scale);
            Font.DrawText(spriteBatch, xOrigin, yOrigin + (yOffset * 3), string.Format("§o<{0:N2}, {1:N2}, {2:N2}>", Client.Position.X, Client.Position.Y, Client.Position.Z), scale);
        }

        protected override void OnHide() { }

        private string GetFPSColor(int fps)
        {
            if (fps <= 16)
                return "§c";
            else if (fps <= 32)
                return "§e";
            else
                return "§a";
        }
    }
}
