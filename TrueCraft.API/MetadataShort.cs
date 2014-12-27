using System;
using TrueCraft.API.Networking;

namespace TrueCraft.API
{
    public class MetadataShort : MetadataEntry
    {
        public override byte Identifier { get { return 1; } }
        public override string FriendlyName { get { return "short"; } }

        public short Value;

        public static implicit operator MetadataShort(short value)
        {
            return new MetadataShort(value);
        }

        public MetadataShort()
        {
        }

        public MetadataShort(short value)
        {
            Value = value;
        }

        public override void FromStream(IMinecraftStream stream)
        {
            Value = stream.ReadInt16();
        }

        public override void WriteTo(IMinecraftStream stream, byte index)
        {
            stream.WriteUInt8(GetKey(index));
            stream.WriteInt16(Value);
        }
    }
}
