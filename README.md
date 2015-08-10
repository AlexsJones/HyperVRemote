# HyperVRemote
Nuget for hyperv control from C#

Best used with inversion of control

Get it here: `Install-Package HyperVRemote`

##Example

```C#
var provider = _container.Resolve<IHyperVProvider>();

provider.Connect();

IEnumerable<IHyperVMachine> machines = provider.GetMachines();

foreach (var machine in machines)
{
  Debug.WriteLine("Found machine => " + provider.GetName(machine));
}

IHyperVMachine machine = provider.GetMachineByName("SomeVM");

provider.Reset(machine);
```
