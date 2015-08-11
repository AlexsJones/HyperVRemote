using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Autofac;
using Autofac.Core;
using FluentAssertions;
using HyperVRemote.Source.Implementation;
using HyperVRemote.Source.Interface;
using NUnit.Framework;

namespace HyperVTests
{
    [TestFixture]
    public class HyperVIntegrationTests
    {
        private const string MachineName = "__NOT__SET__";
        private const string TestUserName = @"__NOT__SET__";
        private const string TestPassword = @"__NOT__SET__";
        private const string TestServerName = @"__NOT__SET__";
        private const string TestNameSpace = @"root\virtualization\v2";
        private IContainer _container;
        [SetUp]
        public void Setup()
        {
            ContainerBuilder builder = new ContainerBuilder();

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

            _container = builder.Build();
        }

        [Test]
        public void TestFetchMachines()
        {
            var provider = _container.Resolve<IHyperVProvider>();

            provider.Connect();

            IEnumerable<IHyperVMachine> machines = provider.GetMachines();

            foreach (var machine in machines)
            {
                Debug.WriteLine("Found machine => " + provider.GetName(machine));
            }
        }

        [Test]
        public void TestFetchMachineByName()
        {
            var provider = _container.Resolve<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(MachineName);

            machine.Should().NotBeNull();
        }

        [Test, NUnit.Framework.Ignore]
        public void TestResetMachineByName()
        {
            var provider = _container.Resolve<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(MachineName);

            provider.Reset(machine);
        }

        [Test, NUnit.Framework.Ignore]
        public void TestCheckpointMachineByName()
        {
            var provider = _container.Resolve<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(MachineName);

            provider.Stop(machine);

            provider.RestoreLastSnapShot(machine);

            Thread.Sleep(5000);

            provider.Start(machine);

        }


        [Test, NUnit.Framework.Ignore]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(Exception))]
        public void TestMachineRestore()
        {
            var provider = _container.Resolve<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(MachineName);

            provider.RestoreLastSnapShot(machine);
        }

        [Test, NUnit.Framework.Ignore]
       
        public void TestMachineStart()
        {
            var provider = _container.Resolve<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(MachineName);

            provider.Start(machine);
        }

        [Test, NUnit.Framework.Ignore]
        public void TestMachineStop()
        {
            var provider = _container.Resolve<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(MachineName);

            provider.Stop(machine);
        }

        [Test, NUnit.Framework.Ignore]
        public void TestMachinePollStatus()
        {
            var provider = _container.Resolve<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(MachineName);

            HyperVStatus status = provider.GetStatus(machine);

            Stopwatch s = new Stopwatch();
            
            s.Start();

            bool isReset = false;

            while (s.Elapsed.TotalSeconds < 15)
            {
                machine = provider.GetMachineByName(MachineName);
                status = provider.GetStatus(machine);

                Debug.WriteLine("Machine Status is => " + status);

                if (s.Elapsed.TotalSeconds >= 1 && !isReset)
                {
                    provider.Reset(machine);

                    isReset = true;
                }

                Thread.Sleep(1000);
            }

            s.Stop();

        }
    }
}
