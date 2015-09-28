using System;
using TrueCraft.API;
using TrueCraft.API.Server;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API.Networking;
using TrueCraft.API.Logging;
using TrueCraft.Exceptions;

namespace TrueCraft.Handlers
{
    public static class PacketHandlers
    {
        public static void RegisterHandlers(IMultiplayerServer server)
        {
            server.RegisterPacketHandler(new KeepAlivePacket().ID, HandleKeepAlive);
            server.RegisterPacketHandler(new ChatMessagePacket().ID, HandleChatMessage);
            server.RegisterPacketHandler(new DisconnectPacket().ID, HandleDisconnect);

            server.RegisterPacketHandler(new HandshakePacket().ID, LoginHandlers.HandleHandshakePacket);
            server.RegisterPacketHandler(new LoginRequestPacket().ID, LoginHandlers.HandleLoginRequestPacket);

            server.RegisterPacketHandler(new PlayerGroundedPacket().ID, (a, b, c) => { /* no-op */ });
            server.RegisterPacketHandler(new PlayerPositionPacket().ID, EntityHandlers.HandlePlayerPositionPacket);
            server.RegisterPacketHandler(new PlayerLookPacket().ID, EntityHandlers.HandlePlayerLookPacket);
            server.RegisterPacketHandler(new PlayerPositionAndLookPacket().ID, EntityHandlers.HandlePlayerPositionAndLookPacket);

            server.RegisterPacketHandler(new PlayerDiggingPacket().ID, InteractionHandlers.HandlePlayerDiggingPacket);
            server.RegisterPacketHandler(new PlayerBlockPlacementPacket().ID, InteractionHandlers.HandlePlayerBlockPlacementPacket);
            server.RegisterPacketHandler(new ChangeHeldItemPacket().ID, InteractionHandlers.HandleChangeHeldItem);
            server.RegisterPacketHandler(new PlayerActionPacket().ID, InteractionHandlers.HandlePlayerAction);
            server.RegisterPacketHandler(new AnimationPacket().ID, InteractionHandlers.HandleAnimation);
            server.RegisterPacketHandler(new ClickWindowPacket().ID, InteractionHandlers.HandleClickWindowPacket);
            server.RegisterPacketHandler(new CloseWindowPacket().ID, InteractionHandlers.HandleCloseWindowPacket);
            server.RegisterPacketHandler(new UpdateSignPacket().ID, InteractionHandlers.HandleUpdateSignPacket);
        }

        internal static void HandleKeepAlive(IPacket _packet, IRemoteClient _client, IMultiplayerServer server)
        {
            // TODO
        }

        internal static void HandleChatMessage(IPacket _packet, IRemoteClient _client, IMultiplayerServer _server)
        {
            // TODO: Abstract this to support things like commands
            // TODO: Sanitize messages
            var packet = (ChatMessagePacket)_packet;
            var server = (MultiplayerServer)_server;
            var args = new ChatMessageEventArgs(_client, packet.Message);
            server.OnChatMessageReceived(args);
        }

        internal static void HandleDisconnect(IPacket _packet, IRemoteClient _client, IMultiplayerServer server)
        {
            throw new PlayerDisconnectException(true);
        }
    }
}