using System;
using TrueCraft.API;

namespace TrueCraft.Core.Entities
{
    public class GhastEntity : MobEntity
    {
        public override Size Size
        {
            get
            {
                return new Size(4.0);
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
                return 56;
            }
        }

        public override bool BeginUpdate()
        {
            // Ghasts can fly, no need to work out gravity
            // TODO: Think about how to deal with walls and such
            return false;
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