using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.TerrainGen;
using TrueCraft.Core.TerrainGen.Biomes;
using TrueCraft.Core.TerrainGen.Decorators;
using TrueCraft.Core.Test.World.TestFakes;

namespace TrueCraft.Core.Test.World
{
    [TestFixture]
    public class SugarCaneDecoratorTests
    {
        [Test]
        public void DecoratorGrowsNoInvalidSugarCane()
        {
            IWorld aWorld = new WorldWithJustASeed(9001);
            IChunk aChunk = new PrimeSugarCaneGrowingSeasonChunk();
            IBiomeRepository aBiomeRepository = new BiomeRepository();
            var decorator = GetDecoratorForTestChunk(aWorld, aChunk, aBiomeRepository);

            decorator.Decorate(aWorld, aChunk, aBiomeRepository);

            AssertChunkHasNoSugarCaneInColumnsWhereItShouldNot(aChunk);
        }

        [Test]
        public void DecoratorDoesNotGrowSugarcaneUniformly()
        {
            IWorld aWorld = new WorldWithJustASeed(9001);
            IChunk aChunk = new PrimeSugarCaneGrowingSeasonChunk();
            IBiomeRepository aBiomeRepository = new BiomeRepository();
            var decorator = GetDecoratorForTestChunk(aWorld, aChunk, aBiomeRepository);

            decorator.Decorate(aWorld, aChunk, aBiomeRepository);

            AssertChunkSugarCaneGrowthIsNotUniform(aChunk);
        }

        private void AssertChunkHasNoSugarCaneInColumnsWhereItShouldNot(IChunk aChunk)
        {
            for (int x = 0; x < 6; x++)
            {
                for (int z = 0; z < 6; z++)
                {
                    Coordinates2D coord = new Coordinates2D(x, z);
                    if (PrimeSugarCaneGrowingSeasonChunk.PointsWithoutAnySugarcane().Contains(coord))
                    {
                        Assert.AreEqual(0, CountBlockInColumn(aChunk, x, z, SugarcaneBlock.BlockID), string.Format("Sugarcane in column ({0},{1})", x,z));
                    }
                }
            }
        }

        private void AssertChunkSugarCaneGrowthIsNotUniform(IChunk aChunk)
        {
            var counts = new List<double>();
            for (int x = 0; x < 6; x++)
            {
                for (int z = 0; z < 6; z++)
                {
                    Coordinates2D coord = new Coordinates2D(x, z);
                    var countOfSugarCane = CountBlockInColumn(aChunk, x, z, SugarcaneBlock.BlockID);
                    if (countOfSugarCane != 0)
                    {
                        counts.Add(countOfSugarCane);
                    }
                }
            }
            double averageOfSugarCaneHeight = counts.Average();

            for (int i = 0; i < 7; i++)
            {
                Assert.AreNotEqual(i, averageOfSugarCaneHeight, "Sugarcane grew with uniform height.");
            }
        }

        private static SugarCaneDecorator GetDecoratorForTestChunk(IWorld aWorld, IChunk aChunk,
            IBiomeRepository aBiomeRepository)
        {
            var decorator = new SugarCaneDecorator(new NoiseAlwaysGrowsSugarCaneInTestBounds());
            aBiomeRepository.RegisterBiomeProvider(new SwamplandBiome());
            return decorator;
        }

        static int CountBlockInColumn(IChunk aChunk, int x, int z, byte blockId)
        {
            int counter = 0;

            for (int y = 0; y < 7; y++)
            {
                byte block = aChunk.GetBlockID(new Coordinates3D(x: x, y: y, z: z));
                if (block == blockId)
                {
                    counter++;
                }
            }
            return counter;
        }
    }
}