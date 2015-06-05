using System;
using NUnit.Framework;
using TrueCraft.API;

namespace TrueCraft.Core.Test
{
    [TestFixture]
    public class MathHelperTest
    {
        [Test]
        public void TestCreateRotationByte()
        {
            byte a = (byte)MathHelper.CreateRotationByte(0);
            byte b = (byte)MathHelper.CreateRotationByte(180);
            byte c = (byte)MathHelper.CreateRotationByte(359);
            byte d = (byte)MathHelper.CreateRotationByte(360);
            Assert.AreEqual(0, a);
            Assert.AreEqual(128, b);
            Assert.AreEqual(255, c);
            Assert.AreEqual(0, d);
        }

        [Test]
        public void TestGetCollisionPoint()
        {
            var inputs = new[]
            {
                Vector3.Down,
                Vector3.Up,
                Vector3.Left,
                Vector3.Right,
                Vector3.Forwards,
                Vector3.Backwards
            };
            var results = new[]
            {
                MathHelper.GetCollisionPoint(inputs[0]),
                MathHelper.GetCollisionPoint(inputs[1]),
                MathHelper.GetCollisionPoint(inputs[2]),
                MathHelper.GetCollisionPoint(inputs[3]),
                MathHelper.GetCollisionPoint(inputs[4]),
                MathHelper.GetCollisionPoint(inputs[5])
            };
            var expected = new[]
            {
                CollisionPoint.NegativeY,
                CollisionPoint.PositiveY,
                CollisionPoint.NegativeX,
                CollisionPoint.PositiveX,
                CollisionPoint.PositiveZ,
                CollisionPoint.NegativeZ
            };
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], results[i]);
            }
        }
    }
}