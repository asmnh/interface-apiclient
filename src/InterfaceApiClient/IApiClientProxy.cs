using System.Reflection;
using System.Threading.Tasks;

namespace InterfaceApiClient
{
    public interface IApiClientProxy
    {
        public TResult Call<TResult>(MethodInfo method, ProxyMetadata metadata, object?[] args);
    }
}