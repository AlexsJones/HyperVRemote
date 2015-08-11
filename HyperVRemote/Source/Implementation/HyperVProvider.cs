using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using HyperVRemote.Source.Interface;

namespace HyperVRemote.Source.Implementation
{

    public class HyperVProvider : IHyperVProvider
    {
        private readonly IHyperVConfiguration _configuration;
        public ConnectionOptions Options { get; }

        private ManagementScope _scope;

        public HyperVProvider(IHyperVConfiguration configuration)
        {
            Options = new ConnectionOptions(
                @"en-US",
                configuration.FetchUsername(),
                configuration.FetchPassword(),
                null,
                ImpersonationLevel.Impersonate, // I don't know if this is correct, but it worked for me
                AuthenticationLevel.Default,
                false,
                null,
                configuration.Timeout());

            _configuration = configuration;
        }

        public HyperVStatus GetStatus(IHyperVMachine machine)
        {
            if (_scope == null)
            {
                throw new HyperVException("No management scope present");
            }

            var rawMachine = ((HyperVMachine) machine).FetchRawMachine();


            return (HyperVStatus)rawMachine["EnabledState"];
        }

        public void Connect()
        {
            _scope = new ManagementScope(new ManagementPath
            {
                Server = _configuration.FetchServer(),
                NamespacePath = _configuration.FetchNamespacePath()
            }, Options);

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

        public string GetName(IHyperVMachine machine)
        {
            return machine.FetchName();
        }

        public void Reset(IHyperVMachine machine)
        {
            ChangeState(machine, HyperVStatus.Reset);
        }

        public void Start(IHyperVMachine machine)
        {
            ChangeState(machine, HyperVStatus.Running);
        }

        public void Stop(IHyperVMachine machine)
        {
            ChangeState(machine, HyperVStatus.Off);
        }

        public void RestoreLastSnapShot(IHyperVMachine machine)
        {
            var raw = ((HyperVMachine) machine).FetchRawMachine();

            var lastSnapshot = raw.GetRelated(
                "Msvm_VirtualSystemSettingData",
                "Msvm_MostCurrentSnapshotInBranch",
                null,
                null,
                "Dependent",
                "Antecedent",
                false,
                null).OfType<ManagementObject>().FirstOrDefault();

            if (lastSnapshot == null)
            {
                throw new HyperVException("No Snapshot found");
            }
            
            var managementService = new ManagementClass(_scope, new ManagementPath("Msvm_VirtualSystemSnapshotService"), null)
                .GetInstances()
                .OfType<ManagementObject>().FirstOrDefault();

            var inParameters = managementService.GetMethodParameters("ApplySnapshot");

            inParameters["Snapshot"] = lastSnapshot.Path.Path;

            var outParameters = managementService.InvokeMethod("ApplySnapshot", inParameters, null);

        }

        private uint ChangeState(IHyperVMachine machine, HyperVStatus state)
        {
            var rawMachine = ((HyperVMachine)machine).FetchRawMachine();

            var managementService = new ManagementClass(_scope, new ManagementPath("Msvm_VirtualSystemManagementService"), null)
                .GetInstances()
                .OfType<ManagementObject>().FirstOrDefault();

            if (managementService != null)
            {
                var inParameters = managementService.GetMethodParameters("RequestStateChange");

                inParameters["RequestedState"] = (object)state;

                var outParameters = rawMachine.InvokeMethod("RequestStateChange", inParameters, null);

                Debug.WriteLine("Changed state with return " + outParameters);

                if (outParameters != null) return (uint) outParameters["ReturnValue"];
            }
            else
            {
                throw new HyperVException("Could not find machine management service for rstate change");
            }

            return 0;
        }
    }
}
