using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
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

        public string FetchName()
        {
            return _rawMachine["ElementName"] as string;
        }
    }
}
