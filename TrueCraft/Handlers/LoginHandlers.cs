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
            Console.WriteLine(packet.ProtocolVersion);
            if (packet.ProtocolVersion < server.PacketReader.ProtocolVersion)
                client.QueuePacket(new DisconnectPacket("Client outdated! Use beta 1.7.3!"));
            else if (packet.ProtocolVersion > server.PacketReader.ProtocolVersion)
                client.QueuePacket(new DisconnectPacket("Server outdated! Use beta 1.7.3!"));
            else
            {
                client.LoggedIn = true;
            }
        }
    }
}