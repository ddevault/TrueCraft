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

namespace TrueCraft.Core.Test.Logic
{
    [TestFixture]
    public class BlockProviderTest
    {
        public Mock<IWorld> World { get; set; }
        public Mock<IMultiplayerServer> Server { get; set; }
        public Mock<IEntityManager> EntityManager { get; set; }
        public Mock<IRemoteClient> User { get; set; }

        [TestFixtureSetUp]
        public void SetUp()
        {
            World = new Mock<IWorld>();
            Server = new Mock<IMultiplayerServer>();
            EntityManager = new Mock<IEntityManager>();
            User = new Mock<IRemoteClient>();

            User.SetupGet(u => u.World).Returns(World.Object);
            User.SetupGet(u => u.Server).Returns(Server.Object);

            World.Setup(w => w.SetBlockID(It.IsAny<Coordinates3D>(), It.IsAny<byte>()));

            Server.Setup(s => s.GetEntityManagerForWorld(It.IsAny<IWorld>()))
                .Returns<IWorld>(w => EntityManager.Object);

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

            blockProvider.Protected().Setup<ItemStack[]>("GetDrop", ItExpr.IsAny<BlockDescriptor>())
                .Returns(() => new[] { new ItemStack(12) });
            blockProvider.Object.BlockMined(descriptor, BlockFace.PositiveY, World.Object, User.Object);
            EntityManager.Verify(m => m.SpawnEntity(It.Is<ItemEntity>(e => e.Item.ID == 12)));
            World.Verify(w => w.SetBlockID(Coordinates3D.Zero, 0));
        }
    }
}