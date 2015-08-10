# HyperVRemote
Nuget for hyperv control from C#

Best used with inversion of control

Get it here: `Install-Package HyperVRemote`

###Initialisation
An example when adding to IoC
```C#
private const string MachineName = "__YOUR__COMPUTER_OR__VM__";
private const string TestUserName = @"__YOUR_USERNAME__";
private const string TestPassword = @"__OUR_PASSWORD__";
private const string TestServerName = @"__YOUR_SERVER__";
private const string TestNameSpace = @"root\virtualization\v2";

builder.RegisterType<HyperVConfiguration>().As<IHyperVConfiguration>().WithParameters(new[]
{
  new ResolvedParameter((p,c) =>
  p.Name == "username",
  (p,c) => TestUserName),
  
  new ResolvedParameter((p,c) =>
  p.Name == "userpassword",
  (p,c) => TestPassword),
  
  new ResolvedParameter((p,c) =>
  p.Name == "servername",
  (p,c) => TestServerName),
  
  new ResolvedParameter((p,c) =>
  p.Name == "nameSpace",
  (p,c) => TestNameSpace),
});

builder.RegisterType<HyperVMachine>().As<IHyperVMachine>();
builder.RegisterType<HyperVProvider>().As<IHyperVProvider>();
```

Or initialising standalone

```
HyperVConfiguration configuration = new HyperVConfiguration {
  TestUserName,
  TestUserPassword,
  TestServerName,
  TestNameSpace
};
HyperVProvider provider = new HyperVProvider(configuration);
```
###Example

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
