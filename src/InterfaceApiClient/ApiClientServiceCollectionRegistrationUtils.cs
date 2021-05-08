using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace InterfaceApiClient
{
    public static class ApiClientServiceCollectionRegistrationUtils
    {
        public static IApiClientBuilder UseInterfaceApiClient(this IServiceCollection serviceCollection, Action<ApiClientConfiguration>? configure = null)
        {
            return new ApiClientBuilder(serviceCollection).WithConfiguration(configure);
        }
    }
}
