namespace HyperVRemote.Source.Interface
{
    public interface IHyperVMachine
    {
        string GetName();

        HyperVStatus GetStatus();

        void Start();

        void Stop();

        void Reset();

        void RestoreLastSnapShot();

    }
}
