using System;

namespace TrueCraft.API
{
    [Flags]
    public enum ToolType
    {
        None = 1,
        Pickaxe = 2,
        Axe = 4,
        Shovel = 8,
        Hoe = 16,
        Sword = 32,
        All = None | Pickaxe | Axe | Shovel | Hoe | Sword
    }
}