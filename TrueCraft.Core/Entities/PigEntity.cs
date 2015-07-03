using System;
using TrueCraft.API;

namespace TrueCraft.Core.Entities
{
    public class PigEntity : MobEntity
    {
        public override Size Size
        {
            get
            {
                return new Size(0.9);
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
                return 90;
            }
        }
    }
}