using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using HyperVRemote.Source.Interface;
using Microsoft.Extensions.Options;

namespace HyperVRemote.Source.Implementation
{

    public class HyperVProvider : IHyperVProvider
    {
        private readonly IOptions<HyperVRemoteOptions> _options;

        public ConnectionOptions ConnectionOptions { get; }

        private ManagementScope _scope;

        public HyperVProvider(IOptions<HyperVRemoteOptions> options)
        {
            _options = options;

            var connectionOptions = new ConnectionOptions();
            connectionOptions.Locale = @"en-US";

            var domain = _options.Value.Domain;
            if (!string.IsNullOrWhiteSpace(domain))
            {
                connectionOptions.Authority = "ntlmdomain:" + domain;
            }

            var userName = options.Value.HyperVUserName;
            if (!string.IsNullOrWhiteSpace(userName))
            {
                connectionOptions.Username = userName;
            }

            var password = options.Value.HyperVUserPassword;
            if (!string.IsNullOrWhiteSpace(password))
            {
                connectionOptions.Password = password;
            }


            connectionOptions.Timeout = options.Value.Timeout;
            connectionOptions.Impersonation = ImpersonationLevel.Impersonate;
            connectionOptions.Authentication = AuthenticationLevel.Default;

            //configuration.FetchPassword(),
            //    null,
            //    ImpersonationLevel.Impersonate, // I don't know if this is correct, but it worked for me
            //    AuthenticationLevel.Default,
            //    false,
            //    null,
            // configuration.Timeout());

            ConnectionOptions = connectionOptions;
            _options = options;
        }

        public void Connect()
        {
            _scope = new ManagementScope(new ManagementPath
            {
                Server = _options.Value.HyperVServerName,
                NamespacePath = _options.Value.HyperVNameSpace
            }, ConnectionOptions);

            _scope.Connect();
        }

        public IEnumerable<IHyperVMachine> GetMachines()
        {
            var en = new ManagementClass(_scope, new ManagementPath("Msvm_ComputerSystem"), null)
                .GetInstances()
                .OfType<ManagementObject>().Where(x => "Virtual Machine" == (string)x["Caption"]);

            List<HyperVMachine> machines = en.Select(machine => new HyperVMachine(machine)).ToList();
            return machines;
        }

        public IHyperVMachine GetMachineByName(string name)
        {
            var en = new ManagementClass(_scope, new ManagementPath("Msvm_ComputerSystem"), null)
               .GetInstances()
               .OfType<ManagementObject>().Where(x => "Virtual Machine" == (string)x["Caption"]);

            return new HyperVMachine(en.First(x => x["ElementName"] as string == name));
        }    

       
    }
}
