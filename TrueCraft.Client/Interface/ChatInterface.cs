using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.Client.Events;
using System.Collections.Generic;
using TrueCraft.Client.Rendering;
using TrueCraft.Client.Input;
using Microsoft.Xna.Framework.Input;

namespace TrueCraft.Client.Interface
{
    public class ChatInterface : IGameInterface
    {
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

        private class ChatMessage
        {
            public string Message { get; set; }
            public DateTime Time { get; set; }

            public ChatMessage(string message)
            {
                Message = message;
                Time = DateTime.Now;
            }
        }

        public event EventHandler<EventArgs> FocusChanged;

        private readonly object _syncLock =
            new object();
        private bool _hasFocus;

        public bool HasFocus
        {
            get { return _hasFocus; }
            private set
            {
                if (value != _hasFocus)
                {
                    _hasFocus = value;

                    var args = EventArgs.Empty;
                    if (FocusChanged != null)
                        FocusChanged(this, args);
                }
            }
        }
        public MultiplayerClient Client { get; set; }
        public KeyboardComponent Keyboard { get; set; }
        public FontRenderer Font { get; set; }

        private string Input { get; set; }
        private List<ChatMessage> Messages { get; set; }

        public ChatInterface(MultiplayerClient client, KeyboardComponent keyboard, FontRenderer font)
        {
            Client = client;
            Keyboard = keyboard;
            Font = font;
            Input = string.Empty;
            HasFocus = false;
            Messages = new List<ChatMessage>();

            keyboard.KeyUp += HandleKeyDown;
            client.ChatMessage += HandleChatMessage;
        }

        public void AddMessage(string message)
        {
            lock (_syncLock)
            {
                Messages.Add(new ChatMessage(message));
                Console.WriteLine(message);
            }
        }

        private void HandleKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (HasFocus)
            {
                if (e.Key == Keys.Enter)
                {
                    HasFocus = false;
                    if (Input.Length != 0)
                    {
                        Client.SendMessage(Input);
                        Input = string.Empty;
                    }
                    return;
                }
                else if (e.Key == Keys.Back)
                {
                    Input = Input.Substring(0, Input.Length - 1);
                }
                else if (e.Key == Keys.Escape)
                {
                    HasFocus = false;
                    Input = string.Empty;
                    return;
                }

                var shift = (Keyboard.State.IsKeyDown(Keys.LeftShift) || Keyboard.State.IsKeyDown(Keys.RightShift));
                var value = default(char);

                if (TryParseKey(e.Key, shift, out value))
                    Input += new string(new char[] { value });
            }
            else
            {
                switch (e.Key)
                {
                    case Keys.T:
                        HasFocus = true;
                        break;

                    case Keys.OemQuestion:
                        HasFocus = true;
                        Input += "/";
                        break;
                }
            }
        }

        void HandleChatMessage(object sender, ChatMessageEventArgs e)
        {
            AddMessage(e.Message);
        }

        public void Update(GameTime gameTime)
        {
            lock (_syncLock)
            {
                for (int i = 0; i < Messages.Count; i++)
                {
                    var message = Messages[i];
                    if ((DateTime.Now - message.Time).TotalSeconds > 10)
                    {
                        Messages.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public void DrawSprites(GameTime gameTime, SpriteBatch spriteBatch)
        {
            lock (_syncLock)
            {
                for (int i = (Messages.Count - 1); i >= 0; i--)
                {
                    if (Messages.Count == 0)
                        break;

                    var invi = (Messages.Count - i);

                    var message = Messages[i];
                    Font.DrawText(spriteBatch, 0, i * 30, message.Message);
                    if (invi >= 5) break;
                }
            }

            if (HasFocus)
                Font.DrawText(spriteBatch, 0, (6 * 30) + 15, "> " + Input);
        }
    }
}