using System;
using TrueCraft.API.Networking;
using TrueCraft.API.Server;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.Core;
using TrueCraft.Core.Windows;

namespace TrueCraft.Handlers
{
    public static class InteractionHandlers
    {
        public static void HandlePlayerDiggingPacket(IPacket _packet, IRemoteClient _client, IMultiplayerServer server)
        {
            var packet = (PlayerDiggingPacket)_packet;
            var client = (RemoteClient)_client;
            var world = _client.World;
            var position = new Coordinates3D(packet.X, packet.Y, packet.Z);
            switch (packet.PlayerAction)
            {
                case PlayerDiggingPacket.Action.DropItem:
                    // TODO
                    break;
                case PlayerDiggingPacket.Action.StartDigging:
                    // TODO
                    break;
                case PlayerDiggingPacket.Action.StopDigging:
                    // TODO: Do this properly
                    var stack = new ItemStack(world.GetBlockID(position), 1, world.GetMetadata(position));
                    client.InventoryWindow.PickUpStack(stack);
                    world.SetBlockID(position, 0);
                    break;
            }
        }

        public static void HandlePlayerBlockPlacementPacket(IPacket _packet, IRemoteClient _client, IMultiplayerServer server)
        {
            var packet = (PlayerBlockPlacementPacket)_packet;
            var client = (RemoteClient)_client;

            var slot = client.SelectedItem;
            var position = new Coordinates3D(packet.X, packet.Y, packet.Z);
            BlockData? block = null;
            if (position != -Coordinates3D.One)
            {
                if (position.DistanceTo((Coordinates3D)client.Entity.Position) > 10 /* TODO: Reach */)
                    return;
                block = client.World.GetBlockData(position);
            }
            else
            {
                // TODO: Handle situations like firing arrows and such? Is that how it works?
                return;
            }
            bool use = true;
            if (block != null)
            {
                // TODO: Call the handler for the block being clicked on and possible cancel use
            }
            if (!slot.Empty)
            {
                if (use)
                {
                    // Temporary
                    position += MathHelper.BlockFaceToCoordinates(packet.Face);
                    client.World.SetBlockID(position, (byte)slot.Id);
                    client.World.SetMetadata(position, (byte)slot.Metadata);
                    // End temporary
                    if (block != null)
                    {
                        // TODO: Use item on block
                    }
                    else
                    {
                        // TODO: Use item
                    }
                }
            }
        }

        public static void HandleChangeHeldItem(IPacket _packet, IRemoteClient _client, IMultiplayerServer server)
        {
            var packet = (ChangeHeldItemPacket)_packet;
            var client = (RemoteClient)_client;
            client.SelectedSlot = (short)(packet.Slot + InventoryWindow.HotbarIndex);
        }
    }
}