using System;
using TrueCraft.API.Networking;

namespace TrueCraft.API
{
    public class MetadataInt : MetadataEntry
    {
        public override byte Identifier { get { return 2; } }
        public override string FriendlyName { get { return "int"; } }

        public int Value;

        public static implicit operator MetadataInt(int value)
        {
            return new MetadataInt(value);
        }

        public MetadataInt()
        {
        }

        public MetadataInt(int value)
        {
            Value = value;
        }

        public override void FromStream(IMinecraftStream stream)
        {
            Value = stream.ReadInt32();
        }

        public override void WriteTo(IMinecraftStream stream, byte index)
        {
            stream.WriteUInt8(GetKey(index));
            stream.WriteInt32(Value);
        }
    }
}
