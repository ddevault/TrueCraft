using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Windows;
using TrueCraft.API.Logic;
using TrueCraft.API;
using TrueCraft.API.Server;

namespace TrueCraft.Core.Windows
{
    public class FurnaceWindow : Window
    {
        public IItemRepository ItemRepository { get; set; }
        public IEventScheduler EventScheduler { get; set; }
        public Coordinates3D Coordinates { get; set; }
        
        public FurnaceWindow(IEventScheduler scheduler, Coordinates3D coordinates,
            IItemRepository itemRepository, InventoryWindow inventory)
        {
            ItemRepository = itemRepository;
            EventScheduler = scheduler;
            Coordinates = coordinates;

            WindowAreas = new[]
                {
                    new WindowArea(IngredientIndex, 1, 1, 1),
                    new WindowArea(FuelIndex, 1, 1, 1),
                    new WindowArea(OutputIndex, 1, 1, 1),
                    new WindowArea(MainIndex, 27, 9, 3),
                    new WindowArea(HotbarIndex, 9, 9, 1)
                };
            inventory.MainInventory.CopyTo(MainInventory);
            inventory.Hotbar.CopyTo(Hotbar);
            foreach (var area in WindowAreas)
                area.WindowChange += (s, e) => OnWindowChange(new WindowChangeEventArgs(
                    (s as WindowArea).StartIndex + e.SlotIndex, e.Value));
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
        }

        private bool Copying { get; set; }

        public const short IngredientIndex = 0;
        public const short FuelIndex = 1;
        public const short OutputIndex = 2;
        public const short MainIndex = 3;
        public const short HotbarIndex = 30;

        public override string Name
        {
            get
            {
                return "Furnace";
            }
        }

        public override sbyte Type
        {
            get
            {
                return 2;
            }
        }

        public override short[] ReadOnlySlots
        {
            get
            {
                return new[] { OutputIndex };
            }
        }

        public override IWindowArea[] WindowAreas { get; protected set; }

        public IWindowArea Ingredient 
        {
            get { return WindowAreas[0]; }
        }

        public IWindowArea Fuel
        {
            get { return WindowAreas[1]; }
        }

        public IWindowArea Output
        {
            get { return WindowAreas[2]; }
        }

        public IWindowArea MainInventory
        {
            get { return WindowAreas[3]; }
        }

        public IWindowArea Hotbar
        {
            get { return WindowAreas[4]; }
        }

        public override ItemStack[] GetSlots()
        {
            var relevantAreas = new[] { Ingredient, Fuel, Output };
            int length = relevantAreas.Sum(area => area.Length);
            var slots = new ItemStack[length];
            foreach (var windowArea in relevantAreas)
                Array.Copy(windowArea.Items, 0, slots, windowArea.StartIndex, windowArea.Length);
            return slots;
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
            if (index < MainIndex)
                return MainInventory;
            return Hotbar;
        }

        public override bool PickUpStack(ItemStack slot)
        {
            var area = MainInventory;
            foreach (var item in Hotbar.Items)
            {
                if (item.Empty || (slot.ID == item.ID && slot.Metadata == item.Metadata))
                    //&& item.Count + slot.Count < Item.GetMaximumStackSize(new ItemDescriptor(item.Id, item.Metadata)))) // TODO
                {
                    area = Hotbar;
                    break;
                }
            }
            int index = area.MoveOrMergeItem(-1, slot, null);
            return index != -1;
        }
    }
}