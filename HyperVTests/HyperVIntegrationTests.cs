using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using FluentAssertions;
using HyperVRemote.Source.Interface;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace HyperVTests
{
    [Category("Integration")]
    [TestFixture]
    public class HyperVIntegrationTests
    {
        private const string TestMachineName = "VS Emulator 7-inch KitKat (4.4) XHDPI Tablet.darrell";
        private IServiceProvider _container;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

            services.AddHyperVRemote((options) =>
            {
                // defaults to working against local hyperv server which is fine, but lets be explicit.
                options.HyperVServerName = ".";
                options.HyperVUserName = "";  // cant use credentials with local server. see https://blogs.technet.microsoft.com/richard_macdonald/2008/08/11/programming-hyper-v-with-wmi-and-c-getting-started/;

            });

            //_hyperVConfigutation = new HyperVConfiguration(TestUserName, TestPassword, TestDomainName, TestServerName, TestNameSpace);
            //ContainerBuilder builder = new ContainerBuilder();
            //builder.RegisterInstance(_hyperVConfigutation).As<IHyperVConfiguration>();
            //builder.RegisterType<HyperVMachine>().As<IHyperVMachine>();
            //builder.RegisterType<HyperVProvider>().As<IHyperVProvider>();
            _container = services.BuildServiceProvider();
        }

        [Test]
        public void TestFetchMachines()
        {
            var provider = _container.GetRequiredService<IHyperVProvider>();
            provider.Connect();

            IEnumerable<IHyperVMachine> machines = provider.GetMachines();

            foreach (var machine in machines)
            {
                Console.WriteLine("Found machine => " + machine.GetName());
            }
        }

        [TestCase(TestMachineName)]
        public void TestFetchMachineByName(string machineName)
        {
            var provider = _container.GetRequiredService<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(machineName);

            machine.Should().NotBeNull();
        }

        [TestCase(TestMachineName)]
        public void TestResetMachineByName(string machineName)
        {
            var provider = _container.GetRequiredService<IHyperVProvider>();

            provider.Connect();
            IHyperVMachine machine = provider.GetMachineByName(machineName);
            machine.Reset();
        }

        [TestCase(TestMachineName)]
        public void TestCheckpointMachineByName(string machineName)
        {
            var provider = _container.GetRequiredService<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(machineName);

            machine.Stop();

            machine.RestoreLastSnapShot();

            Thread.Sleep(5000);

            machine.Start();

        }


        [TestCase(TestMachineName)]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(Exception))]
        public void TestMachineRestore(string machineName)
        {
            var provider = _container.GetRequiredService<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(machineName);

            machine.Stop();

            Thread.Sleep(2000);

            machine.RestoreLastSnapShot();

            Thread.Sleep(2000);

            machine.Start();
        }

        [TestCase(TestMachineName)]
        public void TestMachineStart(string machineName)
        {
            var provider = _container.GetRequiredService<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(machineName);

            machine.Start();
        }

        [TestCase(TestMachineName)]
        public void TestMachineStop(string machineName)
        {
            var provider = _container.GetRequiredService<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(machineName);

            machine.Stop();
        }

        [TestCase(TestMachineName)]
        public void TestMachinePollStatus(string machineName)
        {
            var provider = _container.GetRequiredService<IHyperVProvider>();

            provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(machineName);

            HyperVStatus status = machine.GetStatus();

            Stopwatch s = new Stopwatch();

            s.Start();

            bool isReset = false;

            while (s.Elapsed.TotalSeconds < 15)
            {
                machine = provider.GetMachineByName(machineName);
                status = machine.GetStatus();

                Debug.WriteLine("Machine Status is => " + status);

                if (s.Elapsed.TotalSeconds >= 1 && !isReset)
                {
                    machine.Reset();

                    isReset = true;
                }

                Thread.Sleep(1000);
            }

            s.Stop();

        }

        [OneTimeTearDown()]
        public void OneTimeTearDown()
        {
            // Ensure we shut down the test image after tests complete.
            var provider = _container.GetRequiredService<IHyperVProvider>();

            Thread.Sleep(5000);
           // provider.Connect();

            IHyperVMachine machine = provider.GetMachineByName(TestMachineName);

            HyperVStatus status = machine.GetStatus();
            if(status != HyperVStatus.Off)
            {
                machine.Stop();
            }          

        }


    }
}
