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
                foreach (var type in assembly.GetTypes().Where(t => typeof(IBiomeProvider).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    var instance = (IBiomeProvider)Activator.CreateInstance(type);
                    RegisterBiomeProvider(instance);
                }
            }
        }

        public void RegisterBiomeProvider(IBiomeProvider Provider)
        {
            BiomeProviders[Provider.ID] = Provider;
        }

        public IBiomeProvider GetBiome(byte ID)
        {
            return BiomeProviders[ID];
        }

        public IBiomeProvider GetBiome(double Temperature, double Rainfall)
        {
            List<IBiomeProvider> TemperatureResults = new List<IBiomeProvider>();
            foreach (IBiomeProvider B in BiomeProviders)
            {
                if (B != null && B.Temperature.Equals(Temperature))
                {
                    TemperatureResults.Add(B);
                }
            }

            if (TemperatureResults.Count.Equals(0))
            {
                IBiomeProvider Provider = null;
                float TemperatureDifference = 100.0f;
                foreach (IBiomeProvider B in BiomeProviders)
                {
                    if (B != null)
                    {
                        var Difference = Math.Abs(Temperature - B.Temperature);
                        if (Provider == null || Difference < TemperatureDifference)
                        {
                            Provider = B;
                            TemperatureDifference = (float)Difference;
                        }
                    }
                }
                TemperatureResults.Add(Provider);
            }

            foreach (IBiomeProvider B in BiomeProviders)
            {
                if (B != null && B.Rainfall.Equals(Rainfall) && TemperatureResults.Contains(B))
                {
                    return B;
                }
            }
            IBiomeProvider BProvider = null;
            float RainfallDifference = 100.0f;
            foreach (IBiomeProvider B in BiomeProviders)
            {
                if (B != null)
                {
                    var Difference = Math.Abs(Temperature - B.Temperature);
                    if (BProvider == null || Difference < RainfallDifference)
                    {
                        BProvider = B;
                        RainfallDifference = (float)Difference;
                    }
                }
            }
            return BProvider;
        }
    }
}