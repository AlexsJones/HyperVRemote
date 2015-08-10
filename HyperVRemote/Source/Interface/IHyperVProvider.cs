using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace HyperVRemote.Source.Interface
{
    public interface IHyperVProvider
    {
        void Connect();

        IEnumerable<IHyperVMachine> GetMachines();

        IHyperVMachine GetMachineByName(string name);

        string GetName(IHyperVMachine machine);

        void Start(IHyperVMachine machine);

        void Stop(IHyperVMachine machine);

        void Reset(IHyperVMachine machine);

        void RestoreLastSnapShot(IHyperVMachine machine);
    }
}
