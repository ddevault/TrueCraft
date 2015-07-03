using System;
using TrueCraft.API.Networking;
using TrueCraft.API;
using TrueCraft.Core.Networking.Packets;

namespace TrueCraft.Core.Entities
{
    public class SpiderEntity : MobEntity
    {
        public override Size Size
        {
            get
            {
                return new Size(1.4, 0.9, 1.4);
            }
        }

        public override short MaxHealth
        {
            get
            {
                return 16;
            }
        }

        public override sbyte MobType
        {
            get
            {
                return 52;
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

