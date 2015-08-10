using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HyperVRemote.Source.Interface
{
    public interface IHyperVConfiguration
    {
        string FetchUsername();

        string FetchPassword();

        string FetchServer();

        string FetchNamespacePath();

        TimeSpan Timeout();
    }
}
