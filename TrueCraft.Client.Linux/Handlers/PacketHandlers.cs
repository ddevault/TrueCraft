using System;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.Core.Networking;

namespace TrueCraft.Client.Linux.Handlers
{
    internal static class PacketHandlers
    {
        public static void RegisterHandlers(MultiplayerClient client)
        {
            client.RegisterPacketHandler(new HandshakeResponsePacket().ID, HandleHandshake);
        }

        public static void HandleHandshake(IPacket _packet, MultiplayerClient client)
        {
            var packet = (HandshakeResponsePacket)_packet;
            Console.WriteLine("Got handshake with {0}", packet.ConnectionHash); // TODO: Authenticate?
            client.QueuePacket(new LoginRequestPacket(PacketReader.Version, "TestUser"));
        }
    }
}