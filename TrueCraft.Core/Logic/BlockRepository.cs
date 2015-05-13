using System;
using TrueCraft.API.Logic;
using System.Collections.Generic;
using System.Linq;
using TrueCraft.API.Entities;
using TrueCraft.API;
using TrueCraft.API.World;

namespace TrueCraft.Core.Logic
{
    public class BlockRepository : IBlockRepository, IBlockPhysicsProvider
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

        public void DiscoverBlockProviders()
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
            
        public BoundingBox? GetBoundingBox(IWorld world, Coordinates3D coordinates)
        {
            // TODO: Block-specific bounding boxes
            var id = world.GetBlockID(coordinates);
            if (id == 0) return null;
            var provider = BlockProviders[id];
            return provider.BoundingBox;
        }
    }
}