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
        void Handle(IRemoteClient Client, string Alias, string[] Arguments);
        void Help(IRemoteClient Client, string Alias, string[] Arguments);
    }
}