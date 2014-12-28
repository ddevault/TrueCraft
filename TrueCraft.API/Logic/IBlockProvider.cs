using System;

namespace TrueCraft.API.Logic
{
    public interface IBlockProvider
    {
        byte ID { get; }
        double Hardness { get; }
        string DisplayName { get; }
        Tuple<int, int> TextureMap { get; }
    }
}