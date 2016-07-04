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
        public Chunk Chunk { get; set; }

        [TestFixtureSetUp]
        public void SetUp()
        {
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var file = new NbtFile(Path.Combine(assemblyDir, "Files", "TestChunk.nbt"));
            Chunk = Chunk.FromNbt(file);
        }

        [Test]
        public void TestGetBlockID()
        {
            Assert.AreEqual(BedrockBlock.BlockID, Chunk.GetBlockID(Coordinates3D.Zero));
            Chunk.SetBlockID(Coordinates3D.Zero, 12);
            Assert.AreEqual(12, Chunk.GetBlockID(Coordinates3D.Zero));
            Chunk.SetBlockID(Coordinates3D.Zero, BedrockBlock.BlockID);
        }

        [Test]
        public void TestGetBlockLight()
        {
            Assert.AreEqual(0, Chunk.GetBlockLight(Coordinates3D.Zero));
        }

        [Test]
        public void TestGetSkyLight()
        {
            Assert.AreEqual(0, Chunk.GetBlockLight(Coordinates3D.Zero));
        }

        [Test]
        public void TestGetMetadata()
        {
            Assert.AreEqual(0, Chunk.GetBlockLight(Coordinates3D.Zero));
        }

        [Test]
        public void TestHeightMap()
        {
            Chunk.UpdateHeightMap();
            Assert.AreEqual(59, Chunk.GetHeight(0, 0));
            Assert.AreEqual(58, Chunk.GetHeight(1, 0));
            Chunk.SetBlockID(new Coordinates3D(1, 80, 0), 1);
            Assert.AreEqual(80, Chunk.GetHeight(1, 0));
        }
    }
}