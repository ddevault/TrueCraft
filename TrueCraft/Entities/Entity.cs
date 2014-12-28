using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TrueCraft.API.Entities;
using TrueCraft.API;

namespace TrueCraft.Entities
{
    public abstract class Entity : IEntity
    {
        protected Entity()
        {
            EnablePropertyChange = true;
            EntityID = -1;
        }

        public int EntityID { get; set; }

        protected Vector3 _Position;
        public virtual Vector3 Position
        {
            get { return _Position; }
            set
            {
                _Position = value;
                OnPropertyChanged("Position");
            }
        }

        protected Vector3 _Velocity;
        public virtual Vector3 Velocity
        {
            get { return _Velocity; }
            set
            {
                _Velocity = value;
                OnPropertyChanged("Velocity");
            }
        }

        protected float _Yaw;
        public float Yaw
        {
            get { return _Yaw; }
            set
            {
                _Yaw = value;
                OnPropertyChanged("Yaw");
            }
        }

        protected float _Pitch;
        public float Pitch
        {
            get { return _Pitch; }
            set
            {
                _Pitch = value;
                OnPropertyChanged("Pitch");
            }
        }

        public abstract Size Size { get; }

        public virtual bool SendMetadataToClients { get { return false; } }

        public virtual MetadataDictionary Metadata
        {
            get
            {
                var dictionary = new MetadataDictionary();
                dictionary[0] = new MetadataByte(0); // Flags
                dictionary[1] = new MetadataShort(300);
                return dictionary;
            }
        }

        public virtual void Update(Entity[] nearbyEntities)
        {
        }

        protected bool EnablePropertyChange { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected internal virtual void OnPropertyChanged(string property)
        {
            if (!EnablePropertyChange) return;
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
