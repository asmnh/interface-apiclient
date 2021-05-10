using InterfaceApiClient.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InterfaceApiClient
{
    public class ProxyMetadata
    {
        private Type _interfaceType;
        private Dictionary<MethodInfo, MethodMetadata> _methods;

        public ProxyMetadata(Type interfaceType)
        {
            _interfaceType = interfaceType;
            _methods = new Dictionary<MethodInfo, MethodMetadata>();

            var methods = interfaceType.GetMethods().Where(meth => meth.GetCustomAttributes().OfType<ApiEndpointAttribute>().Any());
            foreach(var method in methods)
            {
                LoadMethodMetadata(method);
            }
        }

        public IEnumerable<string> Groups => _methods.Select(m => m.Value.GroupName).Distinct();

        internal MethodMetadata Method(MethodInfo method)
        {
            return _methods[method];
        }

        private void LoadMethodMetadata(MethodInfo method)
        {
            _methods[method] = new MethodMetadata(method);
        }
    }
}