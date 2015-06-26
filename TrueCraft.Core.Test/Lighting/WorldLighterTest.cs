using System;
using NUnit.Framework;
using TrueCraft.API.World;
using TrueCraft.Core.TerrainGen;
using TrueCraft.Core.Lighting;
using TrueCraft.Core.Logic;
using TrueCraft.API;
using TrueCraft.Core.World;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Test.Lighting
{
    [TestFixture]
    public class WorldLighterTest
    {
        [Test]
        public void TestBasicLighting()
        {
            var repository = new BlockRepository();
            repository.RegisterBlockProvider(new GrassBlock());
            repository.RegisterBlockProvider(new DirtBlock());
            repository.RegisterBlockProvider(new AirBlock());
            repository.RegisterBlockProvider(new BedrockBlock());
            var world = new TrueCraft.Core.World.World("TEST", new FlatlandGenerator());
            world.BlockRepository = repository;
            var lighter = new WorldLighter(world, repository);
            world.GetBlockID(Coordinates3D.Zero); // Generate a chunk
            lighter.EnqueueOperation(new BoundingBox(
                    new Vector3(0, 0, 0),
                    new Vector3(16, 128, 16)), true, true);
            while (lighter.TryLightNext())
            {
            }

            // Validate behavior
            for (int y = 0; y < Chunk.Height; y++)
            {
                for (int x = 0; x < Chunk.Width; x++)
                {
                    for (int z = 0; z < Chunk.Depth; z++)
                    {
                        var coords = new Coordinates3D(x, y, z);
                        var sky = world.GetSkyLight(coords);
                        if (y < 3)
                            Assert.AreEqual(0, sky);
                        else
                            Assert.AreEqual(15, sky);
                    }
                }
            }
        }

        [Test]
        public void TestPropegation()
        {
            var repository = new BlockRepository();
            repository.RegisterBlockProvider(new GrassBlock());
            repository.RegisterBlockProvider(new DirtBlock());
            repository.RegisterBlockProvider(new AirBlock());
            repository.RegisterBlockProvider(new BedrockBlock());
            var world = new TrueCraft.Core.World.World("TEST", new FlatlandGenerator());
            world.BlockRepository = repository;
            var lighter = new WorldLighter(world, repository);
            world.GetBlockID(Coordinates3D.Zero); // Generate a chunk
            lighter.EnqueueOperation(new BoundingBox(
                    new Vector3(0, 0, 0),
                    new Vector3(16, 128, 16)), true, true);
            while (lighter.TryLightNext()) // Initial lighting
            {
            }

            world.SetBlockID(new Coordinates3D(5, 3, 5), 0); // Create area that looks like so:
            world.SetBlockID(new Coordinates3D(5, 2, 5), 0); // x x  Light goes like so: |
            world.SetBlockID(new Coordinates3D(5, 1, 5), 0); // x x                      |
            world.SetBlockID(new Coordinates3D(4, 1, 5), 0); //   x                     -/

            lighter.EnqueueOperation(new BoundingBox(new Vector3(5, 2, 5),
                new Vector3(6, 4, 6)), true);

            while (lighter.TryLightNext()) // Test lighting
            {
            }

            Console.WriteLine("Testing {0}", new Coordinates3D(5, 3, 5));
            Assert.AreEqual(15, world.GetSkyLight(new Coordinates3D(5, 3, 5)));
            Console.WriteLine("Testing {0}", new Coordinates3D(5, 2, 5));
            Assert.AreEqual(15, world.GetSkyLight(new Coordinates3D(5, 2, 5)));
            Console.WriteLine("Testing {0}", new Coordinates3D(5, 1, 5));
            Assert.AreEqual(15, world.GetSkyLight(new Coordinates3D(5, 1, 5)));
            Console.WriteLine("Testing {0}", new Coordinates3D(4, 1, 5));
            Assert.AreEqual(14, world.GetSkyLight(new Coordinates3D(4, 1, 5)));
        }
    }
}

