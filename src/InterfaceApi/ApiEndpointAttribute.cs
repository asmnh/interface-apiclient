using System;
using System.Net.Http;

namespace InterfaceApiClient.DataTypes
{
    /// <summary>
    /// Marks API endpoint.
    /// </summary>
    /// <remarks>
    /// API method will have its implementation generated for API client, and will provide OpenAPI details for method implementation.
    /// 
    /// Endpoint is mandatory for all methods that will have client code generated. Lack of marking will - depending on configuration - result in exception on startup or a method returning default value.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ApiEndpointAttribute : Attribute
    {
        /// <summary>
        /// HTTP method used for call by API.
        /// </summary>
        public string HttpMethodString { get; init; }

        /// <summary>
        /// HTTP method used for call by API - helper to allow setting value using enum.
        /// </summary>
        public HttpRequestMethod Method
        {
            get { return (HttpRequestMethod)Enum.Parse(typeof(HttpRequestMethod), HttpMethodString); }
            init { HttpMethodString = value.ToString(); }
        }
        /// <summary>
        /// Endpoint relative URI.
        /// </summary>
        public string Endpoint { get; init; }

        /// <summary>
        /// Mark API endpoint.
        /// </summary>
        /// <param name="method">HTTP method used by endpoint.</param>
        /// <param name="endpoint">Endpoint relative URI.</param>
        public ApiEndpointAttribute(HttpRequestMethod method, string endpoint)
        {
            HttpMethodString = method.ToString();
            Endpoint = endpoint;
        }

        public ApiEndpointAttribute(string method, string endpoint)
        {
            HttpMethodString = method;
            Endpoint = endpoint;
        }
    }
}
