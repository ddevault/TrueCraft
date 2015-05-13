using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueCraft.Client.Linux.Events;
using System.Collections.Generic;

namespace TrueCraft.Client.Linux.Interface
{
    public class ChatInterface : IGameInterface
    {
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

        public MultiplayerClient Client { get; set; }
        public FontRenderer Font { get; set; }

        private object Messages_lock = new object();
        private List<ChatMessage> Messages { get; set; }

        public ChatInterface(MultiplayerClient client, FontRenderer font)
        {
            Client = client;
            Font = font;
            Messages = new List<ChatMessage>();
            client.ChatMessage += HandleChatMessage;
        }

        void HandleChatMessage(object sender, ChatMessageEventArgs e)
        {
            lock (Messages_lock)
            {
                Messages.Add(new ChatMessage(e.Message));
                Console.WriteLine(e.Message);
            }
        }

        public void Update(GameTime gameTime)
        {
            lock (Messages_lock)
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
            lock (Messages_lock)
            {
                for (int i = 0; i < Messages.Count; i++)
                {
                    var message = Messages[i];
                    Font.DrawText(spriteBatch, 0, i * 30, message.Message);
                }
            }
        }
    }
}