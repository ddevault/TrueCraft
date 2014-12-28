using System;
using TrueCraft.API.Server;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API.Networking;

namespace TrueCraft.Handlers
{
    public static class PacketHandlers
    {
        public static void RegisterHandlers(IMultiplayerServer server)
        {
            server.RegisterPacketHandler(new HandshakePacket().ID, LoginHandlers.HandleHandshakePacket);
            server.RegisterPacketHandler(new LoginRequestPacket().ID, LoginHandlers.HandleLoginRequestPacket);
        }

        internal static void HandleKeepAlive(IPacket _packet, IRemoteClient _client, IMultiplayerServer server)
        {
            // TODO
        }
    }
}