using System;
using TrueCraft.API.Networking;
using TrueCraft.API.Server;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.Core;
using TrueCraft.Core.Windows;
using TrueCraft.API.Logic;
using TrueCraft.Core.Entities;

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
            var provider = server.BlockRepository.GetBlockProvider(descriptor.ID);
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
                    if (provider == null)
                        server.SendMessage(ChatColor.Red + "WARNING: block provider for ID {0} is null (player digging)", descriptor.ID);
                    else
                        provider.BlockLeftClicked(descriptor, packet.Face, world, client);
                    if (provider != null && provider.Hardness == 0)
                        provider.BlockMined(descriptor, packet.Face, world, client);
                    break;
                case PlayerDiggingPacket.Action.StopDigging:
                    foreach (var nearbyClient in server.Clients)
                    {
                        var c = (RemoteClient)nearbyClient;
                        if (c.KnownEntities.Contains(client.Entity))
                            c.QueuePacket(new AnimationPacket(client.Entity.EntityID, AnimationPacket.PlayerAnimation.None));
                    }
                    if (provider != null && descriptor.ID != 0)
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
                if (provider == null)
                {
                    server.SendMessage(ChatColor.Red + "WARNING: block provider for ID {0} is null (player placing)", block.Value.ID);
                    server.SendMessage(ChatColor.Red + "Error occured from client {0} at coordinates {1}", client.Username, block.Value.Coordinates);
                    server.SendMessage(ChatColor.Red + "Packet logged at {0}, please report upstream", DateTime.Now);
                    return;
                }
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
                    var itemProvider = server.ItemRepository.GetItemProvider(slot.ID);
                    if (itemProvider == null)
                    {
                        server.SendMessage(ChatColor.Red + "WARNING: item provider for ID {0} is null (player placing)", block.Value.ID);
                        server.SendMessage(ChatColor.Red + "Error occured from client {0} at coordinates {1}", client.Username, block.Value.Coordinates);
                        server.SendMessage(ChatColor.Red + "Packet logged at {0}, please report upstream", DateTime.Now);
                    }
                    if (block != null)
                    {
                        if (itemProvider != null)
                            itemProvider.ItemUsedOnBlock(position, slot, packet.Face, client.World, client);
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
            if (packet.SlotIndex == InventoryWindow.CraftingOutputIndex
                && (window is InventoryWindow || window is CraftingBenchWindow))
            {
                // Stupid special case because Minecraft was written by morons
                if (held.ID == existing.ID || held.Empty)
                {
                    if (held.Empty)
                        held = existing;
                    else
                        held.Count += existing.Count;
                    client.ItemStaging = held;
                    window[packet.SlotIndex] = ItemStack.EmptyStack;
                }
                return;
            }
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

        public static void HandleCloseWindowPacket(IPacket _packet, IRemoteClient _client, IMultiplayerServer server)
        {
            var packet = (CloseWindowPacket)_packet;
            if (packet.WindowID != 0)
                (_client as RemoteClient).CloseWindow(true);
        }

        public static void HandleChangeHeldItem(IPacket _packet, IRemoteClient _client, IMultiplayerServer server)
        {
            var packet = (ChangeHeldItemPacket)_packet;
            var client = (RemoteClient)_client;
            client.SelectedSlot = (short)(packet.Slot + InventoryWindow.HotbarIndex);
            var notified = server.GetEntityManagerForWorld(client.World).ClientsForEntity(client.Entity);
            foreach (var c in notified)
                c.QueuePacket(new EntityEquipmentPacket(client.Entity.EntityID, 0, client.SelectedItem.ID, client.SelectedItem.Metadata));
        }

        public static void HandlePlayerAction(IPacket _packet, IRemoteClient _client, IMultiplayerServer server)
        {
            var packet = (PlayerActionPacket)_packet;
            var client = (RemoteClient)_client;
            var entity = (PlayerEntity)client.Entity;
            switch (packet.Action)
            {
                case PlayerActionPacket.PlayerAction.Crouch:
                    entity.EntityFlags |= EntityFlags.Crouched;
                    break;
                case PlayerActionPacket.PlayerAction.Uncrouch:
                    entity.EntityFlags &= ~EntityFlags.Crouched;
                    break;
            }
        }
    }
}