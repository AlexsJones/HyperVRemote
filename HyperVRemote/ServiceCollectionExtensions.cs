using HyperVRemote.Source.Implementation;
using HyperVRemote.Source.Interface;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHyperVRemote(this IServiceCollection services, Action<HyperVRemoteOptions> configure)
        {
            var options = new HyperVRemoteOptions();
            if (configure != null)
            {
                services.AddOptions();
                services.Configure<HyperVRemoteOptions>(configure);
            }

            services.AddTransient<IHyperVProvider, HyperVProvider>();
            return services;
        }
    }
}
