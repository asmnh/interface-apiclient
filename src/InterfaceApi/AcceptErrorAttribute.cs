using System;
using System.Net;

namespace InterfaceApiClient.DataTypes
{
    [AttributeUsage(AttributeTargets.Interface|AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AcceptErrorAttribute : Attribute
    {
        /// <summary>
        /// Mark error response status code as not-an-error.
        /// </summary>
        /// <param name="statusCode">Status code to be consider correct response.</param>
        public AcceptErrorAttribute(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Status code to be considered correct response.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// If set, response parsing will be skipped and instead default value will be returned.
        /// </summary>
        public bool ReturnDefault { get; init; }
    }
}
