using System;
using NUnit.Framework;
using TrueCraft.Core.World;
using TrueCraft.API;
using fNbt;
using TrueCraft.Core.Logic.Blocks;
using System.IO;
using System.Reflection;

namespace TrueCraft.Core.Test.World
{
    [TestFixture]
    public class ChunkTest
    {
        [Test]
        public void TestGetBlockID()
        {
            var chunk = new Chunk();
            chunk.SetBlockID(Coordinates3D.Zero, 12);
            Assert.AreEqual(12, chunk.GetBlockID(Coordinates3D.Zero));
        }

        [Test]
        public void TestGetBlockLight()
        {
            var chunk = new Chunk();
            chunk.SetBlockLight(Coordinates3D.Zero, 5);
            Assert.AreEqual(5, chunk.GetBlockLight(Coordinates3D.Zero));
        }

        [Test]
        public void TestGetSkyLight()
        {
            var chunk = new Chunk();
            chunk.SetSkyLight(Coordinates3D.Zero, 5);
            Assert.AreEqual(5, chunk.GetSkyLight(Coordinates3D.Zero));
        }

        [Test]
        public void TestGetMetadata()
        {
            var chunk = new Chunk();
            chunk.SetMetadata(Coordinates3D.Zero, 5);
            Assert.AreEqual(5, chunk.GetMetadata(Coordinates3D.Zero));
        }

        [Test]
        public void TestHeightMap()
        {
            var chunk = new Chunk();
            for (int x = 0; x < Chunk.Width; ++x)
            for (int z = 0; z < Chunk.Width; ++z)
                chunk.SetBlockID(new Coordinates3D(x, 20, z), StoneBlock.BlockID);
            chunk.UpdateHeightMap();
            Assert.AreEqual(20, chunk.GetHeight(0, 0));
            Assert.AreEqual(20, chunk.GetHeight(1, 0));
            chunk.SetBlockID(new Coordinates3D(1, 80, 0), 1);
            Assert.AreEqual(80, chunk.GetHeight(1, 0));
        }

        [Test]
        public void TestConsistency()
        {
            var chunk = new Chunk();
            byte val = 0;
            for (int y = 0; y < Chunk.Height; y++)
            for (int x = 0; x < Chunk.Width; x++)
            for (int z = 0; z < Chunk.Depth; z++)
            {
                var coords = new Coordinates3D(x, y, z);
                chunk.SetBlockID(coords, val);
                chunk.SetMetadata(coords, (byte)(val % 16));
                chunk.SetBlockLight(coords, (byte)(val % 16));
                chunk.SetSkyLight(coords, (byte)(val % 16));
                val++;
            }
            val = 0;
            for (int y = 0; y < Chunk.Height; y++)
            for (int x = 0; x < Chunk.Width; x++)
            for (int z = 0; z < Chunk.Depth; z++)
            {
                var coords = new Coordinates3D(x, y, z);
                Assert.AreEqual(val, chunk.GetBlockID(coords));
                Assert.AreEqual((byte)(val % 16), chunk.GetMetadata(coords));
                Assert.AreEqual((byte)(val % 16), chunk.GetBlockLight(coords));
                Assert.AreEqual((byte)(val % 16), chunk.GetSkyLight(coords));
                val++;
            }
        }
    }
}