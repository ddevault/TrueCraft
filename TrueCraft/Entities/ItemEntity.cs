using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API;
using TrueCraft.API.Networking;

namespace TrueCraft.Entities
{
    public class ItemEntity : ObjectEntity
    {
        public static double PickupRange = 2;

        public ItemEntity(Vector3 position, ItemStack item)
        {
            Position = position;
            Item = item;
            SpawnTime = DateTime.Now;
        }

        public ItemStack Item { get; set; }

        private DateTime SpawnTime { get; set; }

        public override IPacket SpawnPacket
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Size Size
        {
            get { return new Size(0.25, 0.25, 0.25); }
        }

        public override byte EntityType
        {
            get { return 2; }
        }

        public override int Data
        {
            get { return 1; }
        }

        public override MetadataDictionary Metadata
        {
            get
            {
                var metadata = base.Metadata;
                metadata[10] = Item;
                return metadata;
            }
        }

        public override bool SendMetadataToClients
        {
            get
            {
                return true;
            }
        }
    }
}
