using System.Diagnostics;
using System.Linq;
using System.Management;
using HyperVRemote.Source.Interface;

namespace HyperVRemote.Source.Implementation
{

    public class HyperVMachine : IHyperVMachine
    {
        private readonly ManagementObject _rawMachine;

        public HyperVMachine(ManagementObject rawMachine)
        {
            _rawMachine = rawMachine;         
        }

        public ManagementObject FetchRawMachine()
        {
            return _rawMachine;
        }

        public string GetName()
        {
            return _rawMachine["ElementName"] as string;
        }

        public HyperVStatus GetStatus()
        {           
            return (HyperVStatus)_rawMachine["EnabledState"];
        }


        public void Reset()
        {
            ChangeState(HyperVStatus.Reset);
        }

        public void Start()
        {
            ChangeState(HyperVStatus.Running);
        }

        public void Stop()
        {
            ChangeState(HyperVStatus.Off);
        }

        public void RestoreLastSnapShot()
        {
            var raw = _rawMachine;
            var scope = _rawMachine.Scope;

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

          
            var managementService = new ManagementClass(scope, new ManagementPath("Msvm_VirtualSystemSnapshotService"), null)
                .GetInstances()
                .OfType<ManagementObject>().FirstOrDefault();

            var inParameters = managementService.GetMethodParameters("ApplySnapshot");

            inParameters["Snapshot"] = lastSnapshot.Path.Path;

            var outParameters = managementService.InvokeMethod("ApplySnapshot", inParameters, null);

        }

        private uint ChangeState(HyperVStatus state)
        {
            var raw = _rawMachine;
            var scope = _rawMachine.Scope;

            var managementService = new ManagementClass(scope, new ManagementPath("Msvm_VirtualSystemManagementService"), null)
                .GetInstances()
                .OfType<ManagementObject>().FirstOrDefault();

            if (managementService != null)
            {
                var inParameters = managementService.GetMethodParameters("RequestStateChange");

                inParameters["RequestedState"] = (object)state;

                var outParameters = raw.InvokeMethod("RequestStateChange", inParameters, null);

                Debug.WriteLine("Changed state with return " + outParameters);

                if (outParameters != null) {
                    return (uint)outParameters["ReturnValue"];
                }
            }
            else
            {
                throw new HyperVException("Could not find machine management service for rstate change");
            }

            return 0;
        }

    }
}
