using System;
using System.Collections.Generic;
using System.IO;
using TrueCraft.API;
using TrueCraft.API.Networking;
using TrueCraft.API.Server;
using YamlDotNet.Serialization;

namespace TrueCraft
{
    public class AccessConfiguration : IAccessConfiguration
    {
        private IList<string> _blacklist, _whitelist, _oplist; 

        public AccessConfiguration()
        {
            _blacklist = new List<string>();
            _whitelist = new List<string>();
            _oplist = new List<string>();
        }

        [YamlMember(Alias = "blacklist")]
        public IList<string> Blacklist
        {
            get { return _blacklist; }
        }

        [YamlMember(Alias = "whitelist")]
        public IList<string> Whitelist
        {
            get { return _whitelist; }
        }
        
        [YamlMember(Alias = "ops")]
        public IList<string> Oplist
        {
            get { return _oplist; }
        }
    }
}
