using FluentAssertions;
using HyperVRemote.Source.Implementation;
using HyperVRemote.Source.Interface;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace HyperVTests
{
    [TestFixture]
    public class HyperVUnitTests
    {       

        [Test]
        public void TestConfiguration()
        {

            var services = new ServiceCollection();
            services.AddHyperVRemote((options) =>
            {
                // defaults to working against local hyperv server which is fine, but lets be explicit.
                options.HyperVServerName = "TestServer";
                options.HyperVUserName = "TestUsername";  // cant use credentials with local server. see https://blogs.technet.microsoft.com/richard_macdonald/2008/08/11/programming-hyper-v-with-wmi-and-c-getting-started/;
                options.HyperVUserPassword = "TestPassword";
                options.HyperVNameSpace = "TestNameSpace";
            });
            var sp = services.BuildServiceProvider();
            var provider = sp.GetRequiredService<IHyperVProvider>();

            HyperVProvider hprovider = provider as HyperVProvider;
            hprovider.ConnectionOptions.Username.Should().Be("TestUsername");

        }
    }
}
