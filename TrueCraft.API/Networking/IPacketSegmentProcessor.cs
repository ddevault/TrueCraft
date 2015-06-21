using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrueCraft.API.Networking
{
    public interface IPacketSegmentProcessor
    {
        bool ProcessNextSegment(byte[] nextSegment, int offset, int len, out IPacket packet);
    }
}
