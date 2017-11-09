﻿using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.Core.Logic.Blocks;
using TrueCraft.Core.TerrainGen;
using TrueCraft.Core.TerrainGen.Biomes;
using TrueCraft.Core.TerrainGen.Decorators;
using TrueCraft.Core.Test.Logic;
using TrueCraft.Core.Test.World.TestFakes;
using TrueCraft.Core.World;

namespace TrueCraft.Core.Test.World
{
    [TestFixture]
    public class SugarCaneDecoratorTests
    {
        [Test]
        public void DecoratorGrowsNoInvalidSugarCane()
        {
            var aWorld = new WorldWithJustASeed(9001);
            ISpatialBlockInformationProvider aChunk = new PrimeSugarCaneGrowingSeasonChunk();
            IBiomeRepository aBiomeRepository = new BiomeRepository();
            var decorator = GetDecoratorForTestChunk(aWorld, aChunk, aBiomeRepository);

            decorator.Decorate(aWorld, aChunk, aBiomeRepository, null /* Don't need to fake it if you don't use it. */);

            AssertChunkHasNoSugarCaneInColumnsWhereItShouldNot(aChunk);
        }

        [Test]
        public void DecoratorDoesNotGrowSugarcaneUniformly()
        {
            IWorldSeed aWorld = new WorldWithJustASeed(9001);
            ISpatialBlockInformationProvider aChunk = new PrimeSugarCaneGrowingSeasonChunk();
            IBiomeRepository aBiomeRepository = new BiomeRepository();
            var decorator = GetDecoratorForTestChunk(aWorld, aChunk, aBiomeRepository);

            decorator.Decorate(aWorld, aChunk, aBiomeRepository, null);

            AssertChunkSugarCaneGrowthIsNotUniform(aChunk);
        }

        private void AssertChunkHasNoSugarCaneInColumnsWhereItShouldNot(ISpatialBlockInformationProvider aChunk)
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

        private void AssertChunkSugarCaneGrowthIsNotUniform(ISpatialBlockInformationProvider aChunk)
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

        private static SugarCaneDecorator GetDecoratorForTestChunk(IWorldSeed aWorld, ISpatialBlockInformationProvider aChunk,
            IBiomeRepository aBiomeRepository)
        {
            var decorator = new SugarCaneDecorator(new NoiseAlwaysGrowsSugarCaneInTestBounds());
            aBiomeRepository.RegisterBiomeProvider(new SwamplandBiome());
            return decorator;
        }

        static int CountBlockInColumn(ISpatialBlockInformationProvider aChunk, int x, int z, byte blockId)
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

        [Test]
        public void TestUsingAMock()
        {
            Mock<IWorld> aWorld = new Mock<IWorld>();
            aWorld.Setup(foo => foo.Seed).Returns(9001);

            var ourDictionary = PrimeSugarCaneGrowingSeasonChunk.createStartingBlockDictionary();

            Mock<ISpatialBlockInformationProvider> aChunk = new Mock<ISpatialBlockInformationProvider>();

            aChunk.Setup(foo => foo.GetBlockID(It.IsAny<Coordinates3D>())).Returns((Coordinates3D coordinates) =>
            {
                if (ourDictionary.ContainsKey(coordinates))
                {
                    return ourDictionary[coordinates];
                }
                return AirBlock.BlockID;
            });

            aChunk.Setup(foo => foo.SetBlockID(It.IsAny<Coordinates3D>(),
                It.IsAny<byte>())).Callback<Coordinates3D, byte>((a, b) =>
            {
                ourDictionary[a] = b;
            });

            aChunk.Setup(chunk => chunk.X).Returns(6);
            aChunk.Setup(chunk => chunk.Z).Returns(6);
            aChunk.Setup(chunk => chunk.MaxHeight).Returns(6);

            aChunk.Setup(chunk => chunk.HeightMap).Returns(() =>
            {
                 return Enumerable.Repeat(1, Chunk.Width * Chunk.Height).ToArray<int>();
            });

            aChunk.Setup(chunk => chunk.Biomes).Returns(() =>
            {
                return Enumerable.Repeat(new SwamplandBiome().ID, Chunk.Width * Chunk.Height).ToArray<byte>();
            });

            aChunk.Setup(chunk => chunk.GetHeight(It.IsAny<byte>(), It.IsAny<byte>())).Returns(1);

            

            IBiomeRepository aBiomeRepository = new BiomeRepository();
            var decorator = GetDecoratorForTestChunk(aWorld.Object, aChunk.Object, aBiomeRepository);

            decorator.Decorate(aWorld.Object, aChunk.Object, aBiomeRepository, null);

            AssertChunkSugarCaneGrowthIsNotUniform(aChunk.Object);
        }
    }
}