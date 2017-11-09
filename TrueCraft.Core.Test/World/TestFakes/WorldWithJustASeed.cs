
using TrueCraft.API.World;

namespace TrueCraft.Core.Test.World.TestFakes
{
    public class WorldWithJustASeed : IWorldSeed
    {
        public WorldWithJustASeed(int seed)
        {
            this.Seed = seed;
        }

        public int Seed { get; set; }
    }
}
