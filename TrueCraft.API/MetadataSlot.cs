using System;
using System.IO;
using fNbt;
using TrueCraft.API.Networking;

namespace TrueCraft.API
{
    public class MetadataSlot : MetadataEntry
    {
        public override byte Identifier { get { return 5; } }
        public override string FriendlyName { get { return "slot"; } }

        public ItemStack Value;

        public static implicit operator MetadataSlot(ItemStack value)
        {
            return new MetadataSlot(value);
        }

        public MetadataSlot()
        {
        }

        public MetadataSlot(ItemStack value)
        {
            Value = value;
        }

        public override void FromStream(IMinecraftStream stream)
        {
            Value = ItemStack.FromStream(stream);
        }

        public override void WriteTo(IMinecraftStream stream, byte index)
        {
            stream.WriteUInt8(GetKey(index));
            stream.WriteInt16(Value.Id);
            if (Value.Id != -1)
            {
                stream.WriteInt8(Value.Count);
                stream.WriteInt16(Value.Metadata);
                if (Value.Nbt != null)
                {
                    var file = new NbtFile(Value.Nbt);
                    var data = file.SaveToBuffer(NbtCompression.GZip);
                    stream.WriteInt16((short)data.Length);
                    stream.WriteUInt8Array(data);
                }
                else
                    stream.WriteInt16(-1);
            }
        }
    }
}
