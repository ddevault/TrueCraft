using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Windows;
using TrueCraft.API;

namespace TrueCraft.Core.Windows
{
    public abstract class Window : IWindow
    {
        public abstract IWindowArea[] WindowAreas { get; protected set; }

        public event EventHandler<WindowChangeEventArgs> WindowChange;

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

        /// <summary>
        /// When shift-clicking items between areas, this method is used
        /// to determine which area links to which.
        /// </summary>
        /// <param name="index">The index of the area the item is coming from</param>
        /// <param name="slot">The item being moved</param>
        /// <returns>The area to place the item into</returns>
        protected abstract IWindowArea GetLinkedArea(int index, ItemStack slot);

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

        public int Length
        {
            get 
            {
                return WindowAreas.Sum(a => a.Length);
            }
        }

        public bool Empty
        {
            get
            {
                return !WindowAreas.Any(a => a.Items.Any(i => !i.Empty));
            }
        }

        public ItemStack[] GetSlots()
        {
            int length = WindowAreas.Sum(area => area.Length);
            var slots = new ItemStack[length];
            foreach (var windowArea in WindowAreas)
                Array.Copy(windowArea.Items, 0, slots, windowArea.StartIndex, windowArea.Length);
            return slots;
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

        protected internal virtual void OnWindowChange(WindowChangeEventArgs e)
        {
            if (WindowChange != null)
                WindowChange(this, e);
        }
    }
}
