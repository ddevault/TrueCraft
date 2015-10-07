using System;
using TrueCraft.Client.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.Client.Rendering;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

namespace TrueCraft.Client.Modules
{
    public class ChatModule : InputModule, IGraphicalModule
    {
        private static readonly int TimeToFade = 9 * 1000;
        private static readonly int TimeToExpire = 10 * 1000;
        private static readonly int MaxLines = 10;

        private struct Message
        {
            public string Text { get; set; }
            public long Arrived { get; set; }
        }

        private TrueCraftGame Game { get; set; }
        private Texture2D Background { get; set; }
        private FontRenderer Font { get; set; }
        private SpriteBatch SpriteBatch { get; set; }
        private List<Message> Messages { get; set; }
        private Stopwatch Watch { get; set; }
        private bool Editing { get; set; }
        private string Text { get; set; }

        public ChatModule(TrueCraftGame game, FontRenderer font)
        {
            Game = game;
            Font = font;
            Messages = new List<Message>();
            Background = new Texture2D(Game.GraphicsDevice, 1, 1);
            Background.SetData<Color>(new[] { new Color(Color.Black, 160) });
            SpriteBatch = new SpriteBatch(Game.GraphicsDevice);
            Watch = new Stopwatch();
            Watch.Start();
            Text = string.Empty;
            Game.Client.ChatMessage += (sender, e) => AddMessage(e.Message);
        }

        public override bool KeyDown(GameTime gameTime, KeyboardKeyEventArgs e)
        {
            if (!Editing)
            {
                if (e.Key == Keys.T)
                {
                    Editing = true;
                    return true;
                }
                return false;
            }
            if (e.Key == Keys.Back)
                Text = Text.Length > 0 ? Text.Substring(0, Text.Length - 1) : Text;
            else if (e.Key == Keys.Escape)
            {
                Editing = false;
                Text = string.Empty;
            }
            else if (e.Key == Keys.Enter)
            {
                Game.Client.SendMessage(Text);
                Editing = false;
                Text = string.Empty;
            }
            else
            {
                var shift = (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift));
                var value = default(char);

                if (TryParseKey(e.Key, shift, out value))
                    Text += value;
            }
            return true;
        }

        public override bool KeyUp(GameTime gameTime, KeyboardKeyEventArgs e)
        {
            return Editing;
        }

        public void AddMessage(string text)
        {
            Console.WriteLine(text);
            Messages.Insert(0, new Message { Text = text, Arrived = Watch.ElapsedMilliseconds });
        }

        public void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();
            const int height = 25;
            int x = 5, y = (int)(Game.GraphicsDevice.Viewport.Height - Scale(22) * 2);
            int width = Game.GraphicsDevice.Viewport.Width / 2;
            int max = MaxLines;
            if (Editing)
                max = Game.GraphicsDevice.Viewport.Height / height;
            for (int i = 0; i < Messages.Count && i < max; i++)
            {
                var time = Watch.ElapsedMilliseconds - Messages[i].Arrived;
                if (time >= TimeToExpire && !Editing)
                    continue;
                byte alpha = 255;
                if (time > TimeToFade && !Editing)
                {
                    var t = TimeToExpire - time / (double)(TimeToExpire - TimeToFade);
                    alpha = (byte)(t * 256);
                }
                SpriteBatch.Draw(Background, new Rectangle(
                    x, y - (i * height), width, height), new Color(Color.White, alpha));
                Font.DrawText(SpriteBatch, x, y - (i * height) - 5, Messages[i].Text, alpha: alpha);
            }
            if (Editing)
            {
                SpriteBatch.Draw(Background,
                    new Rectangle(0, Game.GraphicsDevice.Viewport.Height - height,
                    Game.GraphicsDevice.Viewport.Width, height), Color.White);
                if (Watch.Elapsed.Seconds % 2 == 0)
                    Font.DrawText(SpriteBatch, 3, Game.GraphicsDevice.Viewport.Height - height - 5, Text);
                else
                    Font.DrawText(SpriteBatch, 3, Game.GraphicsDevice.Viewport.Height - height - 5, Text + "_");
            }
            SpriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
        }

        private float Scale(float value)
        {
            return value * Game.ScaleFactor * 2;
        }

        private static bool TryParseKey(Keys key, bool shift, out char value)
        {
            // Credit to Roy Triesscheijn for thinking of this.
            // His implementation of this solution can be found at:
            // http://roy-t.nl/index.php/2010/02/11/code-snippet-converting-keyboard-input-to-text-in-xna/

            switch (key)
            {
                case Keys.NumPad1: value = '1'; break;
                case Keys.NumPad2: value = '2'; break;
                case Keys.NumPad3: value = '3'; break;
                case Keys.NumPad4: value = '4'; break;
                case Keys.NumPad5: value = '5'; break;
                case Keys.NumPad6: value = '6'; break;
                case Keys.NumPad7: value = '7'; break;
                case Keys.NumPad8: value = '8'; break;
                case Keys.NumPad9: value = '9'; break;
                case Keys.NumPad0: value = '0'; break;

                case Keys.D1: value = (shift) ? '!' : '1'; break;
                case Keys.D2: value = (shift) ? '@' : '2'; break;
                case Keys.D3: value = (shift) ? '#' : '3'; break;
                case Keys.D4: value = (shift) ? '$' : '4'; break;
                case Keys.D5: value = (shift) ? '%' : '5'; break;
                case Keys.D6: value = (shift) ? '^' : '6'; break;
                case Keys.D7: value = (shift) ? '&' : '7'; break;
                case Keys.D8: value = (shift) ? '*' : '8'; break;
                case Keys.D9: value = (shift) ? '(' : '9'; break;
                case Keys.D0: value = (shift) ? ')' : '0'; break;

                case Keys.A: value = (shift) ? 'A' : 'a'; break;
                case Keys.B: value = (shift) ? 'B' : 'b'; break;
                case Keys.C: value = (shift) ? 'C' : 'c'; break;
                case Keys.D: value = (shift) ? 'D' : 'd'; break;
                case Keys.E: value = (shift) ? 'E' : 'e'; break;
                case Keys.F: value = (shift) ? 'F' : 'f'; break;
                case Keys.G: value = (shift) ? 'G' : 'g'; break;
                case Keys.H: value = (shift) ? 'H' : 'h'; break;
                case Keys.I: value = (shift) ? 'I' : 'i'; break;
                case Keys.J: value = (shift) ? 'J' : 'j'; break;
                case Keys.K: value = (shift) ? 'K' : 'k'; break;
                case Keys.L: value = (shift) ? 'L' : 'l'; break;
                case Keys.M: value = (shift) ? 'M' : 'm'; break;
                case Keys.N: value = (shift) ? 'N' : 'n'; break;
                case Keys.O: value = (shift) ? 'O' : 'o'; break;
                case Keys.P: value = (shift) ? 'P' : 'p'; break;
                case Keys.Q: value = (shift) ? 'Q' : 'q'; break;
                case Keys.R: value = (shift) ? 'R' : 'r'; break;
                case Keys.S: value = (shift) ? 'S' : 's'; break;
                case Keys.T: value = (shift) ? 'T' : 't'; break;
                case Keys.U: value = (shift) ? 'U' : 'u'; break;
                case Keys.V: value = (shift) ? 'V' : 'v'; break;
                case Keys.W: value = (shift) ? 'W' : 'w'; break;
                case Keys.X: value = (shift) ? 'X' : 'x'; break;
                case Keys.Y: value = (shift) ? 'Y' : 'y'; break;
                case Keys.Z: value = (shift) ? 'Z' : 'z'; break;

                case Keys.OemTilde: value = (shift) ? '~' : '`'; break;
                case Keys.OemSemicolon: value = (shift) ? ':' : ';'; break;
                case Keys.OemQuotes: value = (shift) ? '"' : '\''; break;
                case Keys.OemQuestion: value = (shift) ? '?' : '/'; break;
                case Keys.OemPlus: value = (shift) ? '+' : '='; break;
                case Keys.OemPipe: value = (shift) ? '|' : '\\'; break;
                case Keys.OemPeriod: value = (shift) ? '>' : '.'; break;
                case Keys.OemOpenBrackets: value = (shift) ? '{' : '['; break;
                case Keys.OemCloseBrackets: value = (shift) ? '}' : ']'; break;
                case Keys.OemMinus: value = (shift) ? '_' : '-'; break;
                case Keys.OemComma: value = (shift) ? '<' : ','; break;

                case Keys.Space: value = ' '; break;

                default: value = default(char); return false;
            }

            return true;
        }
    }
}