using System;
using TrueCraft.API.Networking;

namespace TrueCraft.API.Server
{
    public class PlayerJoinedEventArgs : EventArgs
    {
        public IRemoteClient Client { get; set; }

        public PlayerJoinedEventArgs(IRemoteClient client)
        {
            Client = client;
        }
    }
}