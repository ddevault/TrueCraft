using System;
using TrueCraft.API.Networking;

namespace TrueCraft.API.Server
{
    public class PlayerJoinedQuitEventArgs : EventArgs
    {
        public IRemoteClient Client { get; set; }

        public PlayerJoinedQuitEventArgs(IRemoteClient client)
        {
            Client = client;
        }
    }
}