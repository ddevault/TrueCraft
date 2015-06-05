using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.World;
using TrueCraft.Core.TerrainGen.Biomes;
using TrueCraft.Core.TerrainGen.Noise;
using System.Reflection;

namespace TrueCraft.Core.TerrainGen
{
    public class BiomeRepository : IBiomeRepository
    {
        private readonly IBiomeProvider[] BiomeProviders = new IBiomeProvider[0x100];

        public BiomeRepository()
        {
            DiscoverBiomes();
        }

        internal void DiscoverBiomes()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypes().Where(t => typeof(IBiomeProvider).IsAssignableFrom(t) && !t.IsAbstract))
                    {
                        var instance = (IBiomeProvider)Activator.CreateInstance(type);
                        RegisterBiomeProvider(instance);
                    }
                }
                catch
                {
                    // There are some bugs with loading mscorlib during a unit test like this
                }
            }
        }

        public void RegisterBiomeProvider(IBiomeProvider provider)
        {
            BiomeProviders[provider.ID] = provider;
        }

        public IBiomeProvider GetBiome(byte id)
        {
            return BiomeProviders[id];
        }

        public IBiomeProvider GetBiome(double temperature, double rainfall)
        {
            List<IBiomeProvider> temperatureResults = new List<IBiomeProvider>();
            foreach (var biome in BiomeProviders)
            {
                if (biome != null && biome.Temperature.Equals(temperature))
                {
                    temperatureResults.Add(biome);
                }
            }

            if (temperatureResults.Count.Equals(0))
            {
                IBiomeProvider provider = null;
                float temperatureDifference = 100.0f;
                foreach (var biome in BiomeProviders)
                {
                    if (biome != null)
                    {
                        var Difference = Math.Abs(temperature - biome.Temperature);
                        if (provider == null || Difference < temperatureDifference)
                        {
                            provider = biome;
                            temperatureDifference = (float)Difference;
                        }
                    }
                }
                temperatureResults.Add(provider);
            }

            foreach (var biome in BiomeProviders)
            {
                if (biome != null && biome.Rainfall.Equals(rainfall) && temperatureResults.Contains(biome))
                {
                    return biome;
                }
            }

            IBiomeProvider biomeProvider = null;
            float rainfallDifference = 100.0f;
            foreach (var biome in BiomeProviders)
            {
                if (biome != null)
                {
                    var difference = Math.Abs(temperature - biome.Temperature);
                    if (biomeProvider == null || difference < rainfallDifference)
                    {
                        biomeProvider = biome;
                        rainfallDifference = (float)difference;
                    }
                }
            }
            return biomeProvider;
        }
    }
}