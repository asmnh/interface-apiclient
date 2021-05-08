using InterfaceApiClient.DataTypes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InterfaceApiClient
{
    public class ApiClientBuilder : IApiClientBuilder
    {
        private readonly IServiceCollection _serviceDescriptors;
        private List<Type> _interfaceTypes;
        private ApiClientConfiguration _configuration;

        public ApiClientBuilder(IServiceCollection serviceDescriptors)
        {
            _serviceDescriptors = serviceDescriptors;
            _interfaceTypes = new List<Type>();
            _configuration = new ApiClientConfiguration();
        }

        public IServiceCollection RegisterAllDependencies()
        {
            if(_serviceDescriptors.Any(d => d.ServiceType == typeof(IApiClientProxy)))
            {
                throw new InvalidOperationException($"ApiClientProxy was already registered.");
            }
            var typeList = _interfaceTypes.Distinct().ToArray();
            var proxyBuilder = new ApiClientProxyBuilder(_configuration);
            foreach(var type in typeList)
            {
                proxyBuilder.AddInterface(type);
            }
            proxyBuilder.Build();

            _serviceDescriptors.AddTransient<IApiClientProxy, ApiClientProxy>();
            _serviceDescriptors.AddSingleton(_configuration);

            foreach (var type in typeList)
            {
                var factory = proxyBuilder.ImplementationFor(type);
                var metadata = new ProxyMetadata(type);
                _serviceDescriptors.AddTransient(type, provider => factory(provider.GetRequiredService<IApiClientProxy>(), metadata));
            }

            return _serviceDescriptors;
        }

        public IApiClientBuilder WithConfiguration(Action<ApiClientConfiguration>? configure)
        {
            configure?.Invoke(_configuration);
            return this;
        }

        public IApiClientBuilder WithTransientAssemblyTypes(Assembly assembly, Action<ApiClientConfiguration>? configure = null)
        {
            var typesToRegister = assembly.GetExportedTypes()
                .Where(t => t.IsInterface && (t.GetCustomAttribute<ApiGroupAttribute>() != null || t.GetMethods().Any(m => m.GetCustomAttribute<ApiEndpointAttribute>() != null)));
            _interfaceTypes.AddRange(typesToRegister);
            configure?.Invoke(_configuration);
            return this;
        }

        public IApiClientBuilder WithTransientClient<TInterface>(Action<ApiClientConfiguration>? configure = null)
        {
            _interfaceTypes.Add(typeof(TInterface));
            configure?.Invoke(_configuration);
            return this;
        }
    }
}
