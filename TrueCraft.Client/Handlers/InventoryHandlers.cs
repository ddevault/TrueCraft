using System;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;
using TrueCraft.API.Windows;
using TrueCraft.API;
using TrueCraft.Core.Windows;
using TrueCraft.Core.Logic;
using TrueCraft.API.Logic;

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
                client.CurrentWindow.SetSlots(packet.Items);
        }

        public static void HandleSetSlot(IPacket _packet, MultiplayerClient client)
        {
            var packet = (SetSlotPacket)_packet;
            IWindow window = null;
            if (packet.WindowID == 0)
                window = client.Inventory;
            else
                window = client.CurrentWindow;
            if (window != null)
            {
                if (packet.SlotIndex >= 0 && packet.SlotIndex < window.Length)
                {
                    window[packet.SlotIndex] = new ItemStack(packet.ItemID, packet.Count, packet.Metadata);
                }
            }
        }

        public static void HandleOpenWindowPacket(IPacket _packet, MultiplayerClient client)
        {
            var packet = (OpenWindowPacket)_packet;
            IWindow window = null;
            switch (packet.Type)
            {
                case 1: // Crafting bench window
                    window = new CraftingBenchWindow(client.CraftingRepository, client.Inventory);
                    break;
            }
            window.ID = packet.WindowID;
            client.CurrentWindow = window;
        }

        public static void HandleCloseWindowPacket(IPacket _packet, MultiplayerClient client)
        {
            client.CurrentWindow = null;
        }
    }
}