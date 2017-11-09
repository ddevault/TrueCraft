using TrueCraft.Core.TerrainGen.Noise;

namespace TrueCraft.Core.Test.World.TestFakes
{
    public class NoiseAlwaysGrowsSugarCaneInTestBounds : NoiseGen
    {
        public override double Value2D(double x, double y)
        {
            return x > 5 ? 0 : (y > 5 ? 0 : 1);
        }

        public override double Value3D(double x, double y, double z)
        {
            double value2d = Value2D(x, y);

            return 1;
        }
    }
}
