using System;
using TrueCraft.API;
using YamlDotNet.Serialization;

namespace TrueCraft
{
    public class Configuration
    {
        public Configuration()
        {
            MOTD = ChatColor.Red + "Welcome to TrueCraft!";
        }

        [YamlMember(Alias="motd")]
        public string MOTD { get; set; }
    }
}