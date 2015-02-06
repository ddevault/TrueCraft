using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Networking;

namespace TrueCraft.API.Server
{
    public interface ICommandManager
    {
        IList<ICommand> Commands { get; }
        void HandleCommand(IRemoteClient client, string Alias, string[] arguments);
    }
}
