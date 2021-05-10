using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace InterfaceApiClient
{
    public class ApiClientProxy : IApiClientProxy
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiClientConfiguration _configuration;

        public ApiClientProxy(IHttpClientFactory httpClientFactory, ApiClientConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        TResult IApiClientProxy.Call<TResult>(MethodInfo method, ProxyMetadata metadata, object?[] args)
        {
            var methodMetadata = metadata.Method(method);
            var task = CallAsync(methodMetadata, args);
            if (methodMetadata.IsAsync)
            {
                var castMethodTemplate = typeof(ApiClientProxy).GetMethod(nameof(CastResponse), BindingFlags.Static|BindingFlags.NonPublic)!;
                var castMethod = castMethodTemplate.MakeGenericMethod(methodMetadata.ReturnType);
                var result = castMethod.Invoke(null, new object?[] { task });
                return (TResult)result!;
            }
            else
            {
                task.Wait();
                return (TResult)task.Result!;
            }
        }

        protected static Task<TType> CastResponse<TType>(Task<object?> task) => CastResponseAsync<TType>(task);

        private static async Task<TType> CastResponseAsync<TType>(Task<object?> task) => (TType)(await task)!;

        private async Task<object?> CallAsync(MethodMetadata methodMetadata, object?[] args)
        {
            if (!methodMetadata.IsApiMethod)
            {
                return methodMetadata.HasReturnValue ? Activator.CreateInstance(methodMetadata.ReturnType) : default;
            }

            string fullUri = GetRequestUri(methodMetadata, args);
            var headers = methodMetadata.BuildHeaders(args);
            var query = methodMetadata.BuildQuery(args);
            string? queryString = ToQueryString(query);
            if (!string.IsNullOrWhiteSpace(queryString))
            {
                fullUri += (fullUri.Contains('?') ? '&' : '?') +queryString;
            }
            var body = methodMetadata.BuildBody(args);

            using var httpClient = _httpClientFactory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(methodMetadata.HttpMethod), fullUri);
            foreach(var header in headers)
            {
                if(!string.IsNullOrWhiteSpace(header.Value))
                    request.Headers.Add(header.Key, header.Value);
            }

            if(body != null)
            {
                request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            }

            var response = await httpClient.SendAsync(request);

            string responseString = await response.Content.ReadAsStringAsync();

            Exception? exception = methodMetadata.BuildResponseException(response.StatusCode, responseString);
            if (exception != null)
                throw exception;

            if (!methodMetadata.HasReturnValue)
                return null;

            return JsonSerializer.Deserialize(responseString, methodMetadata.ReturnType);
        }

        private string? ToQueryString(IDictionary<string, string> query)
        {
            List<string> queryValues = new();
            foreach(var pair in query)
            {
                if(!string.IsNullOrEmpty(pair.Value))
                {
                    queryValues.Add(FormatQueryParam(pair.Key, pair.Value));
                }
            }
            return queryValues.Any() ? string.Join('&', queryValues) : null;
        }

        private string FormatQueryParam(string key, string value)
        {
            return $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}";
        }

        private string GetRequestUri(MethodMetadata methodMetadata, object?[] args)
        {
            var groupName = methodMetadata.GroupName;
            var groupUri = _configuration.Endpoints[groupName];
            if (groupUri[^1] == '/')
                groupUri = groupUri[..^1];
            var methodUri = methodMetadata.BuildPath(args);
            if (methodUri[0] == '/')
                methodUri = methodUri[1..];
            string fullUri = string.Join('/', groupUri, methodUri);
            return fullUri;
        }
    }
}