using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API.World
{
    public interface IBiomeRepository
    {
        IBiomeProvider GetBiome(byte ID);
        IBiomeProvider GetBiome(double Temperature, double RainFall);
        void RegisterBiomeProvider(IBiomeProvider Provider);
    }
}