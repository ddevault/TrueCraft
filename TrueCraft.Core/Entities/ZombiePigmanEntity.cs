using System;
using TrueCraft.API;

namespace TrueCraft.Core.Entities
{
    public class ZombiePigmanEntity : MobEntity
    {
        public override Size Size
        {
            get
            {
                return new Size(0.6, 1.8, 0.6);
            }
        }

        public override short MaxHealth
        {
            get
            {
                return 20;
            }
        }

        public override sbyte MobType
        {
            get
            {
                return 57;
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