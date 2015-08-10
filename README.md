# HyperVRemote
nuget for hyperv remote

##Examples

```C#
var provider = _container.Resolve<IHyperVProvider>();

provider.Connect();

IHyperVMachine machine = provider.GetMachineByName(MachineName);

provider.Reset(machine);
```
