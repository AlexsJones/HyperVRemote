using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using AutofacContrib.NSubstitute;
using FluentAssertions;
using HyperVRemote.Source.Implementation;
using HyperVRemote.Source.Interface;
using NSubstitute;
using NUnit.Framework;

namespace HyperVTests
{
    [TestFixture]
    public class HyperVUnitTests
    {

        [Test]
        public void TestGetMachineByName()
        {
            var autoSubstitute = new AutoSubstitute();

            autoSubstitute.Resolve<IHyperVConfiguration>().FetchNamespacePath().Returns("TestNameSpace");

            autoSubstitute.Resolve<IHyperVConfiguration>().FetchServer().Returns("TestServer");

            autoSubstitute.Resolve<IHyperVConfiguration>().FetchUsername().Returns("TestUsername");

            autoSubstitute.Resolve<IHyperVConfiguration>().FetchPassword().Returns("TestPassword");

            var provider = autoSubstitute.ResolveAndSubstituteFor<IHyperVProvider>();

            var machine = new HyperVMachine(
                new ManagementObject("")
                );

            provider.GetMachineByName("Test").Returns( machine );

            provider.GetMachineByName("Test").Should().Be(machine);
        }

        [Test]
        public void TestConfiguration()
        {
            var autoSubstitute = new AutoSubstitute
                (c => c.RegisterType<HyperVProvider>().As<IHyperVProvider>());

            autoSubstitute.Resolve<IHyperVConfiguration>().FetchNamespacePath().Returns("TestNameSpace");

            autoSubstitute.Resolve<IHyperVConfiguration>().FetchServer().Returns("TestServer");

            autoSubstitute.Resolve<IHyperVConfiguration>().FetchUsername().Returns("TestUsername");

            autoSubstitute.Resolve<IHyperVConfiguration>().FetchPassword().Returns("TestPassword");

            var provider = autoSubstitute.Resolve<IHyperVProvider>();

            HyperVProvider hprovider = provider as HyperVProvider;

            hprovider.Options.Username.Should().Be("TestUsername");

        }
    }
}
