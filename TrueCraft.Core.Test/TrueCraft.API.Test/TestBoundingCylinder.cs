using NUnit.Framework;
using System;
namespace TrueCraft.API.Test
{
    [TestFixture]
    public class TestBoundingCylinder
    {
        [Test]
        public void TestIntersectsPoint()
        {
            //   x
            //  /
            // x
            var cylinder = new BoundingCylinder(Vector3.Zero, Vector3.One, 1);
            Assert.IsTrue(cylinder.Intersects(cylinder.Min));
            Assert.IsTrue(cylinder.Intersects(cylinder.Max));
            Assert.IsTrue(cylinder.Intersects(cylinder.Min + (Vector3.One / 2)));
            Assert.IsTrue(cylinder.Intersects(cylinder.Max - (Vector3.One / 2)));
            Assert.IsTrue(cylinder.Intersects(new Vector3(0.25, 0, 0)));
            Assert.IsFalse(cylinder.Intersects(new Vector3(5, 5, 5)));
        }
        
        [Test]
        public void TestIntersectsBox()
        {
            //   x
            //  /
            // x
            var cylinder = new BoundingCylinder(Vector3.Zero, Vector3.One * 10, 3);
            var doesNotIntersect = new BoundingBox(Vector3.One * 10 + 5, Vector3.One * 10 + 5);
            Assert.IsFalse(cylinder.Intersects(doesNotIntersect));
            var intersects = new BoundingBox(Vector3.Zero, Vector3.One);
            Assert.IsTrue(cylinder.Intersects(intersects));
        }
    }
}

