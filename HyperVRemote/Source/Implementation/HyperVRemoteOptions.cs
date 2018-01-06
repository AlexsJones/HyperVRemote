using System;

namespace HyperVRemote.Source.Implementation
{
    public class HyperVRemoteOptions
    {
        public static string DefaultNamespace = @"root\virtualization\v2";
        public static string DefaultUsername = "";
        public static string DefaultPassword = "";
        public static string DefaultServerName = ".";
        public static string DefaultDomain = "";
        public static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(15);

        public HyperVRemoteOptions()
        {
            HyperVUserName = DefaultUsername;
            HyperVUserPassword = DefaultPassword;
            Domain = DefaultDomain;
            HyperVServerName = DefaultServerName;
            HyperVNameSpace = DefaultNamespace;
            Timeout = DefaultTimeout;
        }

        public string HyperVUserName { get; set; }

        public string HyperVUserPassword { get; set; }

        public string HyperVServerName { get; set; }

        public string HyperVNameSpace { get; set; }

        public string Domain { get; set; }

        public TimeSpan Timeout { get; set; }
       
    }
}
