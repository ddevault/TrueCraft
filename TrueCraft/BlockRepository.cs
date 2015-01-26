using System;
using TrueCraft.API.Logic;
using System.Collections.Generic;
using System.Linq;

namespace TrueCraft
{
    public class BlockRepository : IBlockRepository
    {
        private readonly IBlockProvider[] BlockProviders = new IBlockProvider[0x100];

        public IBlockProvider GetBlockProvider(byte id)
        {
            return BlockProviders[id];
        }

        public void RegisterBlockProvider(IBlockProvider provider)
        {
            BlockProviders[provider.ID] = provider;
        }

        internal void DiscoverBlockProviders()
        {
            var providerTypes = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes().Where(t =>
                    typeof(IBlockProvider).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    providerTypes.Add(type);
                }
            }

            providerTypes.ForEach(t =>
            {
                var instance = (IBlockProvider)Activator.CreateInstance(t);
                RegisterBlockProvider(instance);
            });
        }
    }
}