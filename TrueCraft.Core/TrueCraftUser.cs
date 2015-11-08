using System;

namespace TrueCraft.Core
{
    public class TrueCraftUser
    {
        public static string AuthServer = "https://truecraft.io";

        public string Username { get; set; }
        public string SessionId { get; set; }
    }
}