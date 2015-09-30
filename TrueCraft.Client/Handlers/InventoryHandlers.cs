using System;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API.Windows;
using TrueCraft.API;

namespace TrueCraft.Client.Handlers
{
    internal static class InventoryHandlers
    {
        public static void HandleWindowItems(IPacket _packet, MultiplayerClient client)
        {
            var packet = (WindowItemsPacket)_packet;
            if (packet.WindowID == 0)
                client.Inventory.SetSlots(packet.Items);
            else
            {
                // TODO
            }
        }

        public static void HandleSetSlot(IPacket _packet, MultiplayerClient client)
        {
            var packet = (SetSlotPacket)_packet;
            IWindow window = null;
            if (packet.WindowID == 0)
                window = client.Inventory;
            else
            {
                // TODO
            }
            if (window != null)
            {
                if (packet.SlotIndex >= 0 && packet.SlotIndex < window.Length)
                {
                    window[packet.SlotIndex] = new ItemStack(packet.ItemID, packet.Count, packet.Metadata);
                }
            }
        }
    }
}