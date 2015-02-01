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
            var descriptor = world.GetBlockData(position);
            switch (packet.PlayerAction)
            {
                case PlayerDiggingPacket.Action.DropItem:
                    // TODO
                    break;
                case PlayerDiggingPacket.Action.StartDigging:
                    foreach (var nearbyClient in server.Clients) // TODO: Send this repeatedly during the course of the digging
                    {
                        var c = (RemoteClient)nearbyClient;
                        if (c.KnownEntities.Contains(client.Entity))
                            c.QueuePacket(new AnimationPacket(client.Entity.EntityID, AnimationPacket.PlayerAnimation.SwingArm));
                    }
                    break;
                case PlayerDiggingPacket.Action.StopDigging:
                    foreach (var nearbyClient in server.Clients)
                    {
                        var c = (RemoteClient)nearbyClient;
                        if (c.KnownEntities.Contains(client.Entity))
                            c.QueuePacket(new AnimationPacket(client.Entity.EntityID, AnimationPacket.PlayerAnimation.None));
                    }
                    var provider = server.BlockRepository.GetBlockProvider(descriptor.ID);
                    provider.BlockMined(descriptor, packet.Face, world, client);
                    break;
            }
        }

        public static void HandlePlayerBlockPlacementPacket(IPacket _packet, IRemoteClient _client, IMultiplayerServer server)
        {
            var packet = (PlayerBlockPlacementPacket)_packet;
            var client = (RemoteClient)_client;

            var slot = client.SelectedItem;
            var position = new Coordinates3D(packet.X, packet.Y, packet.Z);
            BlockDescriptor? block = null;
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
                var provider = server.BlockRepository.GetBlockProvider(block.Value.ID);
                if (!provider.BlockRightClicked(block.Value, packet.Face, client.World, client))
                {
                    position += MathHelper.BlockFaceToCoordinates(packet.Face);
                    var oldID = client.World.GetBlockID(position);
                    var oldMeta = client.World.GetMetadata(position);
                    client.QueuePacket(new BlockChangePacket(position.X, (sbyte)position.Y, position.Z, (sbyte)oldID, (sbyte)oldMeta));
                    client.QueuePacket(new SetSlotPacket(0, client.SelectedSlot, client.SelectedItem.ID, client.SelectedItem.Count, client.SelectedItem.Metadata));
                    return;
                }
            }
            if (!slot.Empty)
            {
                if (use)
                {
                    // Temporary: just place the damn thing
                    position += MathHelper.BlockFaceToCoordinates(packet.Face);
                    client.World.SetBlockID(position, (byte)slot.ID);
                    client.World.SetMetadata(position, (byte)slot.Metadata);
                    slot.Count--;
                    client.Inventory[client.SelectedSlot] = slot;
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

        public static void HandleClickWindowPacket(IPacket _packet, IRemoteClient _client, IMultiplayerServer server)
        {
            var packet = (ClickWindowPacket)_packet;
            var client = (RemoteClient)_client;
            var window = client.CurrentWindow;
            if (packet.SlotIndex >= window.Length || packet.SlotIndex < 0)
                return;
            ItemStack existing = window[packet.SlotIndex];
            ItemStack held = client.ItemStaging;
            if (client.ItemStaging.Empty) // Picking up something
            {
                if (packet.Shift)
                {
                    window.MoveToAlternateArea(packet.SlotIndex);
                }
                else
                {
                    if (packet.RightClick)
                    {
                        sbyte mod = (sbyte)(existing.Count % 2);
                        existing.Count /= 2;
                        held = existing;
                        held.Count += mod;
                        client.ItemStaging = held;
                        window[packet.SlotIndex] = existing;
                    }
                    else
                    {
                        client.ItemStaging = window[packet.SlotIndex];
                        window[packet.SlotIndex] = ItemStack.EmptyStack;
                    }
                }
            }
            else // Setting something down
            {
                if (existing.Empty) // Replace empty slot
                {
                    if (packet.RightClick)
                    {
                        var newItem = (ItemStack)client.ItemStaging.Clone();
                        newItem.Count = 1;
                        held.Count--;
                        window[packet.SlotIndex] = newItem;
                        client.ItemStaging = held;
                    }
                    else
                    {
                        window[packet.SlotIndex] = client.ItemStaging;
                        client.ItemStaging = ItemStack.EmptyStack;
                    }
                }
                else
                {
                    if (existing.CanMerge(client.ItemStaging)) // Merge items
                    {
                        // TODO: Consider the maximum stack size
                        if (packet.RightClick)
                        {
                            existing.Count++;
                            held.Count--;
                            window[packet.SlotIndex] = existing;
                            client.ItemStaging = held;
                        }
                        else
                        {
                            existing.Count += client.ItemStaging.Count;
                            window[packet.SlotIndex] = existing;
                            client.ItemStaging = ItemStack.EmptyStack;
                        }
                    }
                    else // Swap items
                    {
                        window[packet.SlotIndex] = client.ItemStaging;
                        client.ItemStaging = existing;
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