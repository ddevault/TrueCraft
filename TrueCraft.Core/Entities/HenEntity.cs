using System;
using TrueCraft.API;

namespace TrueCraft.Core.Entities
{
    public class HenEntity : MobEntity
    {
        public override Size Size
        {
            get
            {
                return new Size(0.4, 0.3, 0.4);
            }
        }

        public override short MaxHealth
        {
            get
            {
                return 4;
            }
        }

        public override sbyte MobType
        {
            get
            {
                return 93;
            }
        }
    }
}