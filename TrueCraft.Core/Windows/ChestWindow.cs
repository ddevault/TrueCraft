using System;
using TrueCraft.API.Windows;
using TrueCraft.Core.Windows;
using TrueCraft.API;

namespace TrueCraft.Core.Windows
{
    public class ChestWindow : Window
    {
        public ChestWindow(InventoryWindow inventory, bool doubleChest = false)
        {
            DoubleChest = doubleChest;
            if (doubleChest)
            {
                WindowAreas = new[]
                {
                    new WindowArea(ChestIndex, 54, 9, 3), // Chest
                    new WindowArea(DoubleMainIndex, 27, 9, 3), // Main inventory
                    new WindowArea(DoubleHotbarIndex, 9, 9, 1) // Hotbar
                };
            }
            else
            {
                WindowAreas = new[]
                {
                    new WindowArea(ChestIndex, 27, 9, 3), // Chest
                    new WindowArea(MainIndex, 27, 9, 3), // Main inventory
                    new WindowArea(HotbarIndex, 9, 9, 1) // Hotbar
                };
            }
            inventory.MainInventory.CopyTo(MainInventory);
            inventory.Hotbar.CopyTo(Hotbar);
            Copying = false;
            inventory.WindowChange += (sender, e) =>
            {
                if (Copying) return;
                if ((e.SlotIndex >= InventoryWindow.MainIndex && e.SlotIndex < InventoryWindow.MainIndex + inventory.MainInventory.Length)
                    || (e.SlotIndex >= InventoryWindow.HotbarIndex && e.SlotIndex < InventoryWindow.HotbarIndex + inventory.Hotbar.Length))
                {
                    inventory.MainInventory.CopyTo(MainInventory);
                    inventory.Hotbar.CopyTo(Hotbar);
                }
            };
            foreach (var area in WindowAreas)
                area.WindowChange += (s, e) => OnWindowChange(new WindowChangeEventArgs(
                    (s as WindowArea).StartIndex + e.SlotIndex, e.Value));
        }

        public const int ChestIndex = 0;
        public const int MainIndex = 27;
        public const int HotbarIndex = 54;
        public const int DoubleMainIndex = 54;
        public const int DoubleHotbarIndex = 81;

        public bool DoubleChest { get; set; }
        public override IWindowArea[] WindowAreas { get; protected set; }
        private bool Copying { get; set; }

        public override string Name
        {
            get
            {
                if (DoubleChest)
                    return "Large Chest";
                return "Chest";
            }
        }

        public override sbyte Type
        {
            get
            {
                return 0;
            }
        }

        public IWindowArea ChestInventory
        {
            get
            {
                return WindowAreas[0];
            }
        }

        public IWindowArea MainInventory
        {
            get
            {
                return WindowAreas[1];
            }
        }

        public IWindowArea Hotbar
        {
            get
            {
                return WindowAreas[2];
            }
        }

        public override int MinecraftWasWrittenByFuckingIdiotsLength
        {
            get
            {
                return ChestInventory.Length;
            }
        }

        public override void CopyToInventory(IWindow inventoryWindow)
        {
            var window = (InventoryWindow)inventoryWindow;
            Copying = true;
            MainInventory.CopyTo(window.MainInventory);
            Hotbar.CopyTo(window.Hotbar);
            Copying = false;
        }

        protected override IWindowArea GetLinkedArea(int index, ItemStack slot)
        {
            if (index == 0)
                return Hotbar;
            else
                return ChestInventory;
        }
    }
}