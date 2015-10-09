using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Windows;
using TrueCraft.API;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;

namespace TrueCraft.Core.Windows
{
    public abstract class Window : IWindow, IDisposable, IEventSubject
    {
        public abstract IWindowArea[] WindowAreas { get; protected set; }

        public event EventHandler<WindowChangeEventArgs> WindowChange;

        public bool IsDisposed { get; private set; }

        public IRemoteClient Client { get; set; }

        public virtual void MoveToAlternateArea(int index)
        {
            int fromIndex = GetAreaIndex(index);
            var from = GetArea(ref index);
            var slot = from[index];
            if (slot.Empty)
                return;
            var to = GetLinkedArea(fromIndex, slot);
            int destination = to.MoveOrMergeItem(index, slot, from);
            if (WindowChange != null && destination != -1)
                WindowChange(this, new WindowChangeEventArgs(destination + to.StartIndex, slot));
        }
                
        public sbyte ID { get; set; }
        public abstract string Name { get; }
        public abstract sbyte Type { get; }

        /// <summary>
        /// When shift-clicking items between areas, this method is used
        /// to determine which area links to which.
        /// </summary>
        /// <param name="index">The index of the area the item is coming from</param>
        /// <param name="slot">The item being moved</param>
        /// <returns>The area to place the item into</returns>
        protected abstract IWindowArea GetLinkedArea(int index, ItemStack slot);
        public abstract void CopyToInventory(IWindow inventoryWindow);

        /// <summary>
        /// Gets the window area to handle this index and adjust index accordingly
        /// </summary>
        protected IWindowArea GetArea(ref int index)
        {
            foreach (var area in WindowAreas)
            {
                if (area.StartIndex <= index && area.StartIndex + area.Length > index)
                {
                    index = index - area.StartIndex;
                    return area;
                }
            }
            throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Gets the index of the appropriate area from the WindowAreas array.
        /// </summary>
        protected int GetAreaIndex(int index)
        {
            for (int i = 0; i < WindowAreas.Length; i++)
            {
                var area = WindowAreas[i];
                if (index >= area.StartIndex && index < area.StartIndex + area.Length)
                    return i;
            }
            throw new IndexOutOfRangeException();
        }

        public virtual int Length
        {
            get 
            {
                return WindowAreas.Sum(a => a.Length);
            }
        }

        public virtual int MinecraftWasWrittenByFuckingIdiotsLength { get { return Length; } }

        public bool Empty
        {
            get
            {
                return !WindowAreas.Any(a => a.Items.Any(i => !i.Empty));
            }
        }

        public virtual ItemStack[] GetSlots()
        {
            int length = WindowAreas.Sum(area => area.Length);
            var slots = new ItemStack[length];
            foreach (var windowArea in WindowAreas)
                Array.Copy(windowArea.Items, 0, slots, windowArea.StartIndex, windowArea.Length);
            return slots;
        }

        public virtual void SetSlots(ItemStack[] slots)
        {
            foreach (var windowArea in WindowAreas)
            {
                if (windowArea.StartIndex < slots.Length && windowArea.StartIndex + windowArea.Length <= slots.Length)
                    Array.Copy(slots, windowArea.StartIndex, windowArea.Items, 0, windowArea.Length);
            }
        }

        public virtual ItemStack this[int index]
        {
            get
            {
                foreach (var area in WindowAreas)
                {
                    if (index >= area.StartIndex && index < area.StartIndex + area.Length)
                        return area[index - area.StartIndex];
                }
                throw new IndexOutOfRangeException();
            }
            set
            {
                foreach (var area in WindowAreas)
                {
                    if (index >= area.StartIndex && index < area.StartIndex + area.Length)
                    {
                        var eventArgs = new WindowChangeEventArgs(index, value);
                        OnWindowChange(eventArgs);
                        if (!eventArgs.Handled)
                            area[index - area.StartIndex] = value;
                        return;
                    }
                }
                throw new IndexOutOfRangeException();
            }
        }

        public virtual bool PickUpStack(ItemStack slot)
        {
            throw new NotSupportedException();
        }

        protected internal virtual void OnWindowChange(WindowChangeEventArgs e)
        {
            if (WindowChange != null)
                WindowChange(this, e);
        }

        public event EventHandler Disposed;

        public virtual void Dispose()
        {
            for (int i = 0; i < WindowAreas.Length; i++)
            {
                WindowAreas[i].Dispose();
            }
            WindowChange = null;
            if (Disposed != null)
                Disposed(this, null);
            Client = null;
            IsDisposed = true;
        }

        public virtual short[] ReadOnlySlots
        {
            get { return new short[0]; }
        }

        public static void HandleClickPacket(ClickWindowPacket packet, IWindow window, ref ItemStack itemStaging)
        {
            if (packet.SlotIndex >= window.Length || packet.SlotIndex < 0)
                return;
            var existing = window[packet.SlotIndex];
            if (window.ReadOnlySlots.Contains(packet.SlotIndex))
            {
                if (itemStaging.ID == existing.ID || itemStaging.Empty)
                {
                    if (itemStaging.Empty)
                        itemStaging = existing;
                    else
                        itemStaging.Count += existing.Count;
                    window[packet.SlotIndex] = ItemStack.EmptyStack;
                }
                return;
            }
            if (itemStaging.Empty) // Picking up something
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
                        itemStaging = existing;
                        itemStaging.Count += mod;
                        window[packet.SlotIndex] = existing;
                    }
                    else
                    {
                        itemStaging = window[packet.SlotIndex];
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
                        var newItem = (ItemStack)itemStaging.Clone();
                        newItem.Count = 1;
                        itemStaging.Count--;
                        window[packet.SlotIndex] = newItem;
                    }
                    else
                    {
                        window[packet.SlotIndex] = itemStaging;
                        itemStaging = ItemStack.EmptyStack;
                    }
                }
                else
                {
                    if (existing.CanMerge(itemStaging)) // Merge items
                    {
                        // TODO: Consider the maximum stack size
                        if (packet.RightClick)
                        {
                            existing.Count++;
                            itemStaging.Count--;
                            window[packet.SlotIndex] = existing;
                        }
                        else
                        {
                            existing.Count += itemStaging.Count;
                            window[packet.SlotIndex] = existing;
                            itemStaging = ItemStack.EmptyStack;
                        }
                    }
                    else // Swap items
                    {
                        window[packet.SlotIndex] = itemStaging;
                        itemStaging = existing;
                    }
                }
            }
        }
    }
}
