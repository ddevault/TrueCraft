using System;
using TrueCraft.API;

namespace TrueCraft.Core.Entities
{
    public class WolfEntity : MobEntity
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
                return 10;
            }
        }

        public override sbyte MobType
        {
            get
            {
                return 95;
            }
        }
    }
}