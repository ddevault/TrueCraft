using System;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic
{
    /// <summary>
    /// Provides common implementations of block logic.
    /// </summary>
    public abstract class BlockProvider : IBlockProvider, IItemProvider
    {
        short IItemProvider.ID
        {
            get
            {
                return ID;
            }
        }

        public abstract byte ID { get; }

        public virtual double Hardness { get { return 0; } }

        public virtual string DisplayName { get { return string.Empty; } }

        public virtual Tuple<int, int> GetTextureMap(byte metadata)
        {
            return null;
        }
    }
}