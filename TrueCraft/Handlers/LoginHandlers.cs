using System;
using TrueCraft.API.Server;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API.Logging;
using TrueCraft.API;
using TrueCraft.Entities;

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
            if (packet.ProtocolVersion < server.PacketReader.ProtocolVersion)
                client.QueuePacket(new DisconnectPacket("Client outdated! Use beta 1.7.3."));
            else if (packet.ProtocolVersion > server.PacketReader.ProtocolVersion)
                client.QueuePacket(new DisconnectPacket("Server outdated! Use beta 1.7.3."));
            else if (server.Worlds.Count == 0)
                client.QueuePacket(new DisconnectPacket("Server has no worlds configured."));
            else
            {
                server.Log(LogCategory.Notice, "{0} joined the server.", client.Username); // TODO: Mention the same thing in chat
                client.LoggedIn = true;
                client.Entity = new PlayerEntity();
                client.World = server.Worlds[0];
                server.GetEntityManagerForWorld(client.World).SpawnEntity(client.Entity);
                client.QueuePacket(new LoginResponsePacket(0, 0, Dimension.Overworld));
                client.ChunkRadius = 4;
                client.UpdateChunks();
                client.QueuePacket(new SpawnPositionPacket(0, 16, 0));
                client.QueuePacket(new SetPlayerPositionPacket(0, 16, 17, 0, 0, 0, true));
                client.QueuePacket(new ChatMessagePacket(string.Format("Welcome to the server, {0}!", client.Username)));
            }
        }
    }
}