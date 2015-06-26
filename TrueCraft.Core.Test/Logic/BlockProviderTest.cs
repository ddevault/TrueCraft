using System;
using NUnit.Framework;
using Moq;
using Moq.Protected;
using TrueCraft.API.Logic;
using TrueCraft.Core.Logic;
using TrueCraft.API;
using TrueCraft.API.World;
using TrueCraft.API.Server;
using TrueCraft.API.Networking;
using TrueCraft.API.Entities;
using TrueCraft.Core.Entities;
using TrueCraft.Core.TerrainGen;

namespace TrueCraft.Core.Test.Logic
{
    [TestFixture]
    public class BlockProviderTest
    {
        public Mock<IWorld> World { get; set; }
        public Mock<IMultiplayerServer> Server { get; set; }
        public Mock<IEntityManager> EntityManager { get; set; }
        public Mock<IRemoteClient> User { get; set; }
        public Mock<IBlockRepository> BlockRepository { get; set; }

        [TestFixtureSetUp]
        public void SetUp()
        {
            World = new Mock<IWorld>();
            Server = new Mock<IMultiplayerServer>();
            EntityManager = new Mock<IEntityManager>();
            User = new Mock<IRemoteClient>();
            BlockRepository = new Mock<IBlockRepository>();
            var itemRepository = new ItemRepository();
            BlockProvider.BlockRepository = BlockRepository.Object;
            BlockProvider.ItemRepository = itemRepository;

            User.SetupGet(u => u.World).Returns(World.Object);
            User.SetupGet(u => u.Server).Returns(Server.Object);

            World.Setup(w => w.SetBlockID(It.IsAny<Coordinates3D>(), It.IsAny<byte>()));

            Server.Setup(s => s.GetEntityManagerForWorld(It.IsAny<IWorld>()))
                .Returns<IWorld>(w => EntityManager.Object);
            Server.SetupGet(s => s.BlockRepository).Returns(BlockRepository.Object);

            EntityManager.Setup(m => m.SpawnEntity(It.IsAny<IEntity>()));
        }

        protected void ResetMocks()
        {
            World.ResetCalls();
            Server.ResetCalls();
            EntityManager.ResetCalls();
            User.ResetCalls();
        }

        [Test]
        public void TestBlockMined()
        {
            ResetMocks();
            var blockProvider = new Mock<BlockProvider> { CallBase = true };
            var descriptor = new BlockDescriptor
            {
                ID = 10,
                Coordinates = Coordinates3D.Zero
            };

            blockProvider.Object.BlockMined(descriptor, BlockFace.PositiveY, World.Object, User.Object);
            EntityManager.Verify(m => m.SpawnEntity(It.Is<ItemEntity>(e => e.Item.ID == 10)));
            World.Verify(w => w.SetBlockID(Coordinates3D.Zero, 0));

            blockProvider.Protected().Setup<ItemStack[]>("GetDrop", ItExpr.IsAny<BlockDescriptor>(), ItExpr.IsAny<ItemStack>())
                .Returns(() => new[] { new ItemStack(12) });
            blockProvider.Object.BlockMined(descriptor, BlockFace.PositiveY, World.Object, User.Object);
            EntityManager.Verify(m => m.SpawnEntity(It.Is<ItemEntity>(e => e.Item.ID == 12)));
            World.Verify(w => w.SetBlockID(Coordinates3D.Zero, 0));
        }

        [Test]
        public void TestSupport()
        {
            // We need an actual world for this
            var world = new TrueCraft.Core.World.World("test", new FlatlandGenerator());
            world.SetBlockID(Coordinates3D.Zero, 1);
            world.SetBlockID(Coordinates3D.OneY, 2);

            var blockProvider = new Mock<BlockProvider> { CallBase = true };
            var updated = new BlockDescriptor { ID = 2, Coordinates = Coordinates3D.Up };
            var source = new BlockDescriptor { ID = 2, Coordinates = Coordinates3D.Right };
            blockProvider.Setup(b => b.GetSupportDirection(It.IsAny<BlockDescriptor>())).Returns(Coordinates3D.Down);

            var supportive = new Mock<IBlockProvider>();
            supportive.SetupGet(p => p.Opaque).Returns(true);
            var unsupportive = new Mock<IBlockProvider>();
            unsupportive.SetupGet(p => p.Opaque).Returns(false);

            BlockRepository.Setup(r => r.GetBlockProvider(It.Is<byte>(b => b == 1))).Returns(supportive.Object);
            BlockRepository.Setup(r => r.GetBlockProvider(It.Is<byte>(b => b == 3))).Returns(unsupportive.Object);

            blockProvider.Object.BlockUpdate(updated, source, Server.Object, world);
            World.Verify(w => w.SetBlockID(Coordinates3D.OneY, 0), Times.Never);

            world.SetBlockID(Coordinates3D.Zero, 3);

            blockProvider.Object.BlockUpdate(updated, source, Server.Object, world);
            Assert.AreEqual(0, world.GetBlockID(Coordinates3D.OneY));
            EntityManager.Verify(m => m.SpawnEntity(It.Is<ItemEntity>(e => e.Item.ID == 2)));
        }
    }
}