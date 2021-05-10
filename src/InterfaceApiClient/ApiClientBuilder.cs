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
        private readonly IServiceCollection _services;
        private List<Type> _interfaceTypes;
        private ApiClientConfiguration _configuration;

        /// <summary>
        /// Creates new Api client builder
        /// </summary>
        /// <param name="serviceDescriptors">Service collection</param>
        public ApiClientBuilder(IServiceCollection serviceDescriptors)
        {
            _services = serviceDescriptors;
            _interfaceTypes = new List<Type>();
            _configuration = new ApiClientConfiguration();
        }

        /// <inheritdoc/>
        public IServiceCollection Apply()
        {
            Type[] typeList = GetTypesToRegister();
            ApiClientProxyBuilder proxyBuilder = BuildProxies(typeList);
            VerifyConfiguration(proxyBuilder);
            RegisterStaticServices();
            RegisterImplementations(typeList, proxyBuilder);
            return _services;
        }

        private Type[] GetTypesToRegister()
        {
            var registeredServices = _services.Select(service => service.ServiceType).Distinct().ToHashSet();
            var typeList = _interfaceTypes.Distinct().Where(t => !registeredServices.Contains(t)).ToArray();
            return typeList;
        }

        private void RegisterImplementations(Type[] typeList, ApiClientProxyBuilder proxyBuilder)
        {
            foreach (var type in typeList)
            {
                var factory = proxyBuilder.ImplementationFor(type);
                var metadata = new ProxyMetadata(type);
                _services.AddTransient(type, provider => factory(provider.GetRequiredService<IApiClientProxy>(), metadata));
            }
        }

        private void RegisterStaticServices()
        {
            if (!_services.Any(d => d.ServiceType == typeof(IApiClientProxy)))
            {
                _services.AddTransient<IApiClientProxy, ApiClientProxy>();
            }
            _services.AddSingleton(_configuration);
        }

        private ApiClientProxyBuilder BuildProxies(Type[] typeList)
        {
            var proxyBuilder = new ApiClientProxyBuilder(_configuration);
            foreach (var type in typeList)
            {
                proxyBuilder.AddInterface(type);
            }
            proxyBuilder.Build();
            return proxyBuilder;
        }

        private void VerifyConfiguration(ApiClientProxyBuilder proxyBuilder)
        {
            var missingGroups = proxyBuilder.AllGroups
                            .Where(group => !_configuration.Endpoints.ContainsKey(group))
                            .ToArray();
            if (missingGroups.Any())
                throw new KeyNotFoundException($"Endpoint groups not configured: {string.Join(", ", missingGroups)}");
        }

        /// <inheritdoc/>
        public IApiClientBuilder WithConfiguration(Action<ApiClientConfiguration>? configure)
        {
            configure?.Invoke(_configuration);
            return this;
        }

        /// <inheritdoc/>
        public IApiClientBuilder WithTransientAssemblyTypes(Assembly assembly, Action<ApiClientConfiguration>? configure = null)
        {
            var typesToRegister = assembly.GetExportedTypes()
                .Where(t => t.IsInterface && (t.GetCustomAttribute<ApiGroupAttribute>() != null || t.GetMethods().Any(m => m.GetCustomAttribute<ApiEndpointAttribute>() != null)));
            _interfaceTypes.AddRange(typesToRegister);
            configure?.Invoke(_configuration);
            return this;
        }

        /// <inheritdoc/>
        public IApiClientBuilder WithTransientClient<TInterface>(Action<ApiClientConfiguration>? configure = null)
        {
            _interfaceTypes.Add(typeof(TInterface));
            configure?.Invoke(_configuration);
            return this;
        }
    }
}
