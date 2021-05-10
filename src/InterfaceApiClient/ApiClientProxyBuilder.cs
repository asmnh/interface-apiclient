using InterfaceApiClient.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("InterfaceApiClient.Tests")]
namespace InterfaceApiClient
{

    internal class ApiClientProxyBuilder
    {
        private readonly ApiClientConfiguration _configuration;

        private readonly List<Type> _pendingInterfaceTypes;
        private readonly Dictionary<Type, MethodInfo> _typeMap;
        private readonly Dictionary<Type, ProxyMetadata> _typeMetadata;
        private readonly List<string> _allGroups;

        public IEnumerable<string> AllGroups => _allGroups.Distinct();


        internal ApiClientProxyBuilder(ApiClientConfiguration configuration)
        {
            _configuration = configuration;
            _pendingInterfaceTypes = new List<Type>();
            _typeMetadata = new Dictionary<Type, ProxyMetadata>();
            _typeMap = new Dictionary<Type, MethodInfo>();
            _allGroups = new List<string>();
        }

        internal void AddInterface(Type interfaceType)
        {
            if (!interfaceType.IsInterface)
                throw new ArgumentException($"Type {interfaceType} is not an interface.", nameof(interfaceType));
            if (_pendingInterfaceTypes.Contains(interfaceType))
                throw new ArgumentException($"Interface of type {interfaceType} already marked for build.", nameof(interfaceType));
            if (_pendingInterfaceTypes.Any(iface => iface.Name == interfaceType.Name))
                throw new ArgumentException($"Interface with name of {interfaceType.Name} already marked for build.", nameof(interfaceType));
            _configuration.ConfigureInterface(interfaceType);
            _pendingInterfaceTypes.Add(interfaceType);
        }

        internal void Build()
        {
            // TODO:
            // this should build new in-memory assembly that contains implementation of all interfaces in _pendingInterfaceTypes
            // for each interface, every method is to be registered in ApiClientConfiguration as a method call
            // here we read configuration and output all matching interfaces
            // each interface has a constructor that takes IApiClientProxy as parameter
            // every method call takes its method info, parameter array and passes it to IApiClientProxy alongside its own type

            MethodInfo createDispatch = typeof(DispatchProxy).GetMethod(nameof(DispatchProxy.Create))!;

            foreach(var type in _pendingInterfaceTypes)
            {
                MethodInfo constructor = createDispatch.MakeGenericMethod(type, typeof(ApiDispatchProxy));
                _typeMap[type] = constructor;
                _typeMetadata[type] = GetMetadata(type);
                _allGroups.AddRange(_typeMetadata[type].Groups);
            }
        }

        private ProxyMetadata GetMetadata(Type type)
        {
            return new ProxyMetadata(type);
        }

        internal Func<IApiClientProxy, ProxyMetadata, object> ImplementationFor(Type interfaceType)
        {
            MethodInfo ctor = _typeMap[interfaceType];
            return (apiClientProxy, metadata) => MakeProxy(ctor, apiClientProxy, metadata);
        }

        private object MakeProxy(MethodInfo ctor, IApiClientProxy apiClientProxy, ProxyMetadata metadata)
        {
            ApiDispatchProxy dispatcher = (ApiDispatchProxy)ctor.Invoke(null, null)!;
            dispatcher.SetApiClientProxy(apiClientProxy);
            dispatcher.SetMetadata(metadata);
            return dispatcher;
        }
    }
}