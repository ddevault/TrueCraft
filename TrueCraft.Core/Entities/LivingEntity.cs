using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.Core.Entities
{
    public abstract class LivingEntity : Entity
    {
        protected LivingEntity()
        {
            Health = MaxHealth;
        }

        protected short _Air;
        public short Air
        {
            get { return _Air; }
            set
            {
                _Air = value;
                OnPropertyChanged("Air");
            }
        }

        protected short _Health;
        public short Health
        {
            get { return _Health; }
            set
            {
                _Health = value;
                OnPropertyChanged("Health");
            }
        }

        protected float _HeadYaw;
        public float HeadYaw
        {
            get { return _HeadYaw; }
            set
            {
                _HeadYaw = value;
                OnPropertyChanged("HeadYaw");
            }
        }

        public override bool SendMetadataToClients
        {
            get
            {
                return true;
            }
        }

        public abstract short MaxHealth { get; }
    }
}
