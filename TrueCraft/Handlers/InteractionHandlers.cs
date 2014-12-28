using System;
using TrueCraft.API.Networking;
using TrueCraft.API.Server;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API;

namespace TrueCraft.Handlers
{
    public static class InteractionHandlers
    {
        public static void HandlePlayerDiggingPacket(IPacket _packet, IRemoteClient _client, IMultiplayerServer server)
        {
            var packet = (PlayerDiggingPacket)_packet;
            var world = _client.World;
            var position = new Coordinates3D(packet.X, packet.Y, packet.Z);
            switch (packet.PlayerAction)
            {
                case PlayerDiggingPacket.Action.DropItem:
                    break;
                case PlayerDiggingPacket.Action.StartDigging:
                    break;
                case PlayerDiggingPacket.Action.StopDigging:
                    if (_client.Entity.Position.DistanceTo(position) < 12)
                    {
                        // TODO: Be smarter about this
                        world.SetBlockID(position, 0);
                    }
                    break;
            }
        }
    }
}