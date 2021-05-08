using System;
using System.Collections.Generic;
using System.Reflection;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("InterfaceApiClient.Tests")]
namespace InterfaceApiClient
{
    public class ApiDispatchProxy : System.Reflection.DispatchProxy
    {
        private IApiClientProxy _apiClientProxy = null!;
        private readonly MethodInfo _callMethodBase;
        private Dictionary<Type, MethodInfo> _specializedCache;
        private ProxyMetadata _metadata = null!;

        public ApiDispatchProxy()
        {
            _callMethodBase = typeof(IApiClientProxy).GetMethod(nameof(IApiClientProxy.Call))!;
            _specializedCache = new Dictionary<Type, MethodInfo>();
        }

        public void SetApiClientProxy(IApiClientProxy apiClientProxy)
        {
            _apiClientProxy = apiClientProxy;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if(targetMethod == null)
            {
                throw new ArgumentNullException(nameof(targetMethod));
            }
            var callee = SpecializeForReturnType(targetMethod.ReturnType);
            return callee.Invoke(_apiClientProxy, new object?[] { targetMethod, _metadata, args });
        }

        private MethodInfo SpecializeForReturnType(Type returnType)
        {
            if (!_specializedCache.TryGetValue(returnType, out MethodInfo? result))
            {
                result = _callMethodBase.MakeGenericMethod(returnType);
                _specializedCache[returnType] = result;
            }
            return result;
        }

        internal void SetMetadata(ProxyMetadata metadata)
        {
            _metadata = metadata;
        }
    }
}