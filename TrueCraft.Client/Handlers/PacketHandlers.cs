using System;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.Core.Networking;
using TrueCraft.Client.Events;
using TrueCraft.API;
using System.Diagnostics;

namespace TrueCraft.Client.Handlers
{
    internal static class PacketHandlers
    {
        public static void RegisterHandlers(MultiplayerClient client)
        {
            client.RegisterPacketHandler(new HandshakeResponsePacket().ID, HandleHandshake);
            client.RegisterPacketHandler(new ChatMessagePacket().ID, HandleChatMessage);
            client.RegisterPacketHandler(new SetPlayerPositionPacket().ID, HandlePositionAndLook);
            client.RegisterPacketHandler(new LoginResponsePacket().ID, HandleLoginResponse);

            client.RegisterPacketHandler(new ChunkPreamblePacket().ID, ChunkHandler.HandleChunkPreamble);
            client.RegisterPacketHandler(new ChunkDataPacket().ID, ChunkHandler.HandleChunkData);
        }

        public static void HandleChatMessage(IPacket _packet, MultiplayerClient client)
        {
            var packet = (ChatMessagePacket)_packet;
            client.OnChatMessage(new ChatMessageEventArgs(packet.Message));
        }

        public static void HandleHandshake(IPacket _packet, MultiplayerClient client)
        {
            var packet = (HandshakeResponsePacket)_packet;
            if (packet.ConnectionHash != "-")
            {
                Console.WriteLine("Online mode is not supported");
                Process.GetCurrentProcess().Kill();
            }
            // TODO: Authentication
            client.QueuePacket(new LoginRequestPacket(PacketReader.Version, "TestUser"));
        }

        public static void HandleLoginResponse(IPacket _packet, MultiplayerClient client)
        {
            client.QueuePacket(new PlayerGroundedPacket());
        }

        public static void HandlePositionAndLook(IPacket _packet, MultiplayerClient client)
        {
            var packet = (SetPlayerPositionPacket)_packet;
            client._Position = new Vector3(packet.X, packet.Y, packet.Z);
            client.QueuePacket(packet);
            client.LoggedIn = true;
            // TODO: Pitch and yaw
        }
    }
}