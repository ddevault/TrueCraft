using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrueCraft.API.Networking;

namespace TrueCraft.API.Server
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        string[] Aliases { get; }
        void Handle(IRemoteClient Client, string alias, string[] arguments);
        void Help(IRemoteClient client, string alias, string[] arguments);
    }
}