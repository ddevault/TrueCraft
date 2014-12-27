using TrueCraft.API.Networking;

namespace TrueCraft.API
{
    public class MetadataByte : MetadataEntry
    {
        public override byte Identifier { get { return 0; } }
        public override string FriendlyName { get { return "byte"; } }

        public byte Value;

        public static implicit operator MetadataByte(byte value)
        {
            return new MetadataByte(value);
        }

        public MetadataByte()
        {
        }

        public MetadataByte(byte value)
        {
            Value = value;
        }

        public override void FromStream(IMinecraftStream stream)
        {
            Value = stream.ReadUInt8();
        }

        public override void WriteTo(IMinecraftStream stream, byte index)
        {
            stream.WriteUInt8(GetKey(index));
            stream.WriteUInt8(Value);
        }
    }
}
