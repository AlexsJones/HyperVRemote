using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using HyperVRemote.Source.Interface;

namespace HyperVRemote.Source.Implementation
{
    public class HyperVConfiguration : IHyperVConfiguration
    {
        public string HyperVUserName { get; }

        public string HyperVUserPassword { get; }

        public string HyperVServerName { get; }

        public string HyperVNameSpace { get; }

        public HyperVConfiguration(string username, string userpassword,
            string servername, string nameSpace)
        {
            HyperVUserName = username;

            HyperVUserPassword = userpassword;

            HyperVServerName = servername;

            HyperVNameSpace = nameSpace;
        }

        public string FetchUsername()
        {
            return HyperVUserName;        
        }

        public string FetchPassword()
        {
            return HyperVUserPassword;
        }

        public string FetchServer()
        {
            return HyperVServerName;
        }

        public string FetchNamespacePath()
        {
            return HyperVNameSpace;
        }

        public TimeSpan Timeout()
        {
            return TimeSpan.FromSeconds(15);
        }
    }
}
