using System;
using TrueCraft.API.Server;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API.Logging;
using TrueCraft.API;
using TrueCraft.Core.Entities;

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
                client.LoggedIn = true;
                client.Entity = new PlayerEntity(client.Username);
                client.World = server.Worlds[0];
                client.ChunkRadius = 2;

                if (!client.Load())
                    client.Entity.Position = client.World.SpawnPoint;

                // Send setup packets
                client.QueuePacket(new LoginResponsePacket(0, 0, Dimension.Overworld));
                client.UpdateChunks();
                client.QueuePacket(new WindowItemsPacket(0, client.Inventory.GetSlots()));
                client.QueuePacket(new SpawnPositionPacket((int)client.Entity.Position.X,
                        (int)client.Entity.Position.Y, (int)client.Entity.Position.Z));
                client.QueuePacket(new SetPlayerPositionPacket(client.Entity.Position.X,
                        client.Entity.Position.Y + 1,
                        client.Entity.Position.Y + client.Entity.Size.Height + 1,
                        client.Entity.Position.Z, client.Entity.Yaw, client.Entity.Pitch, true));
                client.QueuePacket(new TimeUpdatePacket(client.World.Time));

                // Start housekeeping for this client
                var entityManager = server.GetEntityManagerForWorld(client.World);
                entityManager.SpawnEntity(client.Entity);
                entityManager.SendEntitiesToClient(client);
                server.Scheduler.ScheduleEvent(DateTime.Now.AddSeconds(10), client.SendKeepAlive);
                server.Scheduler.ScheduleEvent(DateTime.Now.AddSeconds(1), client.ExpandChunkRadius);

                if (!string.IsNullOrEmpty(Program.Configuration.MOTD))
                    client.SendMessage(Program.Configuration.MOTD);
                server.SendMessage(ChatColor.Yellow + "{0} joined the server.", client.Username);
            }
        }
    }
}