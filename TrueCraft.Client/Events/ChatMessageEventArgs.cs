using System;

namespace TrueCraft.Client.Events
{
    public class ChatMessageEventArgs : EventArgs
    {
        public string Message { get; set; }

        public ChatMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}