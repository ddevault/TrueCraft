using System;
using TrueCraft.API.Networking;
using TrueCraft.API.Server;

namespace TrueCraft
{
    public interface ICommandHandler
    {
        string Command { get; }
        string Documentation { get; }
        int Arguments { get; }
        void Handle(ChatMessageEventArgs e, string[] Arguments);
    }
}