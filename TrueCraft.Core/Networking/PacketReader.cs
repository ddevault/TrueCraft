using System;
using TrueCraft.API.Networking;
using TrueCraft.Core.Networking.Packets;

namespace TrueCraft.Core.Networking
{
    public class PacketReader : IPacketReader
    {
        public static readonly int Version = 14;
        public int ProtocolVersion { get { return Version; } }

        private Type[] ClientboundPackets = new Type[0x100];
        private Type[] ServerboundPackets = new Type[0x100];

        /// <summary>
        /// Registers TrueCraft.Core implementations of all packets used by vanilla Minecraft.
        /// </summary>
        public void RegisterCorePackets()
        {
            RegisterPacketType<KeepAlivePacket>();                                                      // 0x00
            RegisterPacketType<LoginRequestPacket>(serverbound: true, clientbound: false);              // 0x01
            RegisterPacketType<LoginResponsePacket>(serverbound: false, clientbound: true);             // 0x01
            RegisterPacketType<HandshakePacket>(serverbound: true, clientbound: false);                 // 0x02
            RegisterPacketType<HandshakeResponsePacket>(serverbound: false, clientbound: true);         // 0x02
            RegisterPacketType<ChatMessagePacket>();                                                    // 0x03
            RegisterPacketType<TimeUpdatePacket>(serverbound: false, clientbound: true);                // 0x04
            RegisterPacketType<EntityEquipmentPacket>(serverbound: false, clientbound: true);           // 0x05 // NOTE: serverbound not confirmed
            RegisterPacketType<SpawnPositionPacket>(serverbound: false, clientbound: true);             // 0x06
            RegisterPacketType<UseEntityPacket>(serverbound: true, clientbound: false);                 // 0x07
            RegisterPacketType<UpdateHealthPacket>(serverbound: false, clientbound: true);              // 0x08
            RegisterPacketType<RespawnPacket>();                                                        // 0x09
            RegisterPacketType<PlayerGroundedPacket>(serverbound: true, clientbound: false);            // 0x0A
            RegisterPacketType<PlayerPositionPacket>(serverbound: true, clientbound: false);            // 0x0B
            RegisterPacketType<PlayerLookPacket>(serverbound: true, clientbound: false);                // 0x0C
            RegisterPacketType<PlayerPositionAndLookPacket>(serverbound: true, clientbound: false);     // 0x0D
            RegisterPacketType<SetPlayerPositionPacket>(serverbound: false, clientbound: true);         // 0x0D
            RegisterPacketType<PlayerDiggingPacket>(serverbound: true, clientbound: false);             // 0x0E
            RegisterPacketType<PlayerBlockPlacementPacket>(serverbound: true, clientbound: false);      // 0x0F
            RegisterPacketType<ChangeHeldItemPacket>(serverbound: true, clientbound: false);            // 0x10
            RegisterPacketType<UseBedPacket>(serverbound: false, clientbound: true);                    // 0x11
            RegisterPacketType<AnimationPacket>();                                                      // 0x12
            RegisterPacketType<PlayerActionPacket>(serverbound: true, clientbound: false);              // 0x13
            RegisterPacketType<SpawnPlayerPacket>(serverbound: false, clientbound: true);               // 0x14
            RegisterPacketType<SpawnItemPacket>(serverbound: true, clientbound: true);                  // 0x15
            RegisterPacketType<CollectItemPacket>(serverbound: false, clientbound: true);               // 0x16
            RegisterPacketType<SpawnGenericEntityPacket>(serverbound: false, clientbound: true);        // 0x17
            RegisterPacketType<SpawnMobPacket>(serverbound: false, clientbound: true);                  // 0x18
            RegisterPacketType<SpawnPaintingPacket>(serverbound: false, clientbound: true);             // 0x19

            RegisterPacketType<EntityVelocityPacket>(serverbound: false, clientbound: true);            // 0x1C
            RegisterPacketType<DestroyEntityPacket>(serverbound: false, clientbound: true);             // 0x1D
            RegisterPacketType<UselessEntityPacket>(serverbound: false, clientbound: true);             // 0x1E
            RegisterPacketType<EntityRelativeMovePacket>(serverbound: false, clientbound: true);        // 0x1F
            RegisterPacketType<EntityLookPacket>(serverbound: false, clientbound: true);                // 0x20
            RegisterPacketType<EntityLookAndRelativeMovePacket>(serverbound: false, clientbound: true); // 0x21
            RegisterPacketType<EntityTeleportPacket>(serverbound: false, clientbound: true);            // 0x22

            RegisterPacketType<EntityStatusPacket>(serverbound: false, clientbound: true);              // 0x26
            RegisterPacketType<AttachEntityPacket>(serverbound: false, clientbound: true);              // 0x27
            RegisterPacketType<EntityMetadataPacket>(serverbound: false, clientbound: true);            // 0x28

            RegisterPacketType<ChunkPreamblePacket>(serverbound: false, clientbound: true);             // 0x32
            RegisterPacketType<ChunkDataPacket>(serverbound: false, clientbound: true);                 // 0x33
            RegisterPacketType<BulkBlockChangePacket>(serverbound: false, clientbound: true);           // 0x34
            RegisterPacketType<BlockChangePacket>(serverbound: false, clientbound: true);               // 0x35
            RegisterPacketType<BlockActionPacket>(serverbound: false, clientbound: true);               // 0x36

            RegisterPacketType<ExplosionPacket>(serverbound: false, clientbound: true);                 // 0x3C
            RegisterPacketType<SoundEffectPacket>(serverbound: false, clientbound: true);               // 0x3D

            RegisterPacketType<EnvironmentStatePacket>(serverbound: false, clientbound: true);          // 0x46
            RegisterPacketType<LightningPacket>(serverbound: false, clientbound: true);                 // 0x47

            RegisterPacketType<OpenWindowPacket>(serverbound: false, clientbound: true);                // 0x64
            RegisterPacketType<CloseWindowPacket>();                                                    // 0x65
            RegisterPacketType<ClickWindowPacket>(serverbound: true, clientbound: false);               // 0x66
            RegisterPacketType<SetSlotPacket>(serverbound: false, clientbound: true);                   // 0x67
            RegisterPacketType<WindowItemsPacket>(serverbound: false, clientbound: true);               // 0x68
            RegisterPacketType<UpdateProgressPacket>(serverbound: false, clientbound: true);            // 0x69
            RegisterPacketType<TransactionStatusPacket>(serverbound: false, clientbound: true);         // 0x6A

            RegisterPacketType<UpdateSignPacket>();                                                     // 0x82
            RegisterPacketType<MapDataPacket>(serverbound: false, clientbound: true);                   // 0x83

            RegisterPacketType<UpdateStatisticPacket>(serverbound: false, clientbound: true);           // 0xC8

            RegisterPacketType<DisconnectPacket>();                                                     // 0xFF
        }

        public void RegisterPacketType<T>(bool clientbound = true, bool serverbound = true) where T : IPacket
        {
            var packet = (IPacket)Activator.CreateInstance(typeof(T));
            if (clientbound)
                ClientboundPackets[packet.ID] = typeof(T);
            if (serverbound)
                ServerboundPackets[packet.ID] = typeof(T);
        }

        public IPacket ReadPacket(IMinecraftStream stream, bool serverbound = true)
        {
            var id = stream.ReadUInt8();
            Type type;
            if (serverbound)
                type = ServerboundPackets[id];
            else
                type = ClientboundPackets[id];
            if (type == null)
                throw new NotSupportedException("Unable to read packet type 0x" + id.ToString("X2"));
            var instance = (IPacket)Activator.CreateInstance(type);
            instance.ReadPacket(stream);
            return instance;
        }

        public void WritePacket(IMinecraftStream stream, IPacket packet)
        {
            stream.WriteUInt8(packet.ID);
            packet.WritePacket(stream);
            stream.BaseStream.Flush();
        }
    }
}