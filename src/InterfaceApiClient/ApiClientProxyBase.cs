using System;
using System.Net.Http;

namespace InterfaceApiClient
{
    public abstract class ApiClientProxyBase : IDisposable
    {
        private bool _disposed;

        public ApiClientProxyBase(IApiClientProxy apiClientProxy)
        {
        }

        protected abstract bool TryInvokeMember(Type interfaceType, string name, object?[] args, out object? result);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // TODO: dispose if needed
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
