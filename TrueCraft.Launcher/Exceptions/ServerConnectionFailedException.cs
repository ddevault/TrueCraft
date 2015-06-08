using System;

namespace TrueCraft.Launcher.Exceptions
{
    public class ServerConnectionFailedException : Exception
    {
        public ServerConnectionFailedException(string message)
        {
            Message = message;
        }

        public new string Message { get; set; }
    }
}