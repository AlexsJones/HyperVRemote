using System;
using HyperVRemote.Source.Interface;

namespace HyperVRemote.Source.Implementation
{
    public class HyperVConfiguration : IHyperVConfiguration
    {
        public string HyperVUserName { get; }

        public string HyperVUserPassword { get; }

        public string HyperVServerName { get; }

        public string HyperVNameSpace { get; }

        public string Domain { get; set; }


        public HyperVConfiguration(
            string username,
            string userpassword,
            string domain,
            string servername, 
            string nameSpace)
        {
            HyperVUserName = username;
            HyperVUserPassword = userpassword;
            Domain = domain;
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

        public string FetchDomain()
        {
            return Domain;
        }
    }
}
