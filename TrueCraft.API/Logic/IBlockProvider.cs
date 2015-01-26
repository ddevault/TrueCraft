using System;

namespace TrueCraft.API.Logic
{
    public interface IBlockProvider
    {
        byte ID { get; }
        double BlastResistance { get; }
        double Hardness { get; }
        byte Luminance { get; }
        bool Opaque { get; }
        byte LightModifier { get; }
        string DisplayName { get; }
        Tuple<int, int> GetTextureMap(byte metadata);
    }
}