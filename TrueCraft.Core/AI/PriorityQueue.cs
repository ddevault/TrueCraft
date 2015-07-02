using System;
using System.Collections.Generic;

namespace TrueCraft.Core.AI
{
    // TODO: Replace this with something better eventually
    // Thanks to www.redblobgames.com/pathfinding/a-star/implementation.html
    public class PriorityQueue<T>
    {
        private List<Tuple<T, int>> elements = new List<Tuple<T, int>>();

        public int Count
        {
            get { return elements.Count; }
        }
        
        public void Enqueue(T item, int priority)
        {
            elements.Add(Tuple.Create(item, priority));
        }

        public T Dequeue()
        {
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++) {
                if (elements[i].Item2 < elements[bestIndex].Item2) {
                    bestIndex = i;
                }
            }

            T bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }
}