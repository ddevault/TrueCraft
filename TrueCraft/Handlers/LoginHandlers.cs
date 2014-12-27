using System;
using TrueCraft.API.Server;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;

namespace TrueCraft.Handlers
{
    internal static class LoginHandlers
    {
        public static void HandleHandshakePacket(IPacket _packet, IRemoteClient _client, IMultiplayerServer server)
        {
            var packet = (HandshakePacket)_packet;
            var client = (RemoteClient)_client;
            client.Username = packet.Username;
            client.QueuePacket(new HandshakeResponsePacket("-")); // TODO: Implement some form of authentication
        }

        public static void HandleLoginRequestPacket(IPacket _packet, IRemoteClient _client, IMultiplayerServer server)
        {
            var packet = (LoginRequestPacket)_packet;
            var client = (RemoteClient)_client;
            client.QueuePacket(new DisconnectPacket("It works!"));
        }
    }
}