using System;

namespace TrueCraft.Core
{
    public class TrueCraftUser
    {
        public static string AuthServer = "http://localhost";

        public string Username { get; set; }
        public string SessionId { get; set; }
    }
}