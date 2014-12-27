using System;
using TrueCraft.API.Networking;
using System.Net;
using System.Collections.Generic;

namespace TrueCraft.API.Server
{
    /// <summary>
    /// Called when the given packet comes in from a remote client. Return false to cease communication
    /// with that client.
    /// </summary>
    public delegate void PacketHandler(IPacket packet, IRemoteClient client, IMultiplayerServer server);

    public interface IMultiplayerServer
    {
        IPacketReader PacketReader { get; }
        IList<IRemoteClient> Clients { get; }

        void Start(IPEndPoint endPoint);
        void RegisterPacketHandler(byte packetId, PacketHandler handler);
    }
}