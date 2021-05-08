using System;
using System.Net;

namespace InterfaceApiClient.DataTypes
{
    [AttributeUsage(AttributeTargets.Method|AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class MapErrorAttribute : Attribute
    {
        /// <summary>
        /// Map response status code to an exception.
        /// </summary>
        /// <param name="statusCode">HTTP status code to treat as exception.</param>
        /// <param name="exception">Exception type to be thrown.</param>
        /// <remarks>
        /// Thrown exception must have parameterless constructor or support <c>string? message</c> and string-nullable object dictionary constructor.
        /// Some system exceptions (builtin subclasses of <see cref="ArgumentException"/>) also support specific constructors.
        /// </remarks>
        public MapErrorAttribute(HttpStatusCode statusCode, Type exception)
        {
            StatusCode = statusCode;
            ExceptionType = exception;
        }

        /// <summary>
        /// Status code treated as exception.
        /// </summary>
        public HttpStatusCode StatusCode { get; }
        /// <summary>
        /// Exception type to be thrown.
        /// </summary>
        public Type ExceptionType { get; }
    }
}
