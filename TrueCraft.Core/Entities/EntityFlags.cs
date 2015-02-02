using System;

namespace TrueCraft.Core.Entities
{
    [Flags]
    public enum EntityFlags
    {
        Fire = 0x01,
        Crouched = 0x02,
        Riding = 0x04,
        Sprinting = 0x08,
        Eating = 0x10,
    }
}