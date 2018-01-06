using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace HyperVRemote.Source.Interface
{
    public enum HyperVStatus : ushort
    {
        Other = 1,
        Running = 2,
        Off = 3,
        Saved = 6,
        Paused = 9,
        Starting = 10,
        Reset = 11,
        Saving = 32773,
        Pausing = 32776,
        Resuming = 32777,
        FastSaved = 32779,
        FastSaving = 32780,
    };

    public interface IHyperVProvider
    {
     
        void Connect();

        IEnumerable<IHyperVMachine> GetMachines();

        IHyperVMachine GetMachineByName(string name);      

      
    }
}
