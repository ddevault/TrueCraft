using System;
using TrueCraft.API.Networking;
using TrueCraft.API.Logging;

namespace TrueCraft.Core.Logging
{
    public static class PacketLogging
    {
        public static void Log(this IPacket packet, ILogProvider log, bool clientToServer = false)
        {
            if (clientToServer)
                log.Log(LogCategory.Packets, "[CLIENT > SERVER] 0x{0:X2} {1}", packet.ID, packet.GetType().Name);
            else
                log.Log(LogCategory.Packets, "[SERVER > CLIENT] 0x{0:X2} {1}", packet.ID, packet.GetType().Name);
            foreach (var prop in packet.GetType().GetFields())
                log.Log(LogCategory.Packets, "\t{0} ({1}): {2}", prop.Name, prop.FieldType.Name, prop.GetValue(packet));
            log.Log(LogCategory.Packets, ""); // newline
            // TODO: Log packet payload
        }
    }
}