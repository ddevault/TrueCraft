using System;
using TrueCraft.API.Logic;
using TrueCraft.Core.Logic.Items;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Blocks
{
    public class TallGrassBlock : BlockProvider
    {
        public enum TallGrassType
        {
            DeadBush = 0,
            TallGrass = 1,
            Fern = 2
        }

        public static readonly byte BlockID = 0x1F;
        
        public override byte ID { get { return 0x1F; } }
        
        public override double BlastResistance { get { return 0; } }

        public override double Hardness { get { return 0; } }

        public override byte Luminance { get { return 0; } }

        public override bool Opaque { get { return false; } }
        
        public override string DisplayName { get { return "Tall Grass"; } }

        public override BoundingBox? BoundingBox { get { return null; } }

        public override Coordinates3D GetSupportDirection(BlockDescriptor descriptor)
        {
            return Coordinates3D.Down;
        }

        public override Tuple<int, int> GetTextureMap(byte metadata)
        {
            return new Tuple<int, int>(7, 2);
        }

        protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
        {
            return new[] { new ItemStack(SeedsItem.ItemID, (sbyte)MathHelper.Random.Next(2)) };
        }
    }
}