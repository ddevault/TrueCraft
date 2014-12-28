using System;
using TrueCraft.API.Networking;
using TrueCraft.API.Server;
using TrueCraft.API;
using TrueCraft.Core.Networking.Packets;

namespace TrueCraft.Handlers
{
    internal static class EntityHandlers
    {
        public static void HandlePlayerPositionPacket(IPacket _packet, IRemoteClient _client, IMultiplayerServer server)
        {
            var packet = (PlayerPositionPacket)_packet;
            HandlePlayerMovement(_client, new Vector3(packet.X, packet.Y, packet.Z), _client.Entity.Yaw, _client.Entity.Pitch);
        }

        public static void HandlePlayerMovement(IRemoteClient client, Vector3 position, float yaw, float pitch)
        {
            if (client.Entity.Position.DistanceTo(position) > 3)
            {
                client.QueuePacket(new DisconnectPacket("Client moved to fast (hacking?)"));
            }
            else
            {
                client.Entity.Position = position;
                client.Entity.Yaw = yaw;
                client.Entity.Pitch = pitch;
            }
        }
    }
}