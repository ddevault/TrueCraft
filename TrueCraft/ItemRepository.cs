using System;
using TrueCraft.API.Logic;
using System.Collections.Generic;
using System.Linq;
using TrueCraft.API.Entities;
using TrueCraft.API;
using TrueCraft.API.World;

namespace TrueCraft
{
    public class ItemRepository : IItemRepository
    {
        public ItemRepository()
        {
            ItemProviders = new List<IItemProvider>();
        }

        private readonly List<IItemProvider> ItemProviders = new List<IItemProvider>();

        public IItemProvider GetItemProvider(short id)
        {
            int max = ItemProviders.Count - 1, min = 0;
            while (max >= min)
            {
                int mid = (max - min / 2) + min;
                if (ItemProviders[mid].ID == id)
                    return ItemProviders[mid];
                else if(ItemProviders[mid].ID < id)
                    min = mid + 1;
                else
                    max = min - 1;
            }
            return null;
        }

        public void RegisterItemProvider(IItemProvider provider)
        {
            int i;
            for (i = ItemProviders.Count - 1; i >= 0; i--)
            {
                if (provider.ID == ItemProviders[i].ID)
                {
                    ItemProviders[i] = provider; // Override
                    return;
                }
                if (ItemProviders[i].ID < provider.ID)
                    break;
            }
            ItemProviders.Insert(i + 1, provider);
        }

        internal void DiscoverItemProviders()
        {
            var providerTypes = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes().Where(t =>
                    typeof(IItemProvider).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    providerTypes.Add(type);
                }
            }

            providerTypes.ForEach(t =>
            {
                var instance = (IItemProvider)Activator.CreateInstance(t);
                RegisterItemProvider(instance);
            });
        }
    }
}