using System;
using TrueCraft.API.Logic;
using System.Collections.Generic;
using System.Linq;
using TrueCraft.API.Entities;
using TrueCraft.API;
using TrueCraft.API.World;

namespace TrueCraft.Core.Logic
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
            // TODO: Binary search
            for (int i = 0; i < ItemProviders.Count; i++)
            {
                if (ItemProviders[i].ID == id)
                    return ItemProviders[i];
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

        public void DiscoverItemProviders()
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