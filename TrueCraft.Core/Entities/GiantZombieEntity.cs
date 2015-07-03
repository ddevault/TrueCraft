using System;
using TrueCraft.API;

namespace TrueCraft.Core.Entities
{
    public class GiantZombieEntity : ZombieEntity
    {
        public override Size Size
        {
            get
            {
                return new Size(0.6, 1.8, 0.6) * 6;
            }
        }

        public override short MaxHealth
        {
            get
            {
                return 100;
            }
        }

        public override sbyte MobType
        {
            get
            {
                return 53;
            }
        }

        public override bool Friendly
        {
            get
            {
                return false;
            }
        }
    }
}

