using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Autofac;
using FluentAssertions;
using HyperVRemote.Source.Implementation;
using HyperVRemote.Source.Interface;
using NUnit.Framework;

namespace HyperVTests
{
    [Category("Integration")]
    [TestFixture]
    public class HyperVIntegrationTests
    {
        private const string TestMachineName = "VS Emulator 7-inch KitKat (4.4) XHDPI Tablet.darrell";
        private const string TestUserName = @""; // cant use credentials with local server. see https://blogs.technet.microsoft.com/richard_macdonald/2008/08/11/programming-hyper-v-with-wmi-and-c-getting-started/
        private const string TestPassword = @""; 
        private const string TestServerName = @".";
        private const string TestNameSpace = @"root\virtualization\v2";
        private const string TestDomainName = @"";

        private IContainer _container;

        private IHyperVConfiguration _hyperVConfigutation;

        [SetUp]
        public void Setup()
        {
            _hyperVConfigutation = new HyperVConfiguration(TestUserName, TestPassword, TestDomainName, TestServerName, TestNameSpace);
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterInstance(_hyperVConfigutation).As<IHyperVConfiguration>();
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
                Console.WriteLine("Found machine => " + provider.GetName(machine));
            }
        }

        [TestCase(TestMachineName)]
        public void TestFetchMachineByName(string machineName)
        {
            var provider = _container.Resolve<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(machineName);

            machine.Should().NotBeNull();
        }

        [TestCase(TestMachineName)]
        public void TestResetMachineByName(string machineName)
        {
            var provider = _container.Resolve<IHyperVProvider>();

            provider.Connect();
            IHyperVMachine machine = provider.GetMachineByName(machineName);
            provider.Reset(machine);
        }

        [TestCase(TestMachineName)]
        public void TestCheckpointMachineByName(string machineName)
        {
            var provider = _container.Resolve<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(machineName);

            provider.Stop(machine);

            provider.RestoreLastSnapShot(machine);

            Thread.Sleep(5000);

            provider.Start(machine);

        }


        [TestCase(TestMachineName)]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(Exception))]
        public void TestMachineRestore(string machineName)
        {
            var provider = _container.Resolve<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(machineName);

            provider.Stop(machine);

            Thread.Sleep(2000);

            provider.RestoreLastSnapShot(machine);

            Thread.Sleep(2000);

            provider.Start(machine);
        }

        [TestCase(TestMachineName)]
        public void TestMachineStart(string machineName)
        {
            var provider = _container.Resolve<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(machineName);

            provider.Start(machine);
        }

        [TestCase(TestMachineName), NUnit.Framework.Ignore("")]
        public void TestMachineStop(string machineName)
        {
            var provider = _container.Resolve<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(machineName);

            provider.Stop(machine);
        }

        [TestCase(TestMachineName)]
        public void TestMachinePollStatus(string machineName)
        {
            var provider = _container.Resolve<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(machineName);

            HyperVStatus status = provider.GetStatus(machine);

            Stopwatch s = new Stopwatch();
            
            s.Start();

            bool isReset = false;

            while (s.Elapsed.TotalSeconds < 15)
            {
                machine = provider.GetMachineByName(machineName);
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
