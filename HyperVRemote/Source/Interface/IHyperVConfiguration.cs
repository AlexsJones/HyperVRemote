using System;

namespace HyperVRemote.Source.Interface
{
    public interface IHyperVConfiguration
    {
        string FetchUsername();

        string FetchPassword();

        string FetchDomain();

        string FetchServer();

        string FetchNamespacePath();

        TimeSpan Timeout();
    }
}
