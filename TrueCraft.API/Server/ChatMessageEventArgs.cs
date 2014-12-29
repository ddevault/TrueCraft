using System;
using TrueCraft.API.Networking;

namespace TrueCraft.API.Server
{
    public class ChatMessageEventArgs : EventArgs
    {
        public ChatMessageEventArgs(IRemoteClient client, string message)
        {
            Client = client;
            Message = message;
            PreventDefault = false;
        }

        public IRemoteClient Client { get; set; }
        public string Message { get; set; }
        /// <summary>
        /// If set to true, the server won't send the default message back to the client.
        /// </summary>
        public bool PreventDefault { get; set; }
    }
}