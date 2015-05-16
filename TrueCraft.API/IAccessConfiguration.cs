using System.Collections.Generic;
using TrueCraft.API.Networking;

namespace TrueCraft.API
{
    public interface IAccessConfiguration
    {
        IList<string> Blacklist { get; }
        IList<string> Whitelist { get; }
        IList<string> Oplist { get; }
    }
}
