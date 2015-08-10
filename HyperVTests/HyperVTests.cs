using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autofac;
using Autofac.Core;
using FluentAssertions;
using HyperVRemote.Source.Implementation;
using HyperVRemote.Source.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace HyperVTests
{
    [TestFixture]
    public class HyperVTests
    {
        private const string MachineName = "__YOUR__COMPUTER_OR__VM__";
        private const string TestUserName = @"__YOUR_USERNAME__";
        private const string TestPassword = @"__OUR_PASSWORD__";
        private const string TestServerName = @"__YOUR_SERVER__";
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
    }
}
